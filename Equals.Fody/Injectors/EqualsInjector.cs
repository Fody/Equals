using System;
using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

public partial class ModuleWeaver
{
    const int ExactlyTheSameTypeAsThis = 0;
    const int ExactlyOfType = 1;
    const int EqualsOrSubtype = 2;

    static HashSet<string> simpleTypes = new HashSet<string>(new[]
    {
        "System.Boolean",
        "System.Byte",
        "System.SByte",
        "System.Char",
        "System.Double",
        "System.Single",
        "System.Int32",
        "System.UInt32",
        "System.Int64",
        "System.UInt64",
        "System.Int16",
        "System.UInt16"
    });

    public void InjectEqualsObject(TypeDefinition type, TypeReference typeRef, MethodReference newEquals, int typeCheck)
    {
        var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
        var method = new MethodDefinition("Equals", methodAttributes, TypeSystem.BooleanReference);
        MarkAsGeneratedCode(method.CustomAttributes);

        var obj = method.Parameters.Add("obj", TypeSystem.ObjectReference);

        var body = method.Body;
        body.InitLocals = true;
        var result = body.Variables.Add(TypeSystem.BooleanReference);

        var ins = body.Instructions;

        var labelRet = Instruction.Create(OpCodes.Nop);

        AddCheckEqualsReference(type, ins, true);

        ins.If(
            c => AddTypeChecking(type, typeRef, typeCheck, c),
            t => AddInternalEqualsCall(type, typeRef, newEquals, t, result),
            TypeDefinitionExtensions.AddReturnFalse);

        AddReturn(ins, labelRet, result);

        body.OptimizeMacros();

        type.Methods.AddOrReplace(method);
    }

    public void InjectEqualsType(TypeDefinition type, TypeReference typeRef, MethodReference newEquals)
    {
        var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
        var method = new MethodDefinition("Equals", methodAttributes, TypeSystem.BooleanReference);
        MarkAsGeneratedCode(method.CustomAttributes);
        var body = method.Body;
        var ins = body.Instructions;

        var other = method.Parameters.Add("other", typeRef);

        AddCheckEqualsReference(type, ins, false);
        AddEqualsTypeReturn(newEquals, ins, type);

        body.OptimizeMacros();
        type.Methods.AddOrReplace(method);
    }

    public MethodReference InjectEqualsInternal(TypeDefinition type, TypeReference typeRef, MethodDefinition collectionEquals, bool ignoreBaseClassProperties)
    {
        var methodAttributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static;
        var method = new MethodDefinition("EqualsInternal", methodAttributes, TypeSystem.BooleanReference);
        MarkAsGeneratedCode(method.CustomAttributes);

        var left = method.Parameters.Add("left", typeRef);
        var right = method.Parameters.Add("right", typeRef);

        var body = method.Body;
        body.InitLocals = true;
        var ins = body.Instructions;

        var properties = type.GetPropertiesWithoutIgnores(ignoreAttributeName);
        if (ignoreBaseClassProperties)
        {
            properties = properties.IgnoreBaseClassProperties(type);
        }

        foreach (var property in properties)
        {
            AddPropertyCode(type, collectionEquals, property, ins, left, right);
        }

        var methods = type.GetMethods();
        var customLogic = methods
            .Where(x => x.CustomAttributes.Any(y => y.AttributeType.Name == customEqualsAttribute)).ToArray();

        if (customLogic.Length > 2)
        {
            throw new WeavingException("Only one custom method can be specified.");
        }

        if (customLogic.Length == 1)
        {
            AddCustomLogicCall(type, body, ins, customLogic);
        }

        ins.AddReturnTrue();

        body.OptimizeMacros();

        type.Methods.AddOrReplace(method);

        var methodToCall = new MethodReference(method.Name, method.ReturnType, typeRef);
        foreach (var parameter in method.Parameters)
        {
            methodToCall.Parameters.Add(parameter);
        }

        return methodToCall;
    }

    void AddCustomLogicCall(TypeDefinition type, MethodBody body, Collection<Instruction> ins, MethodDefinition[] customLogic)
    {
        ins.IfNot(
            c =>
            {
                var customMethod = ModuleDefinition.ImportReference(customLogic[0]);

                var parameters = customMethod.Parameters;
                if (parameters.Count != 1)
                {
                    throw new WeavingException($"Custom equals of type {type.FullName} have to have one parameter.");
                }
                if (parameters[0].ParameterType.Resolve().FullName != type.FullName)
                {
                    throw new WeavingException($"Custom equals of type {type.FullName} have to have one parameter of type {type.FullName}.");
                }

                MethodReference selectedMethod;
                TypeReference resolverType;
                if (customMethod.DeclaringType.HasGenericParameters)
                {
                    var genericInstanceType = type.GetGenericInstanceType(type);
                    resolverType = ModuleDefinition.ImportReference(genericInstanceType);
                    var newRef = new MethodReference(customMethod.Name, customMethod.ReturnType)
                    {
                        DeclaringType = resolverType,
                        HasThis = true,
                    };
                    newRef.Parameters.Add(customMethod.Parameters[0].Name, resolverType);

                    selectedMethod = newRef;
                }
                else
                {
                    resolverType = type;
                    selectedMethod = customMethod;
                }

                var imported = ModuleDefinition.ImportReference(selectedMethod);
                if (!type.IsValueType)
                {
                    ins.Add(Instruction.Create(OpCodes.Ldarg_0));
                }
                else
                {
                    var argVariable = body.Variables.Add(resolverType);
                    ins.Add(Instruction.Create(OpCodes.Ldarg_0));
                    ins.Add(Instruction.Create(OpCodes.Stloc, argVariable));

                    ins.Add(Instruction.Create(OpCodes.Ldloca, argVariable));
                }
                ins.Add(Instruction.Create(OpCodes.Ldarg_1));
                ins.Add(Instruction.Create(imported.GetCallForMethod(), imported));
            },
            TypeDefinitionExtensions.AddReturnFalse);
    }


    static void AddEqualsTypeReturn(MethodReference newEquals, Collection<Instruction> ins, TypeReference type)
    {
        ins.Add(Instruction.Create(OpCodes.Ldarg_0));
        if (type.IsValueType)
        {
            var resolvedType = type.GetGenericInstanceType(type);
            ins.Add(Instruction.Create(OpCodes.Ldobj, resolvedType));
        }
        ins.Add(Instruction.Create(OpCodes.Ldarg_1));
        ins.Add(Instruction.Create(OpCodes.Call, newEquals));
        ins.Add(Instruction.Create(OpCodes.Ret));
    }

    static void AddInternalEqualsCall(TypeDefinition type, TypeReference typeRef, MethodReference newEquals, Collection<Instruction> t, VariableDefinition result)
    {
        t.Add(Instruction.Create(OpCodes.Ldarg_0));
        if (type.IsValueType)
        {
            var resolvedType = type.GetGenericInstanceType(type);
            t.Add(Instruction.Create(OpCodes.Ldobj, resolvedType));
        }
        t.Add(Instruction.Create(OpCodes.Ldarg_1));
        if (type.IsValueType)
        {
            t.Add(Instruction.Create(OpCodes.Unbox_Any, typeRef));
        }
        else
        {
            t.Add(Instruction.Create(OpCodes.Castclass, typeRef));
        }

        t.Add(Instruction.Create(OpCodes.Call, newEquals));

        t.Add(Instruction.Create(OpCodes.Stloc, result));
    }

    static void AddReturn(Collection<Instruction> ins, Instruction labelRet, VariableDefinition result)
    {
        ins.Add(labelRet);
        ins.Add(Instruction.Create(OpCodes.Ldloc, result));
        ins.Add(Instruction.Create(OpCodes.Ret));
    }


    void AddTypeChecking(TypeDefinition type, TypeReference typeRef, int typeCheck, Collection<Instruction> c)
    {
        switch (typeCheck)
        {
            case ExactlyTheSameTypeAsThis:
                AddExactlyTheSameTypeAsThis(type, c);
                break;

            case ExactlyOfType:
                AddExactlyOfType(type, typeRef, c);
                break;

            case EqualsOrSubtype:
                AddEqualsOrSubtype(typeRef, c);
                break;

            default:
                throw new WeavingException("Unknown TypeCheck: " + type);
        }
    }

    static void AddEqualsOrSubtype(TypeReference typeRef, Collection<Instruction> c)
    {
        c.Add(Instruction.Create(OpCodes.Ldarg_1));
        c.Add(Instruction.Create(OpCodes.Isinst, typeRef));
    }

    void AddExactlyOfType(TypeDefinition type, TypeReference typeRef, Collection<Instruction> c)
    {
        c.Add(Instruction.Create(OpCodes.Ldarg_0));
        if (type.IsValueType)
        {
            var resolved = type.GetGenericInstanceType(type);
            c.Add(Instruction.Create(OpCodes.Ldobj, resolved));
            c.Add(Instruction.Create(OpCodes.Box, resolved));
        }
        c.Add(Instruction.Create(OpCodes.Call, GetType));

        c.Add(Instruction.Create(OpCodes.Ldtoken, typeRef));
        c.Add(Instruction.Create(OpCodes.Call, GetTypeFromHandle));
        c.Add(Instruction.Create(OpCodes.Ceq));
    }

    void AddExactlyTheSameTypeAsThis(TypeDefinition type, Collection<Instruction> c)
    {
        c.Add(Instruction.Create(OpCodes.Ldarg_0));
        if (type.IsValueType)
        {
            var resolved = type.GetGenericInstanceType(type);
            c.Add(Instruction.Create(OpCodes.Ldobj, resolved));
            c.Add(Instruction.Create(OpCodes.Box, resolved));
        }
        c.Add(Instruction.Create(OpCodes.Call, GetType));

        c.Add(Instruction.Create(OpCodes.Ldarg_1));
        c.Add(Instruction.Create(OpCodes.Callvirt, GetType));

        c.Add(Instruction.Create(OpCodes.Ceq));
    }

    void AddCheckEqualsReference(TypeDefinition type, Collection<Instruction> ins, bool skipBoxingSecond)
    {
        var resolvedType = type.IsValueType ? type.GetGenericInstanceType(type) : null;
        ins.If(
            c =>
            {
                c.Add(Instruction.Create(OpCodes.Ldnull));
                c.Add(Instruction.Create(OpCodes.Ldarg_1));
                if (type.IsValueType && !skipBoxingSecond)
                {
                    c.Add(Instruction.Create(OpCodes.Box, resolvedType));
                }
                c.Add(Instruction.Create(OpCodes.Call, ReferenceEquals));
            },
            TypeDefinitionExtensions.AddReturnFalse);

        ins.If(
            c =>
            {
                c.Add(Instruction.Create(OpCodes.Ldarg_0));
                if (type.IsValueType)
                {
                    c.Add(Instruction.Create(OpCodes.Ldobj, resolvedType));
                    c.Add(Instruction.Create(OpCodes.Box, resolvedType));
                }
                c.Add(Instruction.Create(OpCodes.Ldarg_1));
                if (type.IsValueType && !skipBoxingSecond)
                {
                    c.Add(Instruction.Create(OpCodes.Box, resolvedType));
                }
                c.Add(Instruction.Create(OpCodes.Call, ReferenceEquals));
            },
            TypeDefinitionExtensions.AddReturnTrue);
    }

    void AddPropertyCode(TypeDefinition type, MethodDefinition collectionEquals, PropertyDefinition property, Collection<Instruction> ins, ParameterDefinition left, ParameterDefinition right)
    {
        ins.IfNot(
            c =>
            {
                if (!property.PropertyType.IsGenericParameter)
                {
                    var propType = property.PropertyType.Resolve();
                    var isCollection = propType.IsCollection() || property.PropertyType.IsArray;
                    var fullName = property.PropertyType.FullName;
                    if ((simpleTypes.Contains(fullName) || propType.IsEnum) && !property.PropertyType.IsArray)
                    {
                        AddSimpleValueCheck(c, property, type, left, right);
                    }
                    else if (!isCollection || fullName == typeof(string).FullName)
                    {
                        AddNormalCheck(type, c, property, left, right);
                    }
                    else
                    {
                        AddCollectionCheck(type, collectionEquals, c, property, property.PropertyType, left, right);
                    }
                }
                else
                {
                    var genericType = property.PropertyType.GetGenericInstanceType(type);
                    AddNormalCheck(type, c, property, left, right);
                }
            },
            TypeDefinitionExtensions.AddReturnFalse);
    }

    void AddNormalCheck(TypeDefinition type, Collection<Instruction> c, PropertyDefinition property, ParameterDefinition left, ParameterDefinition right)
    {
        var genericInstance = new Lazy<TypeReference>(() => ModuleDefinition.ImportReference(property.PropertyType.GetGenericInstanceType(type)));
        var getMethodImported = ModuleDefinition.ImportReference(property.GetGetMethod(type));

        c.Add(Instruction.Create(type.GetLdArgForType(), left));
        c.Add(Instruction.Create(getMethodImported.GetCallForMethod(), getMethodImported));
        if (property.PropertyType.IsValueType || property.PropertyType.IsGenericParameter)
        {
            c.Add(Instruction.Create(OpCodes.Box, genericInstance.Value));
        }
        c.Add(Instruction.Create(type.GetLdArgForType(), right));
        c.Add(Instruction.Create(getMethodImported.GetCallForMethod(), getMethodImported));
        if (property.PropertyType.IsValueType || property.PropertyType.IsGenericParameter)
        {
            c.Add(Instruction.Create(OpCodes.Box, genericInstance.Value));
        }
        c.Add(Instruction.Create(OpCodes.Call, StaticEquals));
    }

    void AddCollectionCheck(TypeDefinition type, MethodDefinition collectionEquals, Collection<Instruction> c, PropertyDefinition property, TypeReference propType, ParameterDefinition left, ParameterDefinition right)
    {
        c.If(
            AddCollectionFirstArgumentCheck,
            AddCollectionSecondArgumentCheck,
            e => AddCollectionEquals(type, collectionEquals, property, propType, e, left, right));
    }

    void AddCollectionEquals(TypeDefinition type, MethodDefinition collectionEquals, PropertyDefinition property, TypeReference propType, Collection<Instruction> e, ParameterDefinition left, ParameterDefinition right)
    {
        e.If(
            cf =>
            {
                cf.Add(Instruction.Create(type.GetLdArgForType(), right));
                cf.Add(Instruction.Create(OpCodes.Ldnull));
                cf.Add(Instruction.Create(OpCodes.Ceq));
            },
            t => t.Add(Instruction.Create(OpCodes.Ldc_I4_0)),
            es =>
            {
                var getMethod = property.GetGetMethod(type);
                var getMethodImported = ModuleDefinition.ImportReference(getMethod);

                es.Add(Instruction.Create(type.GetLdArgForType(), left));
                es.Add(Instruction.Create(getMethodImported.GetCallForMethod(), getMethodImported));
                if (propType.IsValueType && !property.PropertyType.IsArray)
                {
                    var imported = ModuleDefinition.ImportReference(property.PropertyType);
                    es.Add(Instruction.Create(OpCodes.Box, imported));
                }

                es.Add(Instruction.Create(type.GetLdArgForType(), right));
                es.Add(Instruction.Create(getMethodImported.GetCallForMethod(), getMethodImported));
                if (propType.IsValueType && !property.PropertyType.IsArray)
                {
                    var imported = ModuleDefinition.ImportReference(property.PropertyType);
                    es.Add(Instruction.Create(OpCodes.Box, imported));
                }

                es.Add(Instruction.Create(OpCodes.Call, collectionEquals));
            });
    }

    static void AddCollectionSecondArgumentCheck(Collection<Instruction> t)
    {
        t.If(
            ct =>
            {
                ct.Add(Instruction.Create(OpCodes.Ldarg_1));
                ct.Add(Instruction.Create(OpCodes.Ldnull));
                ct.Add(Instruction.Create(OpCodes.Ceq));
            },
            tt => tt.Add(Instruction.Create(OpCodes.Ldc_I4_1)),
            ft => ft.Add(Instruction.Create(OpCodes.Ldc_I4_0)));
    }

    static void AddCollectionFirstArgumentCheck(Collection<Instruction> cf)
    {
        cf.Add(Instruction.Create(OpCodes.Ldarg_0));
        cf.Add(Instruction.Create(OpCodes.Ldnull));
        cf.Add(Instruction.Create(OpCodes.Ceq));
    }

    void AddSimpleValueCheck(Collection<Instruction> c, PropertyDefinition property, TypeDefinition type, ParameterDefinition left, ParameterDefinition right)
    {
        var getMethod = property.GetGetMethod(type);
        var getMethodImported = ModuleDefinition.ImportReference(getMethod);

        c.Add(Instruction.Create(type.GetLdArgForType(), left));
        c.Add(Instruction.Create(getMethodImported.GetCallForMethod(), getMethodImported));

        c.Add(Instruction.Create(type.GetLdArgForType(), right));
        c.Add(Instruction.Create(getMethodImported.GetCallForMethod(), getMethodImported));

        c.Add(Instruction.Create(OpCodes.Ceq));
    }
}