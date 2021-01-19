using System.Collections;
using System.Collections.Generic;

public class Configuration
{
    public State atomicState = null;


    public override string ToString()
    {
        return atomicState.ToString();
    }
}
