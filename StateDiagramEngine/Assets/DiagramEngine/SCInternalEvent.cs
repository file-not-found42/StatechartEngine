using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCInternalEvent
{
    public readonly string Type;


    public SCInternalEvent(string newType)
    {
        Type = newType;
    }


    public override bool Equals(object other)
    {
        if (other is SCInternalEvent @event)
            return Type == @event.Type;
        else
            return false;
    }


    public static bool operator ==(SCInternalEvent a, SCInternalEvent b)
    {
        return a.Equals(b);
    }


    public static bool operator !=(SCInternalEvent a, SCInternalEvent b)
    {
        return !a.Equals(b);
    }


    public override int GetHashCode()
    {
        return Type.GetHashCode();
    }
}
