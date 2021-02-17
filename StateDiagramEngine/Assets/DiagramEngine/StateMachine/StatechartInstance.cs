using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StatechartInstance : MonoBehaviour
{
    readonly HashSet<SCEvent> events = new HashSet<SCEvent>();
    readonly Dictionary<string, bool> properties = new Dictionary<string, bool>();
    readonly Dictionary<Action, ActionDelegate> actions = new Dictionary<Action, ActionDelegate>();

    [SerializeField]
    Statechart machine;

    Configuration config;


    void Awake()
    {
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
            if (exited.Contains(p.GetSource()))
                continue;

            valid_paths.Add(p);
            entered.UnionWith(p.GetEntered());
            exited.UnionWith(p.GetExited());
        }

        // Remove exited nodes from config
        config.atomicState.ExceptWith(ExtractAtomic(exited));

        // Execute EXIT actions
        DoActions(exited, Action.Type.EXIT);
        // Execute STAY Actions
        // TODO
        DoActions(exited, Action.Type.STAY);
        // Execute Passthrough
        foreach (var p in valid_paths)
            DoActions(p.GetWaymarks(), Action.Type.PASSTHROUGH);
        // Execute ENTRY Actions
        DoActions(exited, Action.Type.ENTRY);

        // Add entered nodes to config
        config.atomicState.UnionWith(ExtractAtomic(entered));

        Debug.Log(this + " is now in " + config.ToString());
    }


    void DoActions(IEnumerable<ISCElement> objects, Action.Type type)
    {
        foreach(var o in objects)
            if(actions.TryGetValue(new Action(o.ToString(), type), out ActionDelegate act))
                act?.Invoke();
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

        if (machine.GetMode() == Statechart.Mode.On_Event)
            Step();
    }


    public void Subscribe(Action key, ActionDelegate f)
    {
        if (actions.ContainsKey(key))
            actions[key] += f;
        else
            actions[key] = f;
    }


    public void Unsubscribe(Action key, ActionDelegate f)
    {
        if (actions.ContainsKey(key))
            actions[key] -= f;
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
