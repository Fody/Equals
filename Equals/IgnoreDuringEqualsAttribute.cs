using System;

/// <summary>
/// Property will be ignored during generating Equals/GetHashCode method.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class IgnoreDuringEqualsAttribute : Attribute
{
}