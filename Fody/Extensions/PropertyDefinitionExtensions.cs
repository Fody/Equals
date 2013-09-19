using System;
using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace Equals.Fody.Extensions
{
    public static class PropertyDefinitionExtensions
    {
        public static MethodReference GetGetMethod(this PropertyDefinition property, TypeDefinition targetType)
        {
            MethodReference get;

            if (property.DeclaringType.HasGenericParameters)
            {
                var type = property.DeclaringType;

                GenericInstanceType genericInstanceType;

                TypeDefinition parent = targetType;
                TypeReference parentReference = targetType;

                if (property.DeclaringType == targetType)
                {
                    genericInstanceType = GetGenericInstanceType(type, type.GenericParameters);
                }
                else
                {
                    var propertyType = property.DeclaringType.Resolve();

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

                TypeReference returnType = property.PropertyType;


                get = new MethodReference(property.GetMethod.Name, returnType)
                {
                    DeclaringType = genericInstanceType,
                    HasThis = true
                };
            }
            else
            {
                get = property.GetMethod;
            }

            return get;
        }

        private static GenericInstanceType GetGenericInstanceType(TypeDefinition type, Collection<GenericParameter> parameters)
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
