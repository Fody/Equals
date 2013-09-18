using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using Equals.Fody;
using Equals.Fody.Injectors;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

public class ModuleWeaver
{
    public const string attributeName = "EqualsAttribute";
    public const string assemblyName = "Equals";
    public const string ignoreAttributeName = "IgnoreDuringEqualsAttribute";

    public const string DoNotAddEqualityOperators = "DoNotAddEqualityOperators";
    public const string DoNotAddGetHashCode = "DoNotAddGetHashCode";
    public const string DoNotAddEquals = "DoNotAddEquals";

    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public XElement Config { get; set; }

    public IEnumerable<TypeDefinition> GetMachingTypes()
    {
        return ModuleDefinition.GetTypes().Where(x => x.CustomAttributes.Any(a => a.AttributeType.Name == attributeName));
    }

    private TypeReference GetGenericType(TypeReference type)
    {
        if (type.HasGenericParameters)
        {
            var parameters = type.GenericParameters.Select(x => (TypeReference)x).ToArray();
            return type.MakeGenericInstanceType(parameters);
        }
        else
        {
            return type;
        }
    }

    public void Execute()
    {
        ReferenceFinder.FindReferences(this.AssemblyResolver, this.ModuleDefinition);

        var collectionEquals = CollectionHelperInjector.Inject(ModuleDefinition);

        foreach (var type in GetMachingTypes())
        {
            var attribute = type.CustomAttributes.Where(x => x.AttributeType.Name == attributeName).Single();
            var typeRef = this.GetGenericType(type);

            if (!this.IsPropertySet(attribute, DoNotAddEquals))
            {
                int typeCheck = 0;
                var typeCheckProperty = attribute.Properties.Where(x => x.Name == "TypeCheck").SingleOrDefault();
                if (typeCheckProperty.Name != null)
                {
                    typeCheck = (int)typeCheckProperty.Argument.Value;
                }

                var newEquals = EqualsInjector.InjectEqualsInternal(type, typeRef, collectionEquals);
                EqualsInjector.InjectEqualsType(type, typeRef, newEquals);
                EqualsInjector.InjectEqualsObject(type, typeRef, newEquals, typeCheck);

                var typeInterface = ReferenceFinder.IEquatable.TypeReference.MakeGenericInstanceType(typeRef);
                type.Interfaces.Add(typeInterface);
            }

            if (!this.IsPropertySet(attribute, DoNotAddGetHashCode))
            {
                GetHashCodeInjector.Inject(type);
            }

            if (!this.IsPropertySet(attribute, DoNotAddEqualityOperators))
            {
                OperatorInjector.InjectEqualityOperator(type);
                OperatorInjector.InjectInequalityOperator(type);
            }

            this.RemoveFodyAttributes(type);
        }

        this.RemoveReference();
    }

    private bool IsPropertySet(CustomAttribute attribute, string property)
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

    private void RemoveReference()
    {
        var referenceToRemove = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == assemblyName);
        if (referenceToRemove != null)
        {
            ModuleDefinition.AssemblyReferences.Remove(referenceToRemove);
        }
    }

    private void RemoveFodyAttributes(TypeDefinition type)
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
    }
}