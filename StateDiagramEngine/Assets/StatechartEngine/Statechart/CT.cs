using System.Collections.Generic;

public class CT : System.IComparable<CT>
{
    readonly AtomicState source;
    readonly ISet<AtomicState> destinations;
    readonly ISet<ISCElement> waypoints;

    State scope;
    HashSet<State> entered;
    HashSet<State> exited;

    public CT(AtomicState s, ISet<ISCElement> waypoints, ISet<AtomicState> destinations)
    {
        source = s;
        this.waypoints = waypoints;
        this.destinations = destinations;
    }


    public ISet<State> GetEntered()
    {
        if (entered == null)
        {
            entered = new HashSet<State>();
            foreach (var d in destinations)
                entered.UnionWith(d.GetSuperstates(GetScope()));
        }

        return entered;
    }


    public ISet<State> GetExited()
    {
        if (exited == null)
            exited = new HashSet<State>(source.GetSuperstates(GetScope()));

        return exited;
    }


    public int CompareTo(CT y)
    {
        var ances = GetScope().GetSuperstates(null);
        var ancesY = y.GetScope().GetSuperstates(null);

        if (ances.Contains(y.GetScope()))
            return -1;
        else if (ancesY.Contains(GetScope()))
            return 1;
        else
            return 0;
    }


    public State GetScope()
    {
        if (scope == null)
            scope = source.GetCommonSuperstate(destinations);

        return scope;
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
