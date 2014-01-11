using System;

/// <summary>
/// Custom method marker. The Method is called by an auto-generated equality comparison after a generated EqualsInternal. 
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public sealed class CustomEqualsInternalAttribute : Attribute
{
}
