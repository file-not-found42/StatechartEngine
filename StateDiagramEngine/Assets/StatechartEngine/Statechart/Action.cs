public readonly struct Action : System.IEquatable<Action>
{
    public enum Type
    {
        ENTRY,
        STAY,
        EXIT,
        PASSTHROUGH
    }

    public readonly Type type;
    public readonly long source;


    public Action(long source, Type type)
    {
        this.source = source;
        this.type = type;
    }

    
    public readonly override string ToString()
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


    public readonly bool Equals(Action other)
    {
        return source == other.source && type == other.type;
    }


    public readonly override bool Equals(object other)
    {
        if (other is Action action)
            return Equals(action);
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


    public readonly override int GetHashCode()
    {
        return (source.GetHashCode() << 2) + (int)type;
    }
}
