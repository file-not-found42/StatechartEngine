using System.Collections;
using System.Collections.Generic;

public class Configuration
{
    public readonly HashSet<AtomicState> atomicState = null;


    public Configuration()
    {
        atomicState = new HashSet<AtomicState>();
    }


    public Configuration(Configuration other)
    {
        atomicState = new HashSet<AtomicState>(other.atomicState);
    }


    public override string ToString()
    {
        var sb = new System.Text.StringBuilder("[");
        
        foreach (var s in atomicState)
        {
            sb.Append(s.ToString());
            sb.Append(", ");
        }

        sb.Append("]");
        return sb.ToString();
    }
}
