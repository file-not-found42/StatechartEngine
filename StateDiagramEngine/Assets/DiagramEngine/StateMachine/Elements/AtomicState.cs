using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomicState : State
{
    public AtomicState(string n) : base(n) { }


    public override List<AtomicState> Enter()
    {
        return new List<AtomicState>() { this };
    }
}
