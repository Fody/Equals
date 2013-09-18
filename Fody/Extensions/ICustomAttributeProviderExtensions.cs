using System.Linq;

using Mono.Cecil;

public static class ICustomAttributeProviderExtensions
{
    public static void RemoveAttribute(this ICustomAttributeProvider definition, string name)
    {
        var customAttributes = definition.CustomAttributes;

        var attribute = customAttributes.FirstOrDefault(x => x.AttributeType.Name == name);

        if (attribute != null)
        {
            customAttributes.Remove(attribute);
        }
    }
}

