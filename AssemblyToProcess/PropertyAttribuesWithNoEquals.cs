public class PropertyAttributesWithNoEquals
{
    [IgnoreDuringEquals]
    public int Property { get; set; }

    [CustomEqualsInternal]
    [CustomGetHashCode]
    public void Method()
    {
    }
}