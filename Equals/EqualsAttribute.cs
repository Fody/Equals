using System;

/// <summary>
/// Adds Equals,GetHashCode,==,!= methods to class.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class EqualsAttribute : Attribute
{
    // TODO: decide if this should remain. If yes, not setting it should cause an exception in case the woven type does not have equivalency operators defined THE RIGHT WAY.
    public bool DoNotAddEqualityOperators { get; set; }

    public bool DoNotAddGetHashCode { get; set; }

    public bool DoNotAddEquals { get; set; }

    public TypeCheck TypeCheck { get; set; }

    public bool IgnoreBaseClassProperties { get; set; }
}