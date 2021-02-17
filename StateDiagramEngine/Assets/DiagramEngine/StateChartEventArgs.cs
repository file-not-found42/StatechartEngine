using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChartEventArgs
{
    public string Source { get; private set; }
    public Action.Type Type { get; private set; }


    public StateChartEventArgs(string source, Action.Type type)
    {
        Source = source;
        Type = type;
    }
}
