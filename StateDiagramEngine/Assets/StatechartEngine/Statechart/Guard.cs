public readonly struct Guard
{
    // Index of the property
    public readonly int property;


    public Guard(int property)
    {
        this.property = property;
    }


    public bool Evaluate(bool prop_value)
    {     
        return prop_value;
    }


    public bool IsEmpty()
    {
        return property < 0;
    }
}