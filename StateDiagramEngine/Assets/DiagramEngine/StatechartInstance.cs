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
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
#endif
        var snap = new Snapshot(config, properties, events);
        events.Clear();

        var paths = new List<Path>();

        foreach (AtomicState s in config.atomicState)
        {
            Path p = new Path(s);
            if (s.TryExit(p, snap))
                paths.Add(p);
        }

        HashSet<State> entered = new HashSet<State>();
        HashSet<State> exited = new HashSet<State>();

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
                    exited.UnionWith(p.GetSource().GetAncestors(e));
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                valid_paths.Add(p);
                entered.UnionWith(p.GetEntered());
                exited.UnionWith(p.GetExited());
            }
        }

        config.atomicState.ExceptWith(ExtractAtomic(exited));

        DoActions(exited, Action.Type.EXIT);
        DoActions(config.atomicState, Action.Type.STAY);
        foreach (var p in valid_paths)
            DoActions(p.GetWaymarks(), Action.Type.PASSTHROUGH);
        DoActions(entered, Action.Type.ENTRY);

        config.atomicState.UnionWith(ExtractAtomic(entered));

#if SC_PROFILE_SINGLE
        stopwatch.Stop();
        Debug.Log(stopwatch.ElapsedMilliseconds);
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


    public Statechart.Mode GetMode()
    {
        return machine.GetMode();
    }
}
