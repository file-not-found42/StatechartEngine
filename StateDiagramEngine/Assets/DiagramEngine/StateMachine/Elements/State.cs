using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : Node, System.IComparable<State>
{
    public State(string n) : base(n) {}


    public abstract List<AtomicState> Enter();


    public override bool TryEnter(Path path, Snapshot snap)
    {
        var states = Enter();

        foreach (var s in states)
        {
            path.AddDestination(s);
        }
        
        return true;
    }


    public override bool TryExit(Path path, Snapshot snap)
    {
        if (parent != null && parent.TryExit(path, snap))
            return true;

        foreach(Transition t in outTransitions.Values)
            if (t.Through(path, snap))
                return true;

        return false;
    }


    public List<State> GetAncestors(in State limit)
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


    public State GetCommonAncestor(IEnumerable<State> others)
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

    public int CompareTo(State other)
    {
        if (other.IsChildOf(this))
            return -1;
        else if (IsChildOf(other))
            return 1;
        else
            return 0;
    }
}
