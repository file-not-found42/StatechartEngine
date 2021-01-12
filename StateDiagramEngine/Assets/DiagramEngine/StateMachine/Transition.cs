using System.Collections;
using System.Collections.Generic;

public class Transition
{
    Activity passthrough;
    Condition cond;
    SMEvent trigger;

    public State destination { get; internal set; }

    public bool Match(SMEvent e)
    {
        return e.Type == trigger.Type && cond();
    }

    public void Traverse()
    {
        passthrough();
    }
}
