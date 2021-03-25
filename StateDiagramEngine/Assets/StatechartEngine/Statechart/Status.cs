using System.Collections.Generic;

public class Status
{
    public readonly HashSet<SCEvent> events;
    public readonly Dictionary<string, bool> properties;

    public Status(Dictionary<string, bool> ps, HashSet<SCEvent> es)
    {
        properties = new Dictionary<string, bool>(ps);
        events = new HashSet<SCEvent>(es);
    }


    public bool GetProperty(string name)
    {
        properties.TryGetValue(name, out bool value);
        return value;
    }


    public bool ContainsEvent(SCEvent ev)
    {
        return events.Contains(ev);
    }
}
