using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomicState : State
{
    public AtomicState(string n) : base(n) { }


    public override bool TryEnter(Path path, Snapshot snap)
    {
        path.AddDestination(this);
        return true;
    }
}
