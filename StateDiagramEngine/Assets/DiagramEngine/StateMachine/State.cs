using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public string entryAction;
    public string stayAction;
    public string exitAction;

    // Sorted by priority
    public List<Transition> outTransitions = new List<Transition>();

    public readonly string name;
    public State parent = null;
    public State def = null;


    public State(string n)
    {
        name = n;
    }


    public Transition Step(StatechartInstance instance, List<SCEvent> events)
    {
        // Higher level states have higher priority
        Transition next = parent?.Step(instance, events);

        if (next != null)
            return next;
        
        foreach (Transition trans in outTransitions)
        {
            if (trans.Step(instance, events))
                return trans;
        }

        return null;
    }


    public void Entry(StatechartInstance instance)
    {
        parent?.Entry(instance);
        if (entryAction != null)
            instance.DoActivity(entryAction);
    }


    public void Stay(StatechartInstance instance)
    {
        parent?.Stay(instance);
        if (stayAction != null)
            instance.DoActivity(stayAction);
    }


    public void Exit(StatechartInstance instance)
    {
        parent?.Exit(instance);
        if (exitAction != null)
            instance.DoActivity(exitAction);
    }


    public override string ToString()
    {
        return parent == null ? name : parent.ToString() + name;
    }
}
