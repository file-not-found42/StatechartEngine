using System.Collections.Generic;

public class CT : System.IComparable<CT>
{
    readonly AtomicState source;
    readonly ISet<AtomicState> destinations;
    readonly ISet<ISCElement> waypoints;


    public CT(AtomicState s, ISet<ISCElement> waypoints, ISet<AtomicState> destinations)
    {
        source = s;
        this.waypoints = waypoints;
        this.destinations = destinations;
    }


    public ISet<State> GetEntered()
    {
        var scope = source.GetCommonSuperstate(destinations);
        var entered = new HashSet<State>();

        foreach (var d in destinations)
            entered.UnionWith(d.GetSuperstates(scope));

        return entered;
    }


    public ISet<State> GetExited()
    {
        var scope = source.GetCommonSuperstate(destinations);

        return new HashSet<State>(source.GetSuperstates(scope));
    }


    public int CompareTo(CT y)
    {
        var scope = source.GetCommonSuperstate(destinations);
        var scopeY = y.source.GetCommonSuperstate(y.destinations);

        var ances = scope.GetSuperstates(null);
        var ancesY = scopeY.GetSuperstates(null);

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


    public override string ToString()
    {
        var sb = new System.Text.StringBuilder(source.ToString());
        sb.Append("--[");
        foreach (var s in waypoints)
        {
            sb.Append(s.ToString());
            sb.Append(", ");
        }
        sb.Append("]->(");
        foreach (var s in destinations)
        {
            sb.Append(s.ToString());
            sb.Append(", ");
        }
        sb.Append(")");
        return sb.ToString();
    }
}
