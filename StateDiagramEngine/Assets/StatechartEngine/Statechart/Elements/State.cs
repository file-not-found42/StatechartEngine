using System.Collections.Generic;

public abstract class State : Node, System.IComparable<State>
{
    public State(string name, State superstate) : base(name, superstate) { }


    public (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) TryExit(Status snap)
    {
        if (superstate != null)
        {
            var next = superstate.TryExit(snap);
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
    public List<State> GetSuperstates(in State limit)
    {
        if (this == limit)
            return new List<State> { };
        else if (superstate == null)
            return new List<State> { this };
        else
        {
            var result = superstate.GetSuperstates(limit);
            result.Add(this);
            return result;
        }
    }


    public State GetCommonSuperstate(IEnumerable<State> others)
    {
        var superstates = GetSuperstates(null);
        var scope = superstate;

        foreach (var s in others)
        {
            for (int i = superstates.Count - 1; i >= 0; i--)
                if (s.superstate == superstates[i])
                {
                    scope = superstates[i];
                    break;
                }
                else
                    superstates.RemoveAt(i);
        }

        return scope;
    }


    public int CompareTo(State other)
    {
        if (other.IsIn(this))
            return -1;
        else if (IsIn(other))
            return 1;
        else
            return 0;
    }
}
