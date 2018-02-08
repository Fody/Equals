using System.Linq;
using System.Collections.Generic;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Rocks;

public partial class ModuleWeaver : BaseModuleWeaver
{
    public const string attributeName = "EqualsAttribute";
    public const string ignoreAttributeName = "IgnoreDuringEqualsAttribute";
    public const string customEqualsAttribute = "CustomEqualsInternalAttribute";

    public const string DoNotAddEqualityOperators = "DoNotAddEqualityOperators";
    public const string DoNotAddGetHashCode = "DoNotAddGetHashCode";
    public const string DoNotAddEquals = "DoNotAddEquals";
    public const string IgnoreBaseClassProperties = "IgnoreBaseClassProperties";

    public IEnumerable<TypeDefinition> GetMatchingTypes()
    {
        return ModuleDefinition.GetTypes()
            .Where(x => x.CustomAttributes.Any(a => a.AttributeType.Name == attributeName));
    }

    TypeReference GetGenericType(TypeReference type)
    {
        if (type.HasGenericParameters)
        {
            var parameters = type.GenericParameters.Select(x => (TypeReference) x).ToArray();
            return type.MakeGenericInstanceType(parameters);
        }
        return type;
    }

    public override void Execute()
    {
        FindReferences(FindType);

        var collectionEquals = InjectCollectionEquals(ModuleDefinition);

        var matchingTypes = GetMatchingTypes().ToArray();
        foreach (var type in matchingTypes)
        {
            var props = type.Properties;
            foreach (var prop in props)
            {
                ModuleDefinition.ImportReference(prop.PropertyType).Resolve();
            }

            var attribute = type.CustomAttributes.Single(x => x.AttributeType.Name == attributeName);
            var typeRef = GetGenericType(type);
            var ignoreBaseClassProperties = IsPropertySet(attribute, IgnoreBaseClassProperties);

            if (!IsPropertySet(attribute, DoNotAddEquals))
            {
                var typeCheck = 0;
                var typeCheckProperty = attribute.Properties.SingleOrDefault(x => x.Name == "TypeCheck");
                if (typeCheckProperty.Name != null)
                {
                    typeCheck = (int) typeCheckProperty.Argument.Value;
                }

                var newEquals = InjectEqualsInternal(type, typeRef, collectionEquals, ignoreBaseClassProperties);
                InjectEqualsType(type, typeRef, newEquals);
                InjectEqualsObject(type, typeRef, newEquals, typeCheck);

                var typeInterface = IEquatableType.MakeGenericInstanceType(typeRef);
                if (type.Interfaces.All(x => x.InterfaceType.FullName != typeInterface.FullName))
                {
                    type.Interfaces.Add(new InterfaceImplementation(typeInterface));
                }
            }

            if (!IsPropertySet(attribute, DoNotAddGetHashCode))
            {
                InjectGetHashCode(type, ignoreBaseClassProperties);
            }

            if (!IsPropertySet(attribute, DoNotAddEqualityOperators))
            {
                InjectEqualityOperator(type);
                InjectInequalityOperator(type);
            }
        }

        foreach (var type in matchingTypes)
        {
            RemoveFodyAttributes(type);
        }
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
        yield return "System";
        yield return "netstandard";
        yield return "System.Diagnostics.Tools";
        yield return "System.Diagnostics.Debug";
        yield return "System.Runtime";
    }

    bool IsPropertySet(CustomAttribute attribute, string property)
    {
        var argument = attribute.Properties.Where(x => x.Name == property)
            .Select(x => x.Argument)
            .FirstOrDefault();
        if (argument.Value == null)
        {
            return false;
        }

        return true.Equals(argument.Value);
    }

    void RemoveFodyAttributes(TypeDefinition type)
    {
        type.RemoveAttribute(attributeName);

        foreach (var property in type.Properties)
        {
            property.RemoveAttribute(ignoreAttributeName);
        }

        foreach (var property in type.Fields)
        {
            property.RemoveAttribute(ignoreAttributeName);
        }

        foreach (var method in type.Methods)
        {
            method.RemoveAttribute(customEqualsAttribute);
            method.RemoveAttribute(CustomGetHashCodeAttribute);
        }
    }

    public override bool ShouldCleanReference => true;
}