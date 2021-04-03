using System.Collections.Generic;

public abstract class Node : ISCElement
{
    public readonly string name;
    public readonly State superstate = null;

    // Sorted by priority
    public readonly SortedList<int, Transition> outTransitions = new SortedList<int, Transition>();

    readonly string displayName;


    public Node(string name, State superstate)
    {
        this.name = name;
        displayName = superstate == null ? name : superstate.ToString() + "." + name;
        this.superstate = superstate;
    }
    
    
    public abstract (ISet<AtomicState> destinations, ISet<ISCElement> waypoints) TryEnter(Status snap);


    public bool IsIn(State state)
    {
        if (superstate == null)
            return this == state;

        return this == state || superstate.IsIn(state);
    }
    
    
    public override string ToString()
    {
        return displayName;
    }
}
