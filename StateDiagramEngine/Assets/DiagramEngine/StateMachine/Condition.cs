using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    readonly string property;
    readonly bool invert = false;


    public Condition(string prop, bool inv)
    {
        property = prop;
        invert = inv;
    }


    public Condition(string expression)
    {
        property = expression;
    }


    public bool Evaluate(Snapshot snap)
    {
        return snap.GetProperty(property) ^ invert;
    }


    public bool isEmpty()
    {
        return property == null || property == "";
    }
}