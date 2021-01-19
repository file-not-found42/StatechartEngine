using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pseudostate
{
    public List<Transition> outTransitions = new List<Transition>();


    //public LinkedList<Transition> Step(StatechartInstance instance, List<SCEvent> events)
    //{
    //    foreach (Transition trans in outTransitions)
    //    {
    //        return trans.Step(instance, events).Add(this);
    //    }
    //}
}
