using System.Collections.Generic;
using System.Linq;
using Equals.Fody.Extensions;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace Equals.Fody.Extensions
{
    public static class TypeDefinitionExtensions
    {
        public static MethodDefinition FindMethod(this TypeDefinition typeDefinition, string method,
            params string[] paramTypes)
        {
            return typeDefinition.Methods.First(x => x.Name == method && x.IsMatch(paramTypes));
        }

        public static bool IsCollection(this TypeDefinition type)
        {
            return !type.Name.Equals("String") && (type.Interfaces.Any(i => i.Name.Equals("IEnumerable")));
        }

        public static PropertyDefinition[] GetPropertiesWithoutIgnores(this TypeDefinition type, string ignoreAttributeName)
        {
            var properties = new List<PropertyDefinition>();

            var currentType = type;
            do
            {
                var currentPoperties = currentType.Properties.Where(x => x.CustomAttributes.All(y => y.AttributeType.Name != ignoreAttributeName));
                properties.AddRange(currentPoperties);
                currentType = currentType.BaseType.Resolve();
            } while (currentType.FullName != typeof(object).FullName);

            return properties.ToArray();
        }

        public static TypeReference GetGenericInstanceType(this TypeReference type, TypeDefinition targetType)
        {
            if (type.HasGenericParameters)
            {
                GenericInstanceType genericInstanceType;
                TypeDefinition parent = targetType;
                TypeReference parentReference = targetType;

                if (type == targetType)
                {
                    genericInstanceType = GetGenericInstanceType(type, type.GenericParameters);
                }
                else
                {
                    var propertyType = type.Resolve();

                    while (propertyType != parent.Resolve())
                    {
                        parentReference = parent.BaseType;
                        parent = parent.BaseType.Resolve();
                    }

                    genericInstanceType = parentReference as GenericInstanceType;
                    if (genericInstanceType == null)
                    {
                        genericInstanceType = GetGenericInstanceType(type, parentReference.GenericParameters);
                    }
                }

                return genericInstanceType;
            }

            return type;
        }

        private static GenericInstanceType GetGenericInstanceType(TypeReference type, Collection<GenericParameter> parameters)
        {
            var genericInstanceType = new GenericInstanceType(type);
            foreach (var genericParameter in parameters)
            {
                genericInstanceType.GenericArguments.Add(genericParameter);
            }
            return genericInstanceType;
        }
    }
}