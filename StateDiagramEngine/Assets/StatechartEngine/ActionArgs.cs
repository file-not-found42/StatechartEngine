public class ActionArgs
{
    public string Source { get; private set; }
    public Action.Type Type { get; private set; }


    public ActionArgs(string source, Action.Type type)
    {
        Source = source;
        Type = type;
    }
}
