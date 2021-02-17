using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCEventArgs
{
    public string Source { get; private set; }
    public Action.Type Type { get; private set; }


    public SCEventArgs(string source, Action.Type type)
    {
        Source = source;
        Type = type;
    }
}
