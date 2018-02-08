using System;
using Mono.Cecil;
#pragma warning disable 108,114

public partial class ModuleWeaver
{
    public MethodReference GetTypeFromHandle;

    public TypeReference ObjectType;
    public MethodReference StaticEquals;
    public MethodReference ReferenceEquals;
    public MethodReference GetType;
    public MethodReference GetHashcode;
    public MethodReference GeneratedCodeAttributeConstructor;
    public TypeReference BooleanType;

    public TypeReference StringReference;
    public TypeReference Int32Type;
    public TypeReference IEnumerableType;
    public MethodReference GetEnumerator;
    public TypeReference IEquatableType;
    public TypeReference IEnumeratorType;
    public MethodReference MoveNext;
    public MethodReference GetCurrent;
    public MethodReference DebuggerNonUserCodeAttributeConstructor;

    static ModuleDefinition module;

    public static void SetModule(ModuleDefinition module)
    {
        ModuleWeaver.module = module;
    }

    public void FindReferences(Func<string, TypeDefinition> typeFinder)
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