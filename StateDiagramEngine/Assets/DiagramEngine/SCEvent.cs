using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCEvent : System.IEquatable<SCEvent>
{
    public readonly string Type;

    public SCEvent(string newType)
    {
        Type = newType;
    }


    public bool Equals(SCEvent other)
    {
        return this.Type.Equals(other.Type);
    }
}
