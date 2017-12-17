using System;
using Mono.Cecil;
#pragma warning disable 108,114

public static class ReferenceFinder
{
    public static class Type
    {
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
        public static MethodReference ConstructorStringString;
    }

    public static class DebuggerNonUserCodeAttribute
    {
        public static MethodReference Constructor;
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

    public static void FindReferences(Func<string, TypeDefinition> typeFinder)
    {
        Boolean.TypeReference = moduleDefinition.ImportReference(typeFinder("System.Boolean"));

        Int32.TypeReference = moduleDefinition.ImportReference(typeFinder("System.Int32"));

        String.TypeReference = moduleDefinition.ImportReference(typeFinder("System.String"));

        var typeDefinition = typeFinder("System.Type");
        Type.GetTypeFromHandle = moduleDefinition.ImportReference(typeDefinition.FindMethod("GetTypeFromHandle", "RuntimeTypeHandle"));

        var objectDefinition = typeFinder("System.Object");
        Object.TypeReference = moduleDefinition.ImportReference(objectDefinition);
        Object.GetHashcode = moduleDefinition.ImportReference(objectDefinition.FindMethod("GetHashCode"));
        Object.GetType = moduleDefinition.ImportReference(objectDefinition.FindMethod("GetType"));
        Object.StaticEquals = moduleDefinition.ImportReference(objectDefinition.FindMethod("Equals", "Object", "Object"));
        Object.ReferenceEquals = moduleDefinition.ImportReference(objectDefinition.FindMethod("ReferenceEquals", "Object", "Object"));

        var enumerableType = typeFinder("System.Collections.IEnumerable");
        IEnumerable.TypeReference = moduleDefinition.ImportReference(enumerableType);
        IEnumerable.GetEnumerator = moduleDefinition.ImportReference(enumerableType.FindMethod("GetEnumerator"));

        var ienumeratorDefinition = typeFinder("System.Collections.IEnumerator");
        IEnumerator.TypeReference = moduleDefinition.ImportReference(ienumeratorDefinition);
        IEnumerator.MoveNext = moduleDefinition.ImportReference(ienumeratorDefinition.FindMethod("MoveNext"));
        IEnumerator.GetCurrent = moduleDefinition.ImportReference(ienumeratorDefinition.FindMethod("get_Current"));

        IEquatable.TypeReference = moduleDefinition.ImportReference(typeFinder("System.IEquatable`1"));

        var generatedCodeType = typeFinder("System.CodeDom.Compiler.GeneratedCodeAttribute");
        GeneratedCodeAttribute.ConstructorStringString = moduleDefinition.ImportReference(generatedCodeType.FindMethod(".ctor", "String", "String"));

        var debuggerNonUserCodeType = typeFinder("System.Diagnostics.DebuggerNonUserCodeAttribute");
        DebuggerNonUserCodeAttribute.Constructor = moduleDefinition.ImportReference(debuggerNonUserCodeType.FindMethod(".ctor"));
    }
}