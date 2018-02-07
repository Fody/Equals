using Mono.Cecil;
using Mono.Collections.Generic;

public static class CollectionCustomAttributeExtensions
{
    public static void MarkAsGeneratedCode(this Collection<CustomAttribute> customAttributes)
    {
        AddCustomAttributeArgument(customAttributes);
        AddDebuggerNonUserCodeAttribute(customAttributes);
    }

    static void AddDebuggerNonUserCodeAttribute(Collection<CustomAttribute> customAttributes)
    {
        var debuggerAttribute = new CustomAttribute(ModuleWeaver.DebuggerNonUserCodeAttributeConstructor);
        customAttributes.Add(debuggerAttribute);
    }

    static void AddCustomAttributeArgument(Collection<CustomAttribute> customAttributes)
    {
        var version = typeof (ModuleWeaver).Assembly.GetName().Version.ToString();
        var name = typeof (ModuleWeaver).Assembly.GetName().Name;

        var generatedAttribute = new CustomAttribute(ModuleWeaver.GeneratedCodeAttributeConstructor);
        generatedAttribute.ConstructorArguments.Add(new CustomAttributeArgument(ModuleWeaver.StringReference, name));
        generatedAttribute.ConstructorArguments.Add(new CustomAttributeArgument(ModuleWeaver.StringReference, version));
        customAttributes.Add(generatedAttribute);
    }
}