using System.Collections.Generic;

public class ParallelState : State
{
    public readonly List<State> components = new List<State>();


    public ParallelState(string name, State superstate) : base(name, superstate) { }


    public override (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) TryEnter(Status snap)
    {
        (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) result = (new HashSet<AtomicState>(), new HashSet<ISCElement>());

        foreach (var r in components)
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
