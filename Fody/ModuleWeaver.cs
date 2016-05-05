using System.Linq;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Xml.Linq;
using Equals.Fody;
using Equals.Fody.Injectors;
using Mono.Cecil;
using Mono.Cecil.Rocks;

public class ModuleWeaver
{
    public const string attributeName = "EqualsAttribute";
    public const string assemblyName = "Equals";
    public const string ignoreAttributeName = "IgnoreDuringEqualsAttribute";
    public const string customAttribute = "CustomEqualsInternalAttribute";

    public const string DoNotAddEqualityOperators = "DoNotAddEqualityOperators";
    public const string DoNotAddGetHashCode = "DoNotAddGetHashCode";
    public const string DoNotAddEquals = "DoNotAddEquals";
    public const string IgnoreBaseClassProperties = "IgnoreBaseClassProperties";

    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public XElement Config { get; set; }

    public IEnumerable<TypeDefinition> GetMachingTypes()
    {
        return ModuleDefinition.GetTypes().Where(x => x.CustomAttributes.Any(a => a.AttributeType.Name == attributeName));
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

    public void Execute()
    {
        ReferenceFinder.SetModule(ModuleDefinition);
        ReferenceFinder.FindReferences(AssemblyResolver);

        var collectionEquals = CollectionHelperInjector.Inject(ModuleDefinition);

        foreach (var type in GetMachingTypes())
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

                var newEquals = EqualsInjector.InjectEqualsInternal(type, typeRef, collectionEquals, ignoreBaseClassProperties);
                EqualsInjector.InjectEqualsType(type, typeRef, newEquals);
                EqualsInjector.InjectEqualsObject(type, typeRef, newEquals, typeCheck);

                var typeInterface = ReferenceFinder.IEquatable.TypeReference.MakeGenericInstanceType(typeRef);
                if (type.Interfaces.All(x => x.FullName != typeInterface.FullName))
                {
                    type.Interfaces.Add(typeInterface);
                }
            }

            if (!IsPropertySet(attribute, DoNotAddGetHashCode))
            {
                GetHashCodeInjector.Inject(type, ignoreBaseClassProperties);
            }

            if (!IsPropertySet(attribute, DoNotAddEqualityOperators))
            {
                OperatorInjector.InjectEqualityOperator(type);
                OperatorInjector.InjectInequalityOperator(type);
            }

            RemoveFodyAttributes(type);

        }

        RemoveReference();
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

    void RemoveReference()
    {
        var referenceToRemove = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == assemblyName);
        if (referenceToRemove != null)
        {
            ModuleDefinition.AssemblyReferences.Remove(referenceToRemove);
        }
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
            method.RemoveAttribute(customAttribute);
        }
    }
}