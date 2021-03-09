using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : ISCElement
{
    public string name;
    public SCInternalEvent trigger = null;
    public Guard guard;

    public readonly Node destination;


    public Transition(string n, Node dest)
    {
        name = n;
        destination = dest;
    }


    public (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) TryThrough(Snapshot snap)
    {
        bool active = (guard == null || guard.Evaluate(snap))
            && (trigger == null || snap.ContainsEvent(trigger));

        if (!active)
            return (null, null);

        var next = destination.TryEnter(snap);

        if (next == (null, null))
            return (null, null);

        next.waypoints.Add(this);

        return next;
    }


    public override string ToString()
    {
        return name;
    }
}
