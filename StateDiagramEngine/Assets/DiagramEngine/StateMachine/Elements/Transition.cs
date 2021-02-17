using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : ISCElement
{
    public string name;
    public Condition cond;
    public SCInternalEvent trigger = null;

    public readonly Node destination;


    public Transition(string n, Node dest)
    {
        name = n;
        destination = dest;
    }


    public bool Through(Path path, Snapshot snap)
    {
        bool active = (cond == null || cond.Evaluate(snap))
            && (trigger == null || snap.ContainsEvent(trigger));

        if (active && destination.TryEnter(path, snap))
        {
            path.AddWaymark(this);
            return true;
        }

        return false;
    }


    public override string ToString()
    {
        return name;
    }
}
