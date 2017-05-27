using System.Linq;
using Mono.Cecil;
#pragma warning disable 108,114

public static class ReferenceFinder
{
    public static class Type
    {
        public static TypeReference TypeReference;
        public static MethodReference GetTypeFromHandle;
    }

    public static class Object
    {
        public static TypeReference TypeReference;
        public static MethodReference StaticEquals;
        public static MethodReference ReferenceEquals;
        public static MethodReference GetType;
        public static MethodReference GetHashcode;
    }

    public static class Boolean
    {
        public static TypeReference TypeReference;
    }

    public static class String
    {
        public static TypeReference TypeReference;
    }

    public static class Int32
    {
        public static TypeReference TypeReference;
    }

    public static class IEnumerable
    {
        public static TypeReference TypeReference;
        public static MethodReference GetEnumerator;
    }

    public static class IEnumerator
    {
        public static TypeReference TypeReference;
        public static MethodReference MoveNext;
        public static MethodReference GetCurrent;
    }

    public static class IEquatable
    {
        public static TypeReference TypeReference;
    }

    public static class GeneratedCodeAttribute
    {
        public static TypeReference TypeReference;
        public static MethodReference ConstructorStringString;
    }

    public static class DebuggerNonUserCodeAttribute
    {
        public static TypeReference TypeReference;
        public static MethodReference Constructor;
    }

    public static class DateTime
    {
        public static TypeReference TypeReference;
    }

    static ModuleDefinition moduleDefinition;

    public static void SetModule(ModuleDefinition module)
    {
        moduleDefinition = module;
    }

    public static MethodReference ImportCustom(MethodReference method)
    {
        return moduleDefinition.ImportReference(method);
    }

    public static TypeReference ImportCustom(TypeReference type)
    {
        return moduleDefinition.ImportReference(type);
    }

    public static void FindReferences(IAssemblyResolver assemblyResolver)
    {
        var baseLib = assemblyResolver.Resolve(new AssemblyNameReference("mscorlib", null));
        var baseLibTypes = baseLib.MainModule.Types;

        var systemLib = assemblyResolver.Resolve(new AssemblyNameReference("System", null));
        var systemLibTypes = systemLib.MainModule.Types;

        var winrt = baseLibTypes.All(type => type.Name != "Object");
        if (winrt)
        {
            baseLib = assemblyResolver.Resolve(new AssemblyNameReference("System.Runtime", null));
            baseLibTypes = baseLib.MainModule.Types;
        }

        DateTime.TypeReference = moduleDefinition.ImportReference(baseLibTypes.First(t => t.Name == "DateTime"));

        DateTime.TypeReference.Resolve();

        Boolean.TypeReference = moduleDefinition.ImportReference(baseLibTypes.First(t => t.Name == "Boolean"));

        Int32.TypeReference = moduleDefinition.ImportReference(baseLibTypes.First(t => t.Name == "Int32"));

        String.TypeReference = moduleDefinition.ImportReference(baseLibTypes.First(t => t.Name == "String"));

        Type.TypeReference = moduleDefinition.ImportReference(baseLibTypes.First(t => t.Name == "Type"));
        Type.GetTypeFromHandle = moduleDefinition.ImportReference(Type.TypeReference.Resolve().FindMethod("GetTypeFromHandle", "RuntimeTypeHandle"));

        Object.TypeReference = moduleDefinition.ImportReference(baseLibTypes.First(t => t.Name == "Object"));
        Object.GetHashcode = moduleDefinition.ImportReference(Object.TypeReference.Resolve().FindMethod("GetHashCode"));
        Object.GetType = moduleDefinition.ImportReference(Object.TypeReference.Resolve().FindMethod("GetType"));
        Object.StaticEquals = moduleDefinition.ImportReference(Object.TypeReference.Resolve().FindMethod("Equals", "Object", "Object"));
        Object.ReferenceEquals = moduleDefinition.ImportReference(Object.TypeReference.Resolve().FindMethod("ReferenceEquals", "Object", "Object"));

        IEnumerable.TypeReference = moduleDefinition.ImportReference(baseLibTypes.First(t => t.Name == "IEnumerable"));
        IEnumerable.GetEnumerator = moduleDefinition.ImportReference(IEnumerable.TypeReference.Resolve().FindMethod("GetEnumerator"));

        IEnumerator.TypeReference = moduleDefinition.ImportReference(baseLibTypes.First(t => t.Name == "IEnumerator"));
        IEnumerator.MoveNext = moduleDefinition.ImportReference(IEnumerator.TypeReference.Resolve().FindMethod("MoveNext"));
        IEnumerator.GetCurrent = moduleDefinition.ImportReference(IEnumerator.TypeReference.Resolve().FindMethod("get_Current"));

        IEquatable.TypeReference = moduleDefinition.ImportReference(baseLibTypes.First(t => t.Name == "IEquatable`1"));

        var generatedCodeType = systemLibTypes.FirstOrDefault(t => t.Name == "GeneratedCodeAttribute");
        if (generatedCodeType == null)
        {
            var systemDiagnosticsTools = assemblyResolver.Resolve(new AssemblyNameReference("System.Diagnostics.Tools", null));
            generatedCodeType = systemDiagnosticsTools.MainModule.Types.First(t => t.Name == "GeneratedCodeAttribute");
        }
        GeneratedCodeAttribute.TypeReference = moduleDefinition.ImportReference(generatedCodeType);
        GeneratedCodeAttribute.ConstructorStringString = moduleDefinition.ImportReference(GeneratedCodeAttribute.TypeReference.Resolve().FindMethod(".ctor", "String", "String"));

        var debuggerNonUserCodeType = baseLibTypes.FirstOrDefault(t => t.Name == "DebuggerNonUserCodeAttribute");
        if (debuggerNonUserCodeType == null)
        {
            var systemDiagnosticsDebug = assemblyResolver.Resolve(new AssemblyNameReference("System.Diagnostics.Debug", null));
            debuggerNonUserCodeType = systemDiagnosticsDebug.MainModule.Types.First(t => t.Name == "DebuggerNonUserCodeAttribute");
        }
        DebuggerNonUserCodeAttribute.TypeReference = moduleDefinition.ImportReference(debuggerNonUserCodeType);
        DebuggerNonUserCodeAttribute.Constructor = moduleDefinition.ImportReference(DebuggerNonUserCodeAttribute.TypeReference.Resolve().FindMethod(".ctor"));
    }
}