using System;

/// <summary>
/// Property will be ignored during generating Equals/GetHashCode method. 
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class IgnoreDuringEqualsAttribute : Attribute
{
}
