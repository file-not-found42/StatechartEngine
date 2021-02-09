using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : System.IEquatable<Node>
{
    public readonly string name;
    public State parent = null;

    // Sorted by priority
    public readonly SortedList<int, Transition> outTransitions = new SortedList<int, Transition>();


    public Node(string n)
    {
        name = n;
    }


    public abstract bool TryExit(Path path, Snapshot snap);
    
    
    public abstract bool TryEnter(Path path, Snapshot snap);


    public override string ToString()
    {
        return parent == null ? name : parent.ToString() + "." + name;
    }


    public bool Equals(Node other)
    {
        return this.name == other.name;
    }
}
