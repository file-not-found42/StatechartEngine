using System.Collections.Generic;

public class ParallelState : State
{
    public readonly List<State> regions = new List<State>();


    public ParallelState(string name, State parent) : base(name, parent) { }


    public override (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) TryEnter(Status snap)
    {
        (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) result = (new HashSet<AtomicState>(), new HashSet<ISCElement>());

        foreach (var r in regions)
        {
            var next = r.TryEnter(snap);

            if (next == (null, null))
                return (null, null);

            result.destinations.UnionWith(next.destinations);
            result.waypoints.UnionWith(next.waypoints);
        }

        return result;
    }
}
