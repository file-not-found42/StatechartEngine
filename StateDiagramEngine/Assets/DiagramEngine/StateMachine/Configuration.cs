using System.Collections;
using System.Collections.Generic;

public class Configuration
{
    enum Valid
    {
        Active,
        Inactive,
        Error
    }

    public readonly Statechart statechart = null;
    public readonly HashSet<AtomicState> activeStates = null;


    public Configuration(Statechart statechart, IEnumerable<AtomicState> states)
    {
        this.statechart = statechart;
        activeStates = new HashSet<AtomicState>(states);
    }


    public Configuration(Configuration other)
    {
        statechart = other.statechart;
        activeStates = new HashSet<AtomicState>(other.activeStates);
    }


    public bool IsValid()
    {
        return IsValidInternal(statechart.Root) == Valid.Active;
    }


    private Valid IsValidInternal(State subtree)
    {
        if (subtree is AtomicState atomic)
        {
            return activeStates.Contains(atomic) ? Valid.Active : Valid.Inactive;
        }
        else if (subtree is CompoundState compound)
        {
            long count = 0;
            foreach (var c in compound.children)
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
            foreach (var r in parallel.regions)
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
            else if (count == parallel.regions.Count)
                return Valid.Active;
            else
                return Valid.Error;
        }
        
        return Valid.Error;
    }


    public override string ToString()
    {
        var sb = new System.Text.StringBuilder("[");
        
        foreach (var s in activeStates)
        {
            sb.Append(s.ToString());
            sb.Append(", ");
        }

        sb.Append("]");
        return sb.ToString();
    }
}
