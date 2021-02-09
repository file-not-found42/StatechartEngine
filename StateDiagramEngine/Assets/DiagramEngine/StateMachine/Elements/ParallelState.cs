using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelState : State
{
    public readonly List<CompoundState> regions = new List<CompoundState>();


    public ParallelState(string n) : base(n) { }


    public override bool TryEnter(Path path, Snapshot snap)
    {
        foreach (CompoundState r in regions)
            r.TryEnter(path, snap);
        return true;
    }
}
