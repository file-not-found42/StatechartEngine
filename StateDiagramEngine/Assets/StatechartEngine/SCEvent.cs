public class SCEvent
{
    public readonly string Type;
    public static SCEvent emptyEvent = new SCEvent("");


    public SCEvent(string newType)
    {
        Type = newType;
    }


    public override bool Equals(object other)
    {
        if (other is SCEvent @event)
            return Type == @event.Type;
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


    public override int GetHashCode()
    {
        return Type.GetHashCode();
    }

    
    public override string ToString()
    {
        return Type;
    }
}
