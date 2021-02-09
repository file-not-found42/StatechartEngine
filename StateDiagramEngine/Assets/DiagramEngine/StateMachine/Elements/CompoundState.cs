using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompoundState : State
{
    public State entryChild = null;


    public CompoundState(string n) : base(n) { }


    public override bool TryEnter(Path path, Snapshot snap)
    {
        entryChild.TryEnter(path, snap);
        return true;
    }
}
