public readonly struct SCEvent : System.IEquatable<SCEvent>
{
    public readonly string Type;
    public readonly static SCEvent emptyEvent = new SCEvent("");


    public SCEvent(string newType)
    {
        Type = newType;
    }


    public readonly bool Equals(SCEvent other)
    {
        return Type == other.Type;
    }


    public readonly override bool Equals(object other)
    {
        if (other is SCEvent e)
            return Equals(e);
        else
            return false;
    }


    public static bool operator ==(SCEvent a, SCEvent b)
    {
        return a.Equals(b);
    }


    public static bool operator !=(SCEvent a, SCEvent b)
    {
        return !a.Equals(b);
    }


    public readonly override int GetHashCode()
    {
        return Type.GetHashCode();
    }

    
    public readonly override string ToString()
    {
        return Type;
    }
}
