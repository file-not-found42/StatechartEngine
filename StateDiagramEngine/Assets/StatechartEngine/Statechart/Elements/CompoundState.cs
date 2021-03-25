using System.Collections.Generic;

public class CompoundState : State
{
    public Node entryChild = null;
    public List<State> children = new List<State>();


    public CompoundState(string name, State parent) : base(name, parent) { }


    public override (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) TryEnter(Status snap)
    {
        return entryChild.TryEnter(snap);
    }
}
