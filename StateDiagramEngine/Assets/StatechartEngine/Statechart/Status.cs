using System.Collections.Generic;

public class Status
{
    public readonly Statechart statechart;
    public readonly HashSet<int> b_configuration;
    public readonly HashSet<SCEvent> events;
    public readonly List<bool> properties;


    public Status(Statechart statechart, HashSet<int> b_configuration, List<bool> properties)
    {
        this.statechart = statechart;
        this.b_configuration = b_configuration;
        this.properties = properties;
        
        events = new HashSet<SCEvent>();
    }


    public Status(Status other)
    {
        statechart = other.statechart;
        b_configuration = new HashSet<int>(other.b_configuration);
        properties = new List<bool>(other.properties);
        events = new HashSet<SCEvent>(other.events);
    }


    public bool GetProperty(int property)
    {
        return properties[property];
    }


    public bool ContainsEvent(SCEvent ev)
    {
        return events.Contains(ev);
    }


    public override string ToString()
    {
        var sb = new System.Text.StringBuilder(statechart.ToString());
        sb.Append("::Config(");
        foreach (var s in b_configuration)
        {
            sb.Append(statechart.GetNodeName(s));
            sb.Append(", ");
        }
        sb.Append(");Events(");
        foreach (var s in events)
        {
            sb.Append(s.ToString());
            sb.Append(", ");
        }
        sb.Append(");Properties{");
        sb.Append(statechart.PropertiesToString(this));
        sb.Append("}");
        return sb.ToString();
    }
}
