using System;

/// <summary>
/// Adds Equals,GetHashCode,==,!= methods to class.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class EqualsAttribute :
    Attribute
{
    public bool DoNotAddEqualityOperators { get; set; }

    public bool DoNotAddGetHashCode { get; set; }

    public bool DoNotAddEquals { get; set; }

    public TypeCheck TypeCheck { get; set; }

    public bool IgnoreBaseClassProperties { get; set; }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field /* TODO: #41 says maybe fields aren't supported */)]
public sealed class EqualityComparerAttribute : Attribute
{
    public EqualityComparerAttribute(string comparerFieldName)
    {
        //ComparerFieldName = comparerFieldName;
    }

    // Can't access from Mono anyway, so no point in having
    //public string ComparerFieldName { get; set; }
}