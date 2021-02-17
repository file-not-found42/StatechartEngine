using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ProfilingUtility
{
    public static float PrintStopwatchMilliseconds(Stopwatch watch)
    {
        return watch.ElapsedTicks / (Stopwatch.Frequency * 0.001f);
    }
}
