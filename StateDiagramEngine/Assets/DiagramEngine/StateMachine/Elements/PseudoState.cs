using System.Collections.Generic;

public class PseudoState : Node
{
    public PseudoState(string name, State parent) : base(name, parent) { }

    public override (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) TryEnter(Snapshot snap)
    {
        foreach (var t in outTransitions.Values)
        {
            var next = t.TryThrough(snap);
            if (next != (null, null))
            {
                next.waypoints.Add(this);
                return next;
            }
        }
        
        return (null, null);
    }
}
