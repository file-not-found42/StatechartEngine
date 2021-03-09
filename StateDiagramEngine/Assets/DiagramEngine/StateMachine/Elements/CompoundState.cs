using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompoundState : State
{
    public Node entryChild = null;


    public CompoundState(string name, State parent) : base(name, parent) { }

    public override (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) TryEnter(Snapshot snap)
    {
        return entryChild.TryEnter(snap);
    }
}
