using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition
{
    public Activity passthrough;
    public Condition cond;
    public SCEvent trigger = null;

    public readonly State destination;


    public Transition(State dest)
    {
        destination = dest;
    }


    public bool Step(StatechartInstance instance, List<SCEvent> events)
    {
        if (cond != null && !instance.CheckCondition(cond))
            return false;

        if (trigger == null)
            return true;

        foreach (SCEvent e in events)
            if (trigger.Type == e.Type)
                return true;

        return false;
    }


    public void Traverse(StatechartInstance instance)
    {
        if (passthrough != null)
            instance.DoActivity(passthrough);
    }
}
