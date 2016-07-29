using System;

/// <summary>
/// Custom method marker. The Method is called by an auto-generated GetHashCode method. 
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public sealed class CustomGetHashCodeAttribute : Attribute
{
}
