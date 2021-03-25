using System.Collections.Generic;

public class AtomicState : State
{
    public AtomicState(string name, State superstate) : base(name, superstate) { }

    public override (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) TryEnter(Status snap)
    {
        return (new HashSet<AtomicState>() { this }, new HashSet<ISCElement>());
    }
}
