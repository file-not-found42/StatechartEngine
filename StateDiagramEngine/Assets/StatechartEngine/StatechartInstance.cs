using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StatechartInstance : MonoBehaviour
{
    readonly Dictionary<Action, EventHandler<ActionArgs>> actions = new Dictionary<Action, EventHandler<ActionArgs>>();

    Status status;
    
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
            status = machine.Instantiate();

#if SC_LOG_FUNCTIONALITY
        Debug.Log(this + " was initialized to " + status.ToString());
#endif
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


        var CTs = new List<CT>();

        var entered = new HashSet<State>();
        var exited = new HashSet<State>();
        var active = new HashSet<State>();
        foreach (var s in status.b_configuration)
            active.UnionWith(s.GetSuperstates(null));

#if SC_PROFILE_SINGLE
        stopwatch.Stop();
        prepare.Accumulate(stopwatch);
        stopwatch.Reset();
        stopwatch.Start();
#endif
        // Search Paths

        foreach (AtomicState s in status.b_configuration)
        {   
            var next = s.TryExit(status);
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
        for (int i = 0; i < CTs.Count; i++)
        {
            if (active.IsSupersetOf(CTs[i].GetSource().GetSuperstates(null)))
            {
                var ct_exits = CTs[i].GetExited();
                var ct_enters = CTs[i].GetEntered();

                active.ExceptWith(ct_exits);
                exited.UnionWith(ct_exits);
                entered.UnionWith(ct_enters);
            }
            else
                CTs[i] = null;
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
                var next = r.TryEnter(status);
                if (next != (null, null))
                {
                    entered.UnionWith(next.destinations);
                }
                // else: ERROR (Limitation)
            }
        }
        // Cleanup
        entered.ExceptWith(active);

#if SC_PROFILE_SINGLE
        stopwatch.Stop();
        validate.Accumulate(stopwatch);
        stopwatch.Reset();
        stopwatch.Start();
#endif
        // Execute Step

        // Update configuration
        status.b_configuration.ExceptWith(ExtractAtomic(exited));
        status.b_configuration.UnionWith(ExtractAtomic(entered));
        // Execute actions
        DoActions(exited, Action.Type.EXIT);
        DoActions(active, Action.Type.STAY);
        foreach (var p in CTs)
            if (p != null)
                DoActions(p.GetWaypoints(), Action.Type.PASSTHROUGH);
        DoActions(entered, Action.Type.ENTRY);
        // Clear and add events for next step
        status.events.Clear();
        foreach (var s in exited)
            status.events.Add(new SCEvent("exit." + s.ToString()));
        foreach (var s in entered)
            status.events.Add(new SCEvent("enter." + s.ToString()));

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
        Debug.Log(this + " is now in " + status.ToString());
#endif

#if SC_DEBUG
        if (!status.IsValid())
        {
            StringBuilder sb = new StringBuilder("Invalid configuration in instance ");
            sb.Append(this);
            sb.Append(":\n\t");
            sb.Append(status.ToString());
            sb.Append("\nIt occured while executing:");
            foreach (var ct in CTs)
                if(ct != null)
                {
                    sb.Append(ct);
                    sb.Append("\n\t");
                }
            Debug.LogError(sb.ToString());
        }
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
        status.events.Add(e);

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
        status.properties[name] = value;

#if SC_LOG_FUNCTIONALITY
        Debug.Log("Set property \"" + name + "\" to \"" + value + "\" to statechart " + this);
#endif
    }


    public bool GetProperty(string name)
    {
        status.properties.TryGetValue(name, out bool value);
        return value;
    }


    public bool IsStateActive(string name)
    {
        foreach (var s in status.b_configuration)
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
