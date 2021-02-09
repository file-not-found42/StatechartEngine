using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition
{
    public string name;
    public Condition cond;
    public SCEvent trigger = null;

    public readonly Node destination;


    public Transition(string n, Node dest)
    {
        name = n;
        destination = dest;
    }


    public bool Through(Path path, Snapshot snap)
    {
        bool active = EvaluateCondition(snap) 
            && (trigger == null || snap.ContainsEvent(trigger));

        if (active && destination.TryEnter(path, snap))
        {
            path.AddTransition(this);
            return true;
        }

        return false;
    }


    bool EvaluateCondition(Snapshot snap)
    {
        if (cond == null)
            return true;

        return snap.GetProperty(cond.property) ^ cond.invert;
    }
}
