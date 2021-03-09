using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StatechartInstance : MonoBehaviour
{
    readonly HashSet<SCInternalEvent> events = new HashSet<SCInternalEvent>();
    readonly Dictionary<string, bool> properties = new Dictionary<string, bool>();
    readonly Dictionary<Action, EventHandler<SCEventArgs>> actions = new Dictionary<Action, EventHandler<SCEventArgs>>();

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


    public void Step()
    {
#if SC_PROFILE_SINGLE
        stopwatch.Start();
#endif
        // Preparations

        var snap = new Snapshot(properties, events);
        events.Clear();

        var paths = new List<Path>();

        var active = new HashSet<State>();
        var entered = new HashSet<State>();
        var exited = new HashSet<State>();

#if SC_PROFILE_SINGLE
        stopwatch.Stop();
        prepare.Accumulate(stopwatch);
        stopwatch.Reset();
        stopwatch.Start();
#endif
        // Search Paths

        foreach (AtomicState s in config.atomicState)
        {
            active.UnionWith(s.GetAncestors(null));
            
            var next = s.TryExit(snap);
            if (next != (null, null))
                paths.Add(new Path(s, next.waypoints, next.destinations));
        }

#if SC_PROFILE_SINGLE
        stopwatch.Stop();
        search.Accumulate(stopwatch);
        stopwatch.Reset();
        stopwatch.Start();
#endif
        // Validate Paths

        // Sort by scope high to low
        paths.Sort();

        var valid_paths = new List<Path>();

        // Remove conflicting paths by priority
        foreach (var p in paths)
        {
            bool valid = true;
            foreach (var e in exited)
            {
                if (p.GetSource().IsChildOf(e))
                {
                    Move(active, exited, p.GetSource().GetAncestors(e));
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                valid_paths.Add(p);
                entered.UnionWith(p.GetEntered());
                Move(active, exited, p.GetExited());
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
        foreach (var p in valid_paths)
            DoActions(p.GetWaypoints(), Action.Type.PASSTHROUGH);
        DoActions(entered, Action.Type.ENTRY);

        foreach (var s in exited)
            events.Add(new SCInternalEvent("exited." + s.ToString()));
        foreach (var s in entered)
            events.Add(new SCInternalEvent("entered." + s.ToString()));

        config.atomicState.ExceptWith(ExtractAtomic(exited));
        config.atomicState.UnionWith(ExtractAtomic(entered));

#if SC_PROFILE_SINGLE
        stopwatch.Stop();
        execute.Accumulate(stopwatch);
        stopwatch.Reset();
        Debug.Log(prepare.SampleCount + "\t Values: " + prepare.AverageµS + "\t" + search.AverageµS + "\t" + validate.AverageµS + "\t" + execute.AverageµS);
#endif

#if SC_LOG_FUNCTIONALITY
        Debug.Log(this + " is now in " + config.ToString());
#endif
    }


    void DoActions(IEnumerable<ISCElement> objects, Action.Type type)
    {
#if SC_LOG_FUNCTIONALITY
        var sb = new System.Text.StringBuilder();
#endif
        foreach(var o in objects)
        {
            var action = new Action(o.ToString(), type);
            if (actions.TryGetValue(action, out EventHandler<SCEventArgs> act))
                act?.Invoke(this, new SCEventArgs(o.ToString(), type));
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


    void Move<T>(ISet<T> from, ISet<T> to, ICollection<T> collection)
    {
        from.ExceptWith(collection);
        to.UnionWith(collection);
    }


    public void AddEvent(SCInternalEvent e)
    {
        events.Add(e);

        if (machine.GetMode() == Statechart.Mode.On_Event)
            Step();
    }


    public void Subscribe(string source, Action.Type type, EventHandler<SCEventArgs> function)
    {
        var key = new Action(source, type);
        if (actions.ContainsKey(key))
            actions[key] += function;
        else
            actions[key] = function;
    }


    public void Unsubscribe(string source, Action.Type type, EventHandler<SCEventArgs> function)
    {
        var key = new Action(source, type);
        if (actions.ContainsKey(key))
            actions[key] -= function;
    }


    public void SetProperty(string name, bool value)
    {
        properties[name] = value;
    }


    public bool GetProperty(string name)
    {
        properties.TryGetValue(name, out bool value);
        return value;
    }


    public bool IsStateActive(string name)
    {
        foreach (var s in config.atomicState)
            foreach (var a in s.GetAncestors(null))
                if (a.ToString().Equals(name))
                    return true;

        return false;
    }


    public Statechart.Mode GetMode()
    {
        return machine.GetMode();
    }
}
