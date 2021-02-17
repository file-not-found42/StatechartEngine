using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelState : State
{
    public readonly List<State> regions = new List<State>();


    public ParallelState(string n) : base(n) { }


    public override List<AtomicState> Enter()
    {
        var result = new List<AtomicState>(regions.Count);

        foreach (var r in regions)
            result.AddRange(r.Enter());
        
        return result;
    }
}
