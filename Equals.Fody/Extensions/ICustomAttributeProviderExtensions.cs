using System.Linq;
using Mono.Cecil;

static class ICustomAttributeProviderExtensions
{
    public static void RemoveAttribute(this ICustomAttributeProvider definition, string name)
    {
        var customAttributes = definition.CustomAttributes;

        var attribute = customAttributes.FirstOrDefault(_ => _.AttributeType.Name == name);

        if (attribute != null)
        {
            customAttributes.Remove(attribute);
        }
    }
}