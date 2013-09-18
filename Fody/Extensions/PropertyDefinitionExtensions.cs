using Mono.Cecil;

namespace Equals.Fody.Extensions
{
    public static class PropertyDefinitionExtensions
    {
        public static MethodReference GetGetMethod(this PropertyDefinition property)
        {
            MethodReference get;

            if (property.DeclaringType.HasGenericParameters)
            {
                var type = property.DeclaringType;
                var t = type.GenericParameters[0];
                var of_t = new GenericInstanceType(type);
                of_t.GenericArguments.Add(t);

                var field_of_t = new MethodReference(property.GetMethod.Name, property.GetMethod.ReturnType)
                {
                    DeclaringType = of_t,
                    HasThis = true
                };
                get = field_of_t;
            }
            else
            {
                get = property.GetMethod;
            }

            return get;
        }
    }
}
