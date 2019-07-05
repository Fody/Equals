using System;

[Equals]
public class EmptyClass
{
    public EmptyClass()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; }

    public static bool operator ==(EmptyClass left, EmptyClass right) => Operator.Weave();
    public static bool operator !=(EmptyClass left, EmptyClass right) => Operator.Weave();
}
