﻿[Equals]
public class Child :
    Parent
{
    public long InChild { get; set; }

    public static bool operator ==(Child left, Child right) => Operator.Weave(left, right);
    public static bool operator !=(Child left, Child right) => Operator.Weave(left, right);
}