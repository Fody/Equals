using Mono.Cecil;
using Mono.Collections.Generic;

namespace Equals.Fody.Extensions
{
    public static class CollectionCustomAttributeExtensions
    {
        public static void MarkAsGeneratedCode(this Collection<CustomAttribute> customAttributes)
        {
            AddCustomAttributeArgument(customAttributes);
            AddDebuggerNonUserCodeAttribute(customAttributes);
        }

        static void AddDebuggerNonUserCodeAttribute(Collection<CustomAttribute> customAttributes)
        {
            var debuggerAttribute = new CustomAttribute(ReferenceFinder.DebuggerNonUserCodeAttribute.Constructor);
            customAttributes.Add(debuggerAttribute);
        }

        static void AddCustomAttributeArgument(Collection<CustomAttribute> customAttributes)
        {
            var version = typeof (ModuleWeaver).Assembly.GetName().Version.ToString();
            var name = typeof (ModuleWeaver).Assembly.GetName().Name;

            var generatedAttribute = new CustomAttribute(ReferenceFinder.GeneratedCodeAttribute.ConstructorStringString);
            generatedAttribute.ConstructorArguments.Add(new CustomAttributeArgument(ReferenceFinder.String.TypeReference, name));
            generatedAttribute.ConstructorArguments.Add(new CustomAttributeArgument(ReferenceFinder.String.TypeReference, version));
            customAttributes.Add(generatedAttribute);
        }
    }
}
