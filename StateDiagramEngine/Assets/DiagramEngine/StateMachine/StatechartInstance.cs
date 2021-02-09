using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StatechartInstance : MonoBehaviour
{
    readonly HashSet<SCEvent> events = new HashSet<SCEvent>();
    readonly Dictionary<string, bool> properties = new Dictionary<string, bool>();

    readonly Dictionary<string, ActionDelegate> activities = new Dictionary<string, ActionDelegate>();

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

        ISet<State> entered = new HashSet<State>();
        ISet<State> exited = new HashSet<State>();

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

        // Execute EXIT actions
        DoActivities((ICollection<object>)exited, Action.Type.EXIT);
        // Execute STAY Actions
        DoActivities((ICollection<object>)exited, Action.Type.STAY);
        // Execute Passthrough
        foreach (var p in valid_paths)
            DoActivities((ICollection<object>)p.GetTransitions(), Action.Type.PASSTHROUGH);
        // Execute ENTRY Actions
        DoActivities((ICollection<object>)exited, Action.Type.ENTRY);

        // Remove exited nodes from config
        config.atomicState.ExceptWith((IEnumerable<AtomicState>)exited);
        // Add entered nodes to config
        config.atomicState.UnionWith((IEnumerable<AtomicState>)exited);

        Debug.Log(this + " is now in " + config);
    }


    public void DoActivities(ICollection<System.Object> objects, Action.Type type)
    {
        foreach(var o in objects)
            if(activities.TryGetValue(new Action(o.ToString(), type).ToString(), out ActionDelegate act))
                act?.Invoke();
    }

    
    public void AddEvent(SCEvent e)
    {
        events.Add(e);
    }


    public void Subscribe(Action key, ActionDelegate f)
    {
        if (activities.ContainsKey(key.ToString()))
            activities[key.ToString()] += f;
        else
            activities[key.ToString()] = f;
    }


    public void Unsubscribe(Action key, ActionDelegate f)
    {
        if (activities.ContainsKey(key.ToString()))
            activities[key.ToString()] -= f;
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
