using System;
using Mono.Cecil;
#pragma warning disable 108,114

public static class ReferenceFinder
{
    public static MethodReference GetTypeFromHandle;


    public static TypeReference ObjectType;
    public static MethodReference StaticEquals;
    public static MethodReference ReferenceEquals;
    public static MethodReference GetType;
    public static MethodReference GetHashcode;
    public static MethodReference GeneratedCodeAttributeConstructor;
    public static TypeReference BooleanType;

    public static TypeReference StringReference;
    public static TypeReference Int32Type;
    public static TypeReference IEnumerableType;
    public static MethodReference GetEnumerator;
    public static TypeReference IEquatableType;
    public static TypeReference IEnumeratorType;
    public static MethodReference MoveNext;
    public static MethodReference GetCurrent;
    public static MethodReference DebuggerNonUserCodeAttributeConstructor;

    static ModuleDefinition module;

    public static void SetModule(ModuleDefinition module)
    {
        ReferenceFinder.module = module;
    }

    public static MethodReference ImportCustom(MethodReference method)
    {
        return module.ImportReference(method);
    }

    public static TypeReference ImportCustom(TypeReference type)
    {
        return module.ImportReference(type);
    }

    public static void FindReferences(Func<string, TypeDefinition> typeFinder)
    {
        BooleanType = module.ImportReference(typeFinder("System.Boolean"));

        Int32Type = module.ImportReference(typeFinder("System.Int32"));

        StringReference = module.ImportReference(typeFinder("System.String"));

        var typeDefinition = typeFinder("System.Type");
        GetTypeFromHandle = module.ImportReference(typeDefinition.FindMethod("GetTypeFromHandle", "RuntimeTypeHandle"));

        var objectDefinition = typeFinder("System.Object");
        ObjectType = module.ImportReference(objectDefinition);
        GetHashcode = module.ImportReference(objectDefinition.FindMethod("GetHashCode"));
        GetType = module.ImportReference(objectDefinition.FindMethod("GetType"));
        StaticEquals = module.ImportReference(objectDefinition.FindMethod("Equals", "Object", "Object"));
        ReferenceEquals = module.ImportReference(objectDefinition.FindMethod("ReferenceEquals", "Object", "Object"));

        var enumerableType = typeFinder("System.Collections.IEnumerable");
        IEnumerableType = module.ImportReference(enumerableType);
        GetEnumerator = module.ImportReference(enumerableType.FindMethod("GetEnumerator"));

        var ienumeratorDefinition = typeFinder("System.Collections.IEnumerator");
        IEnumeratorType = module.ImportReference(ienumeratorDefinition);
        MoveNext = module.ImportReference(ienumeratorDefinition.FindMethod("MoveNext"));
        GetCurrent = module.ImportReference(ienumeratorDefinition.FindMethod("get_Current"));

        IEquatableType = module.ImportReference(typeFinder("System.IEquatable`1"));

        var generatedCodeType = typeFinder("System.CodeDom.Compiler.GeneratedCodeAttribute");
        GeneratedCodeAttributeConstructor = module.ImportReference(generatedCodeType.FindMethod(".ctor", "String", "String"));

        var debuggerNonUserCodeType = typeFinder("System.Diagnostics.DebuggerNonUserCodeAttribute");
        DebuggerNonUserCodeAttributeConstructor = module.ImportReference(debuggerNonUserCodeType.FindMethod(".ctor"));
    }
}