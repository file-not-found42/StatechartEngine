public class Guard
{
    readonly string property;


    public Guard(string expression)
    {
        property = expression;
    }


    public bool Evaluate(Status snap)
    {     
        return snap.GetProperty(property);
    }


    public bool isEmpty()
    {
        return property == null || property == "";
    }
}