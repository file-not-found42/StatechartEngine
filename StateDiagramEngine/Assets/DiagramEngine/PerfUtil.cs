using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AccumulatedTime
{
    long ticks = 0;
    long count = 0;
    
    public double TotalSec
    { get { return ticks / (double)Stopwatch.Frequency; } }

    public double AverageSec
    { get { return TotalSec / count; } }

    public float AverageMS
    { get { return 1000.0f * (float)AverageSec; } }


    public void Accumulate(Stopwatch watch)
    {
        ticks += watch.ElapsedTicks;
        count++;
    }
}


public class PerfUtil
{
    public static double ExtractSec(Stopwatch watch)
    {
        return watch.ElapsedTicks / (double)Stopwatch.Frequency;
    }
}
