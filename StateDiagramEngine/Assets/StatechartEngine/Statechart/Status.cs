using System.Collections.Generic;

public class Status
{
    enum Valid
    {
        Active,
        Inactive,
        Error
    }

    public readonly Statechart statechart;
    public readonly HashSet<AtomicState> b_configuration;
    public readonly HashSet<SCEvent> events;
    public readonly Dictionary<string, bool> properties;


    public Status(Statechart statechart, HashSet<AtomicState> b_configuration, Dictionary<string, bool> properties, HashSet<SCEvent> events)
    {
        this.statechart = statechart;
        this.b_configuration = b_configuration;
        this.properties = properties;
        this.events = events;
    }


    public Status(Status other)
    {
        statechart = other.statechart;
        b_configuration = new HashSet<AtomicState>(other.b_configuration);
        properties = new Dictionary<string, bool>(other.properties);
        events = new HashSet<SCEvent>(other.events);
    }


    public bool IsValid()
    {
        return IsValidInternal(statechart.Root) == Valid.Active;
    }


    private Valid IsValidInternal(State subtree)
    {
        if (subtree is AtomicState atomic)
        {
            return b_configuration.Contains(atomic) ? Valid.Active : Valid.Inactive;
        }
        else if (subtree is CompoundState compound)
        {
            long count = 0;
            foreach (var c in compound.components)
                switch (IsValidInternal(c))
                {
                    case Valid.Active:
                        count++;
                        break;
                    case Valid.Error:
                        return Valid.Error;
                }

            if (count == 0)
                return Valid.Inactive;
            else if (count == 1)
                return Valid.Active;
            else
                return Valid.Error;
        }
        else if (subtree is ParallelState parallel)
        {
            long count = 0;
            foreach (var r in parallel.components)
                switch (IsValidInternal(r))
                {
                    case Valid.Active:
                        count++;
                        break;
                    case Valid.Error:
                        return Valid.Error;
                }

            if (count == 0)
                return Valid.Inactive;
            else if (count == parallel.components.Count)
                return Valid.Active;
            else
                return Valid.Error;
        }

        return Valid.Error;
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


    public override string ToString()
    {
        var sb = new System.Text.StringBuilder(statechart.ToString());
        sb.Append("::Config(");
        foreach (var s in b_configuration)
        {
            sb.Append(s.ToString());
            sb.Append(", ");
        }
        sb.Append(");Events(");
        foreach (var s in events)
        {
            sb.Append(s.ToString());
            sb.Append(", ");
        }
        sb.Append(");Properties{");
        foreach (var s in properties)
        {
            sb.Append(s.ToString());
            sb.Append(", ");
        }
        sb.Append("}");
        return sb.ToString();
    }
}
