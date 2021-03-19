using System;
using System.Collections.Generic;

public abstract class State : Node, System.IComparable<State>
{
    public State(string name, State parent) : base(name, parent) { }


    public (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) TryExit(Snapshot snap)
    {
        if (parent != null)
        {
            var next = parent.TryExit(snap);
            if (next != (null, null))
                return next;
        }

        foreach (var t in outTransitions.Values)
        {
            var next = t.TryThrough(snap);
            if (next != (null, null))
                return next;
        }

        return (null, null);
    }

    /// <summary>
    /// Computes the list of states which contain the state from top to bottom, up to a limit
    /// </summary>
    /// <param name="limit">The limit up to which the list will be computed</param>
    /// <returns>The list of states which contain the state</returns>
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
