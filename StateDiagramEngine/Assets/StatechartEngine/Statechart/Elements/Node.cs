using System.Collections.Generic;

public abstract class Node : ISCElement
{
    public readonly string name;
    public readonly State parent = null;

    // Sorted by priority
    public readonly SortedList<int, Transition> outTransitions = new SortedList<int, Transition>();


    public Node(string name, State parent)
    {
        this.name = name;
        this.parent = parent;
    }
    
    
    public abstract (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) TryEnter(Status snap);


    public bool IsChildOf(State state)
    {
        if (parent == null)
            return this == state;

        return this == state || parent.IsChildOf(state);
    }
    
    
    public override string ToString()
    {
        return parent == null ? name : parent.ToString() + "." + name;
    }
}
