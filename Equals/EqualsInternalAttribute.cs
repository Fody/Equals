using System;

/// <summary>
/// Custom method marker. The Method is called by an auto-generated equality comparison after a generated EqualsInternal.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class CustomEqualsInternalAttribute : Attribute
{
}