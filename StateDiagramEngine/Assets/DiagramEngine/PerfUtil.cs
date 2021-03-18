using System.Diagnostics;
using System.Text;

public class AccumulatedTime
{
    public enum TimeUnit
    {
        µs,
        ms,
        s
    }

    long sum = 0;
    long count = 0;
    long min = long.MaxValue;
    long max = long.MinValue;


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
        count++;
    }


    public string GetStatisitics(TimeUnit unit)
    {
        var builder = new StringBuilder();

        builder.Append("Count: ");
        builder.Append(count);
        builder.Append("\t Average: ");
        builder.Append(ToUnit(sum, unit) / count);
        builder.Append("\t Minimum: ");
        builder.Append(ToUnit(min, unit));
        builder.Append("\t Maximum: ");
        builder.Append(ToUnit(max, unit));

        return builder.ToString();
    }


    static double ToUnit(long ticks, TimeUnit unit)
    {
        return unit switch
        {
            TimeUnit.µs => ticks / (double)Stopwatch.Frequency * 1e6,
            TimeUnit.ms => ticks / (double)Stopwatch.Frequency * 1e3,
            TimeUnit.s => ticks / (double)Stopwatch.Frequency,
            _ => 0,
        };
    }
}


public class PerfUtil
{
    public static double ExtractSec(Stopwatch watch)
    {
        return watch.ElapsedTicks / (double)Stopwatch.Frequency;
    }
}
