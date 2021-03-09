using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snapshot
{
    public readonly HashSet<SCInternalEvent> events;
    public readonly Dictionary<string, bool> properties;

    public Snapshot(Dictionary<string, bool> ps, HashSet<SCInternalEvent> es)
    {
        properties = new Dictionary<string, bool>(ps);
        events = new HashSet<SCInternalEvent>(es);
    }


    public bool GetProperty(string name)
    {
        properties.TryGetValue(name, out bool value);
        return value;
    }


    public bool ContainsEvent(SCInternalEvent ev)
    {
        return events.Contains(ev);
    }
}
