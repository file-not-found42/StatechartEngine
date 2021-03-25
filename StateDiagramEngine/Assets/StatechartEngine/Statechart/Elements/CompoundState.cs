using System.Collections.Generic;

public class CompoundState : State
{
    public Node defaultComponent = null;
    public List<State> components = new List<State>();


    public CompoundState(string name, State superstate) : base(name, superstate) { }


    public override (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) TryEnter(Status snap)
    {
        return defaultComponent.TryEnter(snap);
    }
}
