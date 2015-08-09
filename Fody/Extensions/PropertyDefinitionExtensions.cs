using Mono.Cecil;

namespace Equals.Fody.Extensions
{
    public static class PropertyDefinitionExtensions
    {
        public static MethodReference GetGetMethod(this PropertyDefinition property, TypeReference targetType)
        {
            MethodReference method = property.GetMethod;
            if (method.DeclaringType.HasGenericParameters)
            {
                var genericInstanceType = property.DeclaringType.GetGenericInstanceType(targetType);
                method = new MethodReference(method.Name, method.ReturnType.IsGenericParameter ? method.ReturnType : ReferenceFinder.ImportCustom(method.ReturnType))
                {
                    DeclaringType = ReferenceFinder.ImportCustom(genericInstanceType),
                    HasThis = true
                };

            }
            return method;
        }
    }
}
