using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    Activity entry;
    Activity stay;
    Activity exit;

    // Sorted by priority
    List<Transition> outTransitions = new List<Transition>();

    State parent = null;
    List<State> children = new List<State>();

    public State Forward(SMEvent e)
    {
        // Higher level states have higher priority
        State next = parent?.Forward(e);

        if (next != null)
            return next;
        
        foreach (Transition trans in outTransitions)
        {
            if (trans.Match(e))
                return trans.destination;
        }

        return null;
    }
}
