using System.Diagnostics;
using System.Text;

public class SampleBuffer
{
    private readonly double[] values;
    private long pos = 0;

    public bool Filled 
    { get; private set; } = false;
    public long Count
    { get { return Filled ? values.Length : pos; } }

    public SampleBuffer(long count)
    {
        values = new double[count];
    }

    public void Insert(double value)
    {
        values[pos] = value;
        pos++;
        if (pos == values.Length)
        {
            pos = 0;
            Filled = true;
        }
    }

    public (double mean, double median, double min, double max) GetStatistics()
    {
        var workingcopy = values.Clone() as double[];
        System.Array.Sort(workingcopy);

        var min = workingcopy[0];

        var max = workingcopy[Count - 1];
        
        var mean = 0.0;
        foreach (var v in workingcopy)
            mean += v;
        mean /= Count;

        double median;
        if (Count % 2 == 0)
            median = (workingcopy[Count / 2] + workingcopy[Count / 2 + 1]) * 0.5;
        else
            median = workingcopy[Count / 2];

        return (mean, median, min, max);
    }
}


public class AccumulatedTime
{
    public enum TimeUnit
    {
        탎,
        ms,
        s
    }

    long sum = 0;
    long count = 0;
    long min = long.MaxValue;
    long max = long.MinValue;
    readonly SampleBuffer samples = new SampleBuffer(5000);


    public long SampleCount
    { get { return count; } }


    public void Accumulate(Stopwatch watch)
    {
        long elapsed = watch.ElapsedTicks;
        
        if (elapsed < min)
            min = elapsed;
        else if (elapsed > max)
            max = elapsed;
       
        sum += elapsed;
        samples.Insert(elapsed / (Stopwatch.Frequency * 1e-6));
        count++;
    }


    public string GetStatistics(TimeUnit unit)
    {
        var builder = new StringBuilder();

        builder.Append("Count: ");
        builder.Append(count);
       
        builder.Append("\n    Total:     Mean ");
        builder.Append(string.Format("{0:0.0}", ToUnit(sum, unit) / count));
        builder.Append(" 탎,    Min ");
        builder.Append(string.Format("{0:0.0}", ToUnit(min, unit)));
        builder.Append(" 탎,    Max ");
        builder.Append(string.Format("{0:0.0}", ToUnit(max, unit)));
        builder.Append(" 탎");

        if (samples.Filled)
        {
            var stats = samples.GetStatistics();

            builder.Append("\n    Last 5000: Mean ");
            builder.Append(string.Format("{0:0.0}", stats.mean));
            builder.Append(" 탎,    Min ");
            builder.Append(string.Format("{0:0.0}", stats.min));
            builder.Append(" 탎,    Max ");
            builder.Append(string.Format("{0:0.0}", stats.max));
            builder.Append(" 탎,    Median ");
            builder.Append(string.Format("{0:0.0}", stats.median));
            builder.Append(" 탎");
        }

        return builder.ToString();
    }


    static double ToUnit(long ticks, TimeUnit unit)
    {
        return unit switch
        {
            TimeUnit.탎 => ticks / (double)Stopwatch.Frequency * 1e6,
            TimeUnit.ms => ticks / (double)Stopwatch.Frequency * 1e3,
            TimeUnit.s => ticks / (double)Stopwatch.Frequency,
            _ => 0,
        };
    }
}


public class Utility
{
    public static double ExtractSec(Stopwatch watch)
    {
        return watch.ElapsedTicks / (double)Stopwatch.Frequency;
    }
}
