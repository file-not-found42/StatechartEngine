using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    public string property;
    public bool invert;


    public Condition(string prop, bool inv)
    {
        property = prop;
        invert = inv;
    }


    public Condition(string expression)
    {

    }
}