using System;

public class Guard
{
    enum Operator
    {
        None,
        Invert,
        Equals,
        NotEquals,
        LessThan,
        LessOrEqualThan,
        GreaterThan,
        GreaterOrEqualThan,
        IsActive
    }


    readonly string property;
    readonly Operator op;
    readonly ValueType value;


    public Guard(string expression)
    {
        property = expression;
    }


    public bool Evaluate(Status snap)
    {
        //switch (op)
        //{
        //    case Operator.None:
        //        break;
        //    case Operator.Invert:
        //    case Operator.Equals:
        //        break;
        //    case Operator.NotEquals:
        //        break;
        //    case Operator.LessThan:
        //        break;
        //    case Operator.LessOrEqualThan:
        //        break;
        //    case Operator.GreaterThan:
        //        break;
        //    case Operator.GreaterOrEqualThan:
        //        break;
        //    case Operator.IsActive:
        //        break;
        //    default:
        //        break;
        //}
        
        return snap.GetProperty(property);
    }


    public bool isEmpty()
    {
        return property == null || property == "";
    }
}