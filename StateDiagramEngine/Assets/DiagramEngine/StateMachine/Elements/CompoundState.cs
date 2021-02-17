using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompoundState : State
{
    public State entryChild = null;


    public CompoundState(string n) : base(n) { }


    public override List<AtomicState> Enter()
    {
        return entryChild.Enter();
    }
}
