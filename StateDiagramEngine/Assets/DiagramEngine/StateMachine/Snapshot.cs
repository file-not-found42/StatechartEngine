using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snapshot
{
    public readonly Configuration config;
    public readonly HashSet<SCEvent> events;
    public readonly Dictionary<string, bool> properties;

    public Snapshot(Configuration conf, Dictionary<string, bool> ps, HashSet<SCEvent> es)
    {
        config = new Configuration(conf);
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
