using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StatechartInstance : MonoBehaviour
{
    const int minCapacity = 16;
    readonly Dictionary<Action, EventHandler<ActionArgs>> actions = new Dictionary<Action, EventHandler<ActionArgs>>();

    Status status;
    
    [SerializeField]
    Statechart machine;

    readonly List<CT> CTs = new List<CT>(minCapacity);
    readonly HashSet<int> entered = new HashSet<int>();
    readonly HashSet<int> exited = new HashSet<int>();
    readonly HashSet<int> active = new HashSet<int>();
    HashSet<int> generic = new HashSet<int>();

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

        CTs.Clear();
        entered.Clear();
        exited.Clear();
        active.Clear();
        foreach (var s in status.b_configuration)
            active.UnionWith(machine.GetSuperstates(s));

#if SC_PROFILE_SINGLE
        stopwatch.Stop();
        prepare.Accumulate(stopwatch);
        stopwatch.Reset();
        stopwatch.Start();
#endif
        // Search Paths

        foreach (var s in status.b_configuration)
        {   
            var next = machine.TryExit(s, status);
            if (next != (null, null))
                CTs.Add(new CT(machine, s, next.waypoints, next.destinations));
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
            if (active.IsSupersetOf(machine.GetSuperstates(CTs[i].GetSource())))
            {
                generic.Clear();
                CTs[i].GetExited(ref generic);
                active.ExceptWith(generic);
                exited.UnionWith(generic);

                generic.Clear();
                CTs[i].GetEntered(ref generic);
                entered.UnionWith(generic);
            }
            else
                CTs[i] = null;
        }
        // Remove any so far untouched parallel regions which have been implicitly exited
        generic.Clear();
        foreach (var act in active)
        {
            var ancestors = machine.GetSuperstates(act);
            ancestors.Reverse();
            foreach (var a in ancestors)
                if (!active.Contains(a))
                {
                    generic.UnionWith(machine.GetSuperstates(act, a));
                    break;
                }
        }

        active.ExceptWith(generic);
        exited.UnionWith(generic);
        // Implicitly enter all so far untouched parallel regions
        generic.Clear();
        foreach (var e in entered)
            if (machine.GetNodeType(e) == CompactNode.Type.Parallel)
                generic.UnionWith(machine.GetNodeComponents(e));

        generic.ExceptWith(active);
        generic.ExceptWith(entered);

        foreach (var r in generic)
        {
            var next = machine.TryEnter(r, status);
            if (next != (null, null))
            {
                entered.UnionWith(next.destinations);
            }
            // else: ERROR (Limitation)
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
                DoActions(p.GetWaypoints());
        DoActions(entered, Action.Type.ENTRY);
        // Clear and add events for next step
        status.events.Clear();
        foreach (var s in exited)
            status.events.Add(new SCEvent("exit." + machine.GetNodeName(s)));
        foreach (var s in entered)
            status.events.Add(new SCEvent("enter." + machine.GetNodeName(s)));

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
        StringBuilder builder = new StringBuilder(this.ToString());
        builder.Append(" is now in\n");
        builder.Append(status.ToString());
        builder.Append("\nafter executing\n");
        foreach (var ct in CTs)
            if (ct != null)
            {
                builder.Append(ct);
                builder.Append("\n\t");
            }
        Debug.Log(builder.ToString());
#endif

#if SC_DEBUG
        if (!machine.IsValid(status))
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


    void DoActions(IEnumerable<long> waypoints)
    {
        foreach(var o in waypoints)
        {
            var action = new Action(o, Action.Type.PASSTHROUGH);
            if (actions.TryGetValue(action, out EventHandler<ActionArgs> act))
                act?.Invoke(this, new ActionArgs(o.ToString(), Action.Type.PASSTHROUGH));
        }
    }


    void DoActions(IEnumerable<int> nodes, Action.Type type)
    {
        foreach (var o in nodes)
        {
            var action = new Action(o, type);
            if (actions.TryGetValue(action, out EventHandler<ActionArgs> act))
                act?.Invoke(this, new ActionArgs(o.ToString(), type));
        }
    }


    HashSet<int> ExtractAtomic(ISet<int> source)
    {
        var result = new HashSet<int>();

        foreach (var s in source)
            if (machine.GetNodeType(s) == CompactNode.Type.Basic)
                result.Add(s);

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
        var key = new Action(machine.GetElementByName(source), type);
        if (actions.ContainsKey(key))
            actions[key] += function;
        else
            actions[key] = function;
    }


    public void Unsubscribe(string source, Action.Type type, EventHandler<ActionArgs> function)
    {
        var key = new Action(machine.GetElementByName(source), type);
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
        int state = machine.GetNodeByName(name);

        foreach (var s in status.b_configuration)
            foreach (var a in machine.GetSuperstates(s))
                if (a == state)
                    return true;

        return false;
    }


    public Statechart.Mode GetMode()
    {
        return machine.GetMode();
    }
}
