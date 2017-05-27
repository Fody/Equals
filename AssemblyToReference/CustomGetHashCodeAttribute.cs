using System;

/// <summary>
/// Custom method marker. The Method is called by an auto-generated GetHashCode method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class CustomGetHashCodeAttribute : Attribute
{
}