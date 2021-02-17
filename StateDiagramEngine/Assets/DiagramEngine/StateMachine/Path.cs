using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : System.IComparable<Path>
{
    readonly AtomicState source;
    readonly ISet<AtomicState> destinations = new HashSet<AtomicState>();
    readonly ICollection<ISCElement> waymarks = new List<ISCElement>();


    public Path(AtomicState s)
    {
        source = s;
    }


    public ISet<State> GetEntered()
    {
        var scope = source.GetCommonAncestor(destinations);
        var entered = new HashSet<State>();

        foreach (var d in destinations)
            foreach (var s in d.GetAncestors(scope))
                entered.Add(s);

        return entered;
    }


    public ISet<State> GetExited()
    {
        var scope = source.GetCommonAncestor(destinations);

        return new HashSet<State>(source.GetAncestors(scope));
    }


    public int CompareTo(Path y)
    {
        var scope = source.GetCommonAncestor(destinations);
        var scopeY = y.source.GetCommonAncestor(y.destinations);

        var ances = scope.GetAncestors(null);
        var ancesY = scopeY.GetAncestors(null);

        if (ances.Contains(scopeY))
            return -1;
        else if (ancesY.Contains(scope))
            return 1;
        else
            return 0;
    }


    public AtomicState GetSource()
    {
        return source;
    }


    public void AddWaymark(ISCElement transition)
    {
        waymarks.Add(transition);
    }


    public ICollection<ISCElement> GetWaymarks()
    {
        return waymarks;
    }


    public void AddDestination(AtomicState state)
    {
        destinations.Add(state);
    }


    public ISet<AtomicState> GetDestinations()
    {
        return destinations;
    }
}
