﻿[Equals(DoNotAddEquals = true, DoNotAddGetHashCode = true)]
public class OnlyOperator
{
    public int Value { get; set; }

    public override bool Equals(object obj)
    {
        if (!(obj is OnlyOperator second))
        {
            return false;
        }

        return Value == 1 && second.Value == 2;
    }

    public static bool operator ==(OnlyOperator left, OnlyOperator right) => Operator.Weave();
    public static bool operator !=(OnlyOperator left, OnlyOperator right) => Operator.Weave();
}