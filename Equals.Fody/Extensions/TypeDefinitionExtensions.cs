using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

public static class TypeDefinitionExtensions
{
    public static MethodDefinition FindMethod(this TypeDefinition typeDefinition, string method, params string[] paramTypes)
    {
        return typeDefinition.Methods.First(x => x.Name == method && x.IsMatch(paramTypes));
    }

    public static bool IsCollection(this TypeDefinition type)
    {
        return !type.Name.Equals("String") &&
               type.Interfaces.Any(i => i.InterfaceType.Name.Equals("IEnumerable"));
    }

    public static PropertyDefinition[] GetPropertiesWithoutIgnores(this TypeDefinition type, string ignoreAttributeName)
    {
        var properties = new Dictionary<string, PropertyDefinition>();

        // Since we are resolving stuff from top => bottom, the top classes can exclude overridden properties. This
        // hash set keeps track of ignored properties (even when they are not ignored in base classes) to make sure
        // the properties are correctly ignored, see #23
        var ignoredProperties = new HashSet<string>();

        var currentType = type;

        do
        {
            foreach (var property in currentType.Properties)
            {
                var isPotentialCandidate = property.HasThis &&
                                           !property.HasParameters;
                if (!isPotentialCandidate)
                {
                    continue;
                }

                if (ignoredProperties.Contains(property.Name))
                {
                    continue;
                }

                var shouldBeIgnored = property.CustomAttributes.Any(y => y.AttributeType.Name == ignoreAttributeName);
                if (shouldBeIgnored)
                {
                    ignoredProperties.Add(property.Name);
                    continue;
                }

                if (properties.ContainsKey(property.Name))
                {
                    continue;
                }

                properties.Add(property.Name, property);
            }

            currentType = currentType.BaseType.Resolve();
        } while (currentType.FullName != typeof(object).FullName);

        return properties.Values.ToArray();
    }

    public static TypeReference GetGenericInstanceType(this TypeReference type, TypeReference targetType)
    {
        if (targetType is GenericInstanceType genericInstance)
        {
            return genericInstance;
        }

        if (type.IsGenericParameter)
        {
            var genericParameter = (GenericParameter)type;

            var current = targetType;
            var currentResolved = current.Resolve();

            while (currentResolved.FullName != genericParameter.DeclaringType.FullName)
            {
                if (currentResolved.BaseType == null)
                {
                    return type;
                }
                current = currentResolved.BaseType;
                currentResolved = current.Resolve();
            }

            if (current is GenericInstanceType genericInstanceType)
            {
                return genericInstanceType.GenericArguments[genericParameter.Position];
            }

            return type;
        }

        if (type.HasGenericParameters)
        {
            GenericInstanceType genericInstanceType;
            var parent = targetType;
            var parentReference = targetType;

            if (type.FullName == targetType.Resolve().FullName)
            {
                genericInstanceType = GetGenericInstanceType(type, type.GenericParameters);
            }
            else
            {
                var propertyType = type.Resolve();

                TypeDefinition parentResolved;
                while (parent != null && propertyType.FullName != (parentResolved = parent.Resolve()).FullName)
                {
                    parentReference = parentResolved.BaseType;
                    parent = parentResolved.BaseType?.Resolve();
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

    static GenericInstanceType GetGenericInstanceType(TypeReference type, Collection<GenericParameter> parameters)
    {
        var genericInstanceType = new GenericInstanceType(type);
        foreach (var genericParameter in parameters)
        {
            genericInstanceType.GenericArguments.Add(genericParameter);
        }
        return genericInstanceType;
    }

    public static void AddNegateValue(this Collection<Instruction> ins)
    {
        ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
        ins.Add(Instruction.Create(OpCodes.Ceq));
    }

    public static void AddReturnValue(this Collection<Instruction> ins, bool isEquality)
    {
        if (!isEquality)
        {
            ins.AddNegateValue();
        }

        ins.Add(Instruction.Create(OpCodes.Ret));
    }

    public static OpCode GetLdArgForType(this TypeReference type)
    {
        if (type.IsValueType)
        {
            return OpCodes.Ldarga;
        }
        return OpCodes.Ldarg;
    }


    public static void AddReturnTrue(this Collection<Instruction> e)
    {
        e.Add(Instruction.Create(OpCodes.Ldc_I4_1));
        e.Add(Instruction.Create(OpCodes.Ret));
    }

    public static void AddReturnFalse(this Collection<Instruction> e)
    {
        e.Add(Instruction.Create(OpCodes.Ldc_I4_0));
        e.Add(Instruction.Create(OpCodes.Ret));
    }
}