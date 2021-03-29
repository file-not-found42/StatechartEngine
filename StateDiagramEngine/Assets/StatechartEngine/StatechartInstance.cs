using System;
using System.Collections.Generic;
using UnityEngine;

public class StatechartInstance : MonoBehaviour
{
    readonly HashSet<SCEvent> events = new HashSet<SCEvent>();
    readonly Dictionary<string, bool> properties = new Dictionary<string, bool>();
    readonly Dictionary<Action, EventHandler<ActionArgs>> actions = new Dictionary<Action, EventHandler<ActionArgs>>();

    Configuration config;
    
    [SerializeField]
    Statechart machine;

#if SC_PROFILE_SINGLE
    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    AccumulatedTime prepare = new AccumulatedTime();
    AccumulatedTime search = new AccumulatedTime();
    AccumulatedTime validate = new AccumulatedTime();
    AccumulatedTime execute = new AccumulatedTime();
#endif

    public void Initialize(Statechart chart)
    {
        machine = chart;
        Awake();
    }


    void Awake()
    {
        if (machine == null)
            Debug.LogError(this + " does not have a statechart attached!");
        else
            config = machine.Instantiate();
    }


    void OnEnable()
    {
        StatechartEngine.AddInstance(this);
    }


    void OnDisable()
    {
        StatechartEngine.RemoveInstance(this);
    }


    public bool SuperStep()
    {
        uint i = 0;
        
        while (i < machine.GetStepSize() && Step()) 
            i++;
        
        return i != 0;
    }


    bool Step()
    {
#if SC_PROFILE_SINGLE
        stopwatch.Start();
#endif
        // Preparations

        var snap = new Status(properties, events);
        events.Clear();

        var CTs = new List<CT>();

        var entered = new HashSet<State>();
        var exited = new HashSet<State>();
        var active = new HashSet<State>();
        foreach (var s in config.activeStates)
            active.UnionWith(s.GetSuperstates(null));

#if SC_PROFILE_SINGLE
        stopwatch.Stop();
        prepare.Accumulate(stopwatch);
        stopwatch.Reset();
        stopwatch.Start();
#endif
        // Search Paths

        foreach (AtomicState s in config.activeStates)
        {   
            var next = s.TryExit(snap);
            if (next != (null, null))
                CTs.Add(new CT(s, next.waypoints, next.destinations));
        }

#if SC_PROFILE_SINGLE
        stopwatch.Stop();
        search.Accumulate(stopwatch);
        stopwatch.Reset();
        stopwatch.Start();
#endif
        // Validate Paths

        // Sort by scope high to low
        CTs.Sort();

        // Remove conflicting paths by priority
        var full_CTs = new List<CT>();
        foreach (var ct in CTs)
        {
            if (!active.IsSupersetOf(ct.GetSource().GetSuperstates(null)))
            {
                var ct_exits = ct.GetExited();
                var ct_enters = ct.GetEntered();

                active.ExceptWith(ct_exits);
                exited.UnionWith(ct_exits);
                entered.UnionWith(ct_enters);

                full_CTs.Add(ct);
            }
        }
        // Remove any so far untouched parallel regions which have been implicitly exited
        {
            var ToRemove = new HashSet<State>();
            
            foreach (var act in active)
            {
                var ancestors = act.GetSuperstates(null);
                ancestors.Reverse();
                foreach (var a in ancestors)
                    if (!active.Contains(a))
                    {
                        ToRemove.UnionWith(act.GetSuperstates(a));
                        break;
                    }
            }

            active.ExceptWith(ToRemove);
            exited.UnionWith(ToRemove);
        }
        // Implicitly enter all so far untouched parallel regions
        {
            var regions = new HashSet<State>();
            foreach (var e in entered)
                if (e is ParallelState p)
                    regions.UnionWith(p.components);

            regions.ExceptWith(active);
            regions.ExceptWith(entered);

            foreach (var r in regions)
            {
                var next = r.TryEnter(snap);
                if (next != (null, null))
                {
                    entered.UnionWith(next.destinations);
                }
                // else: ERROR (Limitation)
            }
        }

#if SC_PROFILE_SINGLE
        stopwatch.Stop();
        validate.Accumulate(stopwatch);
        stopwatch.Reset();
        stopwatch.Start();
#endif
        // Execute Step

        DoActions(exited, Action.Type.EXIT);
        DoActions(active, Action.Type.STAY);
        foreach (var p in full_CTs)
            DoActions(p.GetWaypoints(), Action.Type.PASSTHROUGH);
        DoActions(entered, Action.Type.ENTRY);

        foreach (var s in exited)
            events.Add(new SCEvent("exited." + s.ToString()));
        foreach (var s in entered)
            events.Add(new SCEvent("entered." + s.ToString()));

        config.activeStates.ExceptWith(ExtractAtomic(exited));
        config.activeStates.UnionWith(ExtractAtomic(entered));

#if SC_PROFILE_SINGLE
        stopwatch.Stop();
        execute.Accumulate(stopwatch);
        stopwatch.Reset();
        if (prepare.SampleCount % 1000 == 0) // Reduce clutter
            Debug.Log(
                "Prepare: " + prepare.GetStatistics(AccumulatedTime.TimeUnit.탎) + 
                "\nSearch: " + search.GetStatistics(AccumulatedTime.TimeUnit.탎) + 
                "\nValidate: " + validate.GetStatistics(AccumulatedTime.TimeUnit.탎) + 
                "\nExecute: " + execute.GetStatistics(AccumulatedTime.TimeUnit.탎));
#endif

#if SC_LOG_FUNCTIONALITY
        Debug.Log(this + " is now in " + config.ToString());
#endif
#if SC_DEBUG
        if (!config.IsValid())
            Debug.LogError("Invalid configuration in instance " + this + ":\n" + config.ToString());
#endif

        return exited.Count > 0 || entered.Count > 0;
    }


    void DoActions(IEnumerable<ISCElement> sources, Action.Type type)
    {
#if SC_LOG_FUNCTIONALITY
        var sb = new System.Text.StringBuilder();
#endif
        foreach(var o in sources)
        {
            var action = new Action(o.ToString(), type);
            if (actions.TryGetValue(action, out EventHandler<ActionArgs> act))
                act?.Invoke(this, new ActionArgs(o.ToString(), type));
#if SC_LOG_FUNCTIONALITY
            sb.Append(action);
            sb.Append(", ");
#endif
        }

#if SC_LOG_FUNCTIONALITY
        if (sb.Length > 0)
            Debug.Log("Executed the following actions:\n" + sb.ToString());
#endif
    }


    HashSet<AtomicState> ExtractAtomic(ISet<State> source)
    {
        var result = new HashSet<AtomicState>();

        foreach (var s in source)
            if (s is AtomicState a)
                result.Add(a);

        return result;
    }


    public void AddEvent(SCEvent e)
    {
        events.Add(e);

#if SC_LOG_FUNCTIONALITY
        Debug.Log("Added event \"" + e.ToString() + "\" to statechart " + this);
#endif

        if (machine.GetMode() == Statechart.Mode.On_Event)
            SuperStep();
    }


    public void Subscribe(string source, Action.Type type, EventHandler<ActionArgs> function)
    {
        var key = new Action(source, type);
        if (actions.ContainsKey(key))
            actions[key] += function;
        else
            actions[key] = function;
    }


    public void Unsubscribe(string source, Action.Type type, EventHandler<ActionArgs> function)
    {
        var key = new Action(source, type);
        if (actions.ContainsKey(key))
            actions[key] -= function;
    }


    public void SetProperty(string name, bool value)
    {
        properties[name] = value;

#if SC_LOG_FUNCTIONALITY
        Debug.Log("Set property \"" + name + "\" to \"" + value + "\" to statechart " + this);
#endif
    }


    public bool GetProperty(string name)
    {
        properties.TryGetValue(name, out bool value);
        return value;
    }


    public bool IsStateActive(string name)
    {
        foreach (var s in config.activeStates)
            foreach (var a in s.GetSuperstates(null))
                if (a.ToString().Equals(name))
                    return true;

        return false;
    }


    public Statechart.Mode GetMode()
    {
        return machine.GetMode();
    }
}
