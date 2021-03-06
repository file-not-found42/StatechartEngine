using System.Collections.Generic;

public class CT : System.IComparable<CT>
{
    readonly Statechart statechart;
    readonly int source;
    readonly ISet<int> destinations;
    readonly ISet<long> waypoints;

    int scope = -1;

    public CT(Statechart statechart, int source, ISet<long> waypoints, ISet<int> destinations)
    {
        this.statechart = statechart;
        this.source = source;
        this.waypoints = waypoints;
        this.destinations = destinations;
    }


    public void GetEntered(ref HashSet<int> entered)
    {
        foreach (var d in destinations)
            entered.UnionWith(statechart.GetSuperstates(d, GetScope()));
    }


    public void GetExited(ref HashSet<int> exited)
    {
        exited.UnionWith(statechart.GetSuperstates(source, GetScope()));
    }


    public int CompareTo(CT y)
    {
        var ances = statechart.GetSuperstates(GetScope());
        var ancesY = statechart.GetSuperstates(y.GetScope());

        if (ances.Contains(y.GetScope()))
            return -1;
        else if (ancesY.Contains(GetScope()))
            return 1;
        else
            return 0;
    }


    public int GetScope()
    {
        if (scope == -1)
            scope = statechart.GetCommonSuperstate(new List<int>(destinations) { source });

        return scope;
    }


    public int GetSource()
    {
        return source;
    }


    public ICollection<long> GetWaypoints()
    {
        return waypoints;
    }


    public override string ToString()
    {
        var sb = new System.Text.StringBuilder(statechart.GetNodeName(source));
        sb.Append("--[");
        foreach (var s in waypoints)
        {
            sb.Append(statechart.GetElementName(s));
            sb.Append(", ");
        }
        sb.Append("]->(");
        foreach (var s in destinations)
        {
            sb.Append(statechart.GetNodeName(s));
            sb.Append(", ");
        }
        sb.Append(")");
        return sb.ToString();
    }
}
