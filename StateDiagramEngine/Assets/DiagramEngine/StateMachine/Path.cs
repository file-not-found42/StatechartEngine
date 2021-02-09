using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : System.IComparable<Path>
{
    readonly AtomicState source;
    readonly ISet<AtomicState> destinations = new HashSet<AtomicState>();
    readonly ICollection<Transition> transitions = new List<Transition>();


    public Path(AtomicState s)
    {
        source = s;
    }


    public ISet<State> GetEntered()
    {
        var scope = source.GetCommonAncestor((ICollection<State>)destinations);
        var entered = new HashSet<State>();

        foreach (var d in destinations)
            foreach (var s in d.GetAncestors(scope))
                entered.Add(s);

        return entered;
    }


    public ISet<State> GetExited()
    {
        var scope = source.GetCommonAncestor((ICollection<State>)destinations);

        return new HashSet<State>(source.GetAncestors(scope));
    }


    public int CompareTo(Path y)
    {
        var scope = source.GetCommonAncestor((ICollection<State>)destinations);
        var scopeY = y.source.GetCommonAncestor((ICollection<State>)y.destinations);

        var ances = scope.GetAncestors(null);
        var ancesY = scopeY.GetAncestors(null);

        if (ances.Contains(scopeY))
            return -1;
        else if (ancesY.Contains(scope))
            return 1;
        else
            return 0;
    }


    public AtomicState GetSource()
    {
        return source;
    }


    public void AddTransition(Transition transition)
    {
        transitions.Add(transition);
    }


    public ICollection<Transition> GetTransitions()
    {
        return transitions;
    }


    public void AddDestination(AtomicState state)
    {
        destinations.Add(state);
    }


    public ISet<AtomicState> GetDestinations()
    {
        return destinations;
    }
}
