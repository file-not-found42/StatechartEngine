using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomicState : State
{
    public AtomicState(string name, State parent) : base(name, parent) { }

    public override (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) TryEnter(Snapshot snap)
    {
        return (new HashSet<AtomicState>() { this }, new HashSet<ISCElement>());
    }
}
