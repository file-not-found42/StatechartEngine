using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : System.IComparable<Path>
{
    readonly AtomicState source;
    
    readonly ISet<AtomicState> destinations;
    readonly ISet<ISCElement> waypoints;


    public Path(AtomicState s, ISet<ISCElement> waypoints, ISet<AtomicState> destinations)
    {
        source = s;
        this.waypoints = waypoints;
        this.destinations = destinations;
    }


    public ISet<State> GetEntered()
    {
        var scope = source.GetCommonAncestor(destinations);
        var entered = new HashSet<State>();

        foreach (var d in destinations)
            entered.UnionWith(d.GetAncestors(scope));

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


    public ICollection<ISCElement> GetWaypoints()
    {
        return waypoints;
    }
}
