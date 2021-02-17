using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public enum Type
    {
        ENTRY,
        STAY,
        EXIT,
        PASSTHROUGH
    }

    public readonly Type type;
    public readonly string source;


    public Action(string source, Type type)
    {
        this.source = source;
        this.type = type;
    }

    
    public override string ToString()
    {
        return type switch
        {
            Type.ENTRY          => source + ":ENTRY",
            Type.STAY           => source + ":STAY",
            Type.EXIT           => source + ":EXIT",
            Type.PASSTHROUGH    => source + ":PASSTHROUGH",
            _                   => source + ":ERROR",
        };
    }


    public override bool Equals(object other)
    {
        if (other is Action action)
            return source == action.source && type == action.type;
        else
            return false;
    }


    public static bool operator ==(Action a, Action b)
    {
        return a.Equals(b);
    }


    public static bool operator !=(Action a, Action b)
    {
        return !a.Equals(b);
    }


    public override int GetHashCode()
    {
        return (source.GetHashCode() << 2) + (int)type;
    }
}

public delegate void ActionDelegate();
