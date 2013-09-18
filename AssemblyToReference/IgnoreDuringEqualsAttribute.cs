using System;

/// <summary>
/// Property will be ignored during generating ToString method. 
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class IgnoreDuringEqualsAttribute : Attribute
{
}
