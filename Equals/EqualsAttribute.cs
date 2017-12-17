using System;

/// <summary>
/// Adds Equals,GetHashCode,==,!= methods to class.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class EqualsAttribute : Attribute
{
    public bool DoNotAddEqualityOperators { get; set; }

    public bool DoNotAddGetHashCode { get; set; }

    public bool DoNotAddEquals { get; set; }

    public TypeCheck TypeCheck { get; set; }

    public bool IgnoreBaseClassProperties { get; set; }
}