public readonly struct Node
{
    public enum Type
    {
        Error,
        Compound,
        Parallel,
        Basic,
        Pseudo
    }

    // The node type: basic, compound, parallel, pseudo
    public readonly Type type;
    // The index of the superstate
    public readonly int superstate;
    // The index of the first component in the relations array
    public readonly int components;
    // The index of the first transition in the transition array
    public readonly int transitions;
    // Either the number of components after the parallel in the relations array
    // or the default component for a compound state
    public readonly int data;


    public Node(Type type, int superstate, int components, int data, int transitions)
    {
        this.type = type;
        this.superstate = superstate;
        this.components = components;
        this.data = data;
        this.transitions = transitions;
    }
}


public readonly struct Transition
{
    public readonly SCEvent trigger;
    public readonly Guard guard;
    // The index of the destination node in the node array
    public readonly int destination;


    public Transition(SCEvent trigger, Guard guard, int destination)
    {
        this.trigger = trigger;
        this.guard = guard;
        this.destination = destination;
    }
}
