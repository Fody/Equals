using System.Linq;
using Mono.Cecil;

public static class PropertyDefinitionExtensions
{
    public static MethodReference GetGetMethod(this PropertyDefinition property, TypeReference targetType)
    {
        MethodReference method = property.GetMethod;
        if (method.DeclaringType.HasGenericParameters)
        {
            var genericInstanceType = property.DeclaringType.GetGenericInstanceType(targetType);
            method = new MethodReference(method.Name, method.ReturnType.IsGenericParameter ? method.ReturnType : property.Module.ImportReference(method.ReturnType))
            {
                DeclaringType = property.Module.ImportReference(genericInstanceType),
                HasThis = true
            };

        }
        return method;
    }

    public static PropertyDefinition[] IgnoreBaseClassProperties(this PropertyDefinition[] properties, TypeDefinition type)
    {
        return properties.Where(x => x.DeclaringType == type)
            .ToArray();
    }
}