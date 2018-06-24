using System;
using Mono.Cecil;
#pragma warning disable 108,114

public partial class ModuleWeaver
{
    public MethodReference GetTypeFromHandle;

    public MethodReference StaticEquals;
    public MethodReference ReferenceEquals;
    public MethodReference GetType;
    public MethodReference GetHashcode;
    public MethodReference GeneratedCodeAttributeConstructor;

    public TypeReference IEnumerableType;
    public MethodReference GetEnumerator;
    public TypeReference IEquatableType;
    public TypeReference IEnumeratorType;
    public MethodReference MoveNext;
    public MethodReference GetCurrent;
    public MethodReference DebuggerNonUserCodeAttributeConstructor;

    public void FindReferences(Func<string, TypeDefinition> typeFinder)
    {
        var typeDefinition = typeFinder("System.Type");
        GetTypeFromHandle = ModuleDefinition.ImportReference(typeDefinition.FindMethod("GetTypeFromHandle", "RuntimeTypeHandle"));

        GetHashcode = ModuleDefinition.ImportReference(TypeSystem.ObjectDefinition.FindMethod("GetHashCode"));
        GetType = ModuleDefinition.ImportReference(TypeSystem.ObjectDefinition.FindMethod("GetType"));
        StaticEquals = ModuleDefinition.ImportReference(TypeSystem.ObjectDefinition.FindMethod("Equals", "Object", "Object"));
        ReferenceEquals = ModuleDefinition.ImportReference(TypeSystem.ObjectDefinition.FindMethod("ReferenceEquals", "Object", "Object"));

        var enumerableType = typeFinder("System.Collections.IEnumerable");
        IEnumerableType = ModuleDefinition.ImportReference(enumerableType);
        GetEnumerator = ModuleDefinition.ImportReference(enumerableType.FindMethod("GetEnumerator"));

        var ienumeratorDefinition = typeFinder("System.Collections.IEnumerator");
        IEnumeratorType = ModuleDefinition.ImportReference(ienumeratorDefinition);
        MoveNext = ModuleDefinition.ImportReference(ienumeratorDefinition.FindMethod("MoveNext"));
        GetCurrent = ModuleDefinition.ImportReference(ienumeratorDefinition.FindMethod("get_Current"));

        IEquatableType = ModuleDefinition.ImportReference(typeFinder("System.IEquatable`1"));

        var generatedCodeType = typeFinder("System.CodeDom.Compiler.GeneratedCodeAttribute");
        GeneratedCodeAttributeConstructor = ModuleDefinition.ImportReference(generatedCodeType.FindMethod(".ctor", "String", "String"));

        var debuggerNonUserCodeType = typeFinder("System.Diagnostics.DebuggerNonUserCodeAttribute");
        DebuggerNonUserCodeAttributeConstructor = ModuleDefinition.ImportReference(debuggerNonUserCodeType.FindMethod(".ctor"));
    }
}