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
            MethodReference method = property.GetMethod;
            if (method.DeclaringType.HasGenericParameters)
            {
                var genericInstanceType = property.DeclaringType.GetGenericInstanceType(targetType);
                MethodReference newRef = new MethodReference(method.Name, method.ReturnType)
                {
                    DeclaringType = genericInstanceType,
                    HasThis = true
                };

                return newRef;
            }
            else
            {
                return method;
            }
        }
    }
}
