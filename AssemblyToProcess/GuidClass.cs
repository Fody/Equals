using System;

[Equals]
public class GuidClass
{
    public Guid Key { get; set; }

    public static bool operator ==(GuidClass left, GuidClass right) => Operator.Weave(left, right);
    public static bool operator !=(GuidClass left, GuidClass right) => Operator.Weave(left, right);
}