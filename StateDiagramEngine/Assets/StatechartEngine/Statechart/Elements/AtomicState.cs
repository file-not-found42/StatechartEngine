using System.Collections.Generic;

public class AtomicState : State
{
    public AtomicState(string name, State parent) : base(name, parent) { }

    public override (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) TryEnter(Status snap)
    {
        return (new HashSet<AtomicState>() { this }, new HashSet<ISCElement>());
    }
}
