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
}

public delegate void ActionDelegate();
