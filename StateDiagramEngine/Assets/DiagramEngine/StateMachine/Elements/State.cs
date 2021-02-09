using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : Node
{
    public State(string n) : base(n) {}


    public override bool TryExit(Path path, Snapshot snap)
    {
        if (parent != null && parent.TryExit(path, snap))
            return true;

        foreach(Transition t in outTransitions.Values)
            if (t.Through(path, snap))
                return true;

        return false;
    }


    public List<State> GetAncestors(State limit)
    {
        if (this == limit)
            return new List<State> { };
        else if (parent == null)
            return new List<State> { this };
        else
        {
            var result = parent.GetAncestors(limit);
            result.Add(this);
            return result;
        }
    }


    public State GetCommonAncestor(ICollection<State> others)
    {
        var ancestry = GetAncestors(null);
        var scope = parent;

        foreach (var s in others)
        {
            for (int i = ancestry.Count - 1; i >= 0; i--)
                if (s.parent == ancestry[i])
                {
                    scope = ancestry[i];
                    break;
                }
                else
                    ancestry.RemoveAt(i);
        }

        return scope;
    }
}
