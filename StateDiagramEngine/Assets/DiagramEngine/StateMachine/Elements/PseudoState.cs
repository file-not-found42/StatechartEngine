using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoState : Node
{
    public PseudoState(string n) : base(n) { }


    public override bool TryEnter(Path path, Snapshot snap)
    {
        return TryExit(path, snap);
    }


    public override bool TryExit(Path path, Snapshot snap)
    {
        foreach (Transition t in outTransitions.Values)
            if (t.Through(path, snap))
            {
                path.AddWaymark(this);
                return true;
            }
        return false;
    }
}
