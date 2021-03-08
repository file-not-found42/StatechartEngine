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
    { get { return (float)(1e3 * AverageSec); } }

    public long Average�S
    { get { return (long)(1e6 * AverageSec); } }

    public long SampleCount
    { get { return count; } }

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
