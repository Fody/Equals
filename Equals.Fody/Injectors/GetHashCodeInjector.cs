using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

public static class GetHashCodeInjector
{
    const string ignoreAttributeName = "IgnoreDuringEqualsAttribute";

    const string customAttribute = "CustomGetHashCodeAttribute";

    const int magicNumber = 397;

    public static MethodDefinition Inject(TypeDefinition type, bool ignoreBaseClassProperties)
    {
        var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
        var method = new MethodDefinition("GetHashCode", methodAttributes, ReferenceFinder.Int32.TypeReference);
        method.CustomAttributes.MarkAsGeneratedCode();

        var resultVariable = method.Body.Variables.Add(ReferenceFinder.Int32.TypeReference);

        var body = method.Body;
        body.InitLocals = true;
        var ins = body.Instructions;

        ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
        ins.Add(Instruction.Create(OpCodes.Stloc, resultVariable));

        var properties = ReferenceFinder.ImportCustom(type).Resolve().GetPropertiesWithoutIgnores(ignoreAttributeName);
        if (ignoreBaseClassProperties)
        {
            properties = properties.IgnoreBaseClassProperties(type);
        }

        if (properties.Length == 0)
        {
            AddResultInit(ins, resultVariable);
        }


        var methods = type.GetMethods();
        var customLogic = methods
            .Where(x => x.CustomAttributes.Any(y => y.AttributeType.Name == customAttribute)).ToArray();

        if (customLogic.Length > 2)
        {
            throw new WeavingException("Only one custom method can be specified.");
        }

        var isFirst = true;
        foreach (var property in properties)
        {
            var variable = AddPropertyCode(property, isFirst, ins, resultVariable, method, type);

            if (variable != null)
            {
                method.Body.Variables.Add(variable);
            }

            isFirst = false;
        }

        if (customLogic.Length == 1)
        {
            ins.Add(Instruction.Create(OpCodes.Ldloc, resultVariable));
            ins.Add(Instruction.Create(OpCodes.Ldc_I4, magicNumber));
            ins.Add(Instruction.Create(OpCodes.Mul));
            AddCustomLogicCall(type, body, ins, customLogic[0]);
            ins.Add(Instruction.Create(OpCodes.Xor));
            ins.Add(Instruction.Create(OpCodes.Stloc, resultVariable));
        }

        AddReturnCode(ins, resultVariable);

        body.OptimizeMacros();

        type.Methods.AddOrReplace(method);

        return method;
    }

    static void AddCustomLogicCall(TypeDefinition type, MethodBody body, Collection<Instruction> ins, MethodDefinition customLogic)
    {
        var customMethod = ReferenceFinder.ImportCustom(customLogic);

        var parameters = customMethod.Parameters;
        if (parameters.Count != 0)
        {
            throw new WeavingException($"Custom GetHashCode of type {type.FullName} have to have empty parameter list.");
        }
        if (customMethod.ReturnType.FullName != typeof(int).FullName)
        {
            throw new WeavingException($"Custom GetHashCode of type {type.FullName} have to return int.");
        }

        MethodReference selectedMethod;
        if (customMethod.DeclaringType.HasGenericParameters)
        {
            var genericInstanceType = type.GetGenericInstanceType(type);
            var resolverType = ReferenceFinder.ImportCustom(genericInstanceType);
            var newRef = new MethodReference(customMethod.Name, customMethod.ReturnType)
            {
                DeclaringType = resolverType,
                HasThis = true,
            };

            selectedMethod = newRef;
        }
        else
        {
            selectedMethod = customMethod;
        }

        var imported = ReferenceFinder.ImportCustom(selectedMethod);
        ins.Add(Instruction.Create(OpCodes.Ldarg_0));
        ins.Add(Instruction.Create(imported.GetCallForMethod(), imported));
    }

    static void AddReturnCode(Collection<Instruction> ins, VariableDefinition resultVariable)
    {
        ins.Add(Instruction.Create(OpCodes.Ldloc, resultVariable));
        ins.Add(Instruction.Create(OpCodes.Ret));
    }

    static VariableDefinition AddPropertyCode(PropertyDefinition property, bool isFirst, Collection<Instruction> ins, VariableDefinition resultVariable, MethodDefinition method, TypeDefinition type)
    {
        VariableDefinition variable = null;
        bool isCollection;
        var propType = ReferenceFinder.ImportCustom(property.PropertyType.GetGenericInstanceType(type));
        if (property.PropertyType.IsGenericParameter)
        {
            isCollection = false;
        }
        else
        {
            isCollection = propType.Resolve().IsCollection() || property.PropertyType.IsArray;
        }

        AddMultiplicityByMagicNumber(isFirst, ins, resultVariable, isCollection);

        if (property.PropertyType.FullName.StartsWith("System.Nullable`1"))
        {
            variable = AddNullableProperty(property, ins, type, variable);
        }
        else if (isCollection && property.PropertyType.FullName != "System.String")
        {
            AddCollectionCode(property, isFirst, ins, resultVariable, method, type);
        }
        else if (property.PropertyType.IsValueType || property.PropertyType.IsGenericParameter)
        {
            LoadVariable(property, ins, type);
            if (property.PropertyType.FullName != "System.Int32")
            {
                ins.Add(Instruction.Create(OpCodes.Box, propType));
                ins.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.Object.GetHashcode));
            }
        }
        else
        {
            LoadVariable(property, ins, type);
            AddNormalCode(property, ins, type);
        }

        if (!isFirst && !isCollection)
        {
            ins.Add(Instruction.Create(OpCodes.Xor));
        }

        if (!isCollection)
        {
            ins.Add(Instruction.Create(OpCodes.Stloc, resultVariable));
        }

        return variable;
    }

    static VariableDefinition AddNullableProperty(PropertyDefinition property, Collection<Instruction> ins, TypeDefinition type, VariableDefinition variable)
    {
        ins.If(c =>
            {
                var nullablePropertyResolved = property.PropertyType.Resolve();
                var nullablePropertyImported = ReferenceFinder.ImportCustom(property.PropertyType);

                ins.Add(Instruction.Create(OpCodes.Ldarg_0));
                var getMethod = ReferenceFinder.ImportCustom(property.GetGetMethod(type));
                c.Add(Instruction.Create(OpCodes.Call, getMethod));

                variable = new VariableDefinition(getMethod.ReturnType);
                c.Add(Instruction.Create(OpCodes.Stloc, variable));
                c.Add(Instruction.Create(OpCodes.Ldloca, variable));

                var hasValuePropertyResolved = nullablePropertyResolved.Properties.First(x => x.Name == "HasValue").Resolve();
                var hasMethod = ReferenceFinder.ImportCustom(hasValuePropertyResolved.GetGetMethod(nullablePropertyImported));
                c.Add(Instruction.Create(OpCodes.Call, hasMethod));
            },
            t =>
            {
                var nullableProperty = ReferenceFinder.ImportCustom(property.PropertyType);

                t.Add(Instruction.Create(OpCodes.Ldarg_0));
                var imp = property.GetGetMethod(type);
                var imp2 = ReferenceFinder.ImportCustom(imp);

                t.Add(Instruction.Create(OpCodes.Call, imp2));
                t.Add(Instruction.Create(OpCodes.Box, nullableProperty));
                t.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.Object.GetHashcode));
            },
            e => e.Add(Instruction.Create(OpCodes.Ldc_I4_0)));
        return variable;
    }

    static void LoadVariable(PropertyDefinition property, Collection<Instruction> ins, TypeDefinition type)
    {
        var get = property.GetGetMethod(type);
        var imported = ReferenceFinder.ImportCustom(get);
        ins.Add(Instruction.Create(OpCodes.Ldarg_0));
        ins.Add(Instruction.Create(OpCodes.Call, imported));
    }

    static void AddMultiplicityByMagicNumber(bool isFirst, Collection<Instruction> ins, VariableDefinition resultVariable,
        bool isCollection)
    {
        if (!isFirst && !isCollection)
        {
            ins.Add(Instruction.Create(OpCodes.Ldloc, resultVariable));
            ins.Add(Instruction.Create(OpCodes.Ldc_I4, magicNumber));
            ins.Add(Instruction.Create(OpCodes.Mul));
        }
    }

    static void AddValueTypeCode(PropertyDefinition property, Collection<Instruction> ins)
    {
        ins.Add(Instruction.Create(OpCodes.Box, property.PropertyType));
        ins.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.Object.GetHashcode));
    }

    static void AddNormalCode(PropertyDefinition property, Collection<Instruction> ins, TypeDefinition type)
    {
        ins.If(
            c =>
            {
            },
            t =>
            {
                LoadVariable(property, t, type);
                t.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.Object.GetHashcode));
            },
            f => f.Add(Instruction.Create(OpCodes.Ldc_I4_0)));
    }

    static void AddCollectionCode(PropertyDefinition property, bool isFirst, Collection<Instruction> ins, VariableDefinition resultVariable, MethodDefinition method, TypeDefinition type)
    {
        if (!property.PropertyType.IsValueType)
        {
            ins.If(
                c => LoadVariable(property, c, type),
                t =>
                {
                    GenerateCollectionCode(property, resultVariable, method, type, t);
                },
                f => { });
        }
        else
        {
            GenerateCollectionCode(property, resultVariable, method, type, ins);
        }
    }

    private static void GenerateCollectionCode(PropertyDefinition property, VariableDefinition resultVariable,
        MethodDefinition method, TypeDefinition type, Collection<Instruction> t)
    {
        LoadVariable(property, t, type);
        var enumeratorVariable = method.Body.Variables.Add(ReferenceFinder.IEnumerator.TypeReference);
        var currentVariable = method.Body.Variables.Add(ReferenceFinder.Object.TypeReference);

        GetEnumerator(t, enumeratorVariable, property);

        AddCollectionLoop(resultVariable, t, enumeratorVariable, currentVariable);
    }

    static void AddCollectionLoop(VariableDefinition resultVariable, Collection<Instruction> t, VariableDefinition enumeratorVariable, VariableDefinition currentVariable)
    {
        t.While(
            c =>
            {
                c.Add(Instruction.Create(OpCodes.Ldloc, enumeratorVariable));
                c.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.IEnumerator.MoveNext));
            },
            b =>
            {
                b.Add(Instruction.Create(OpCodes.Ldloc, resultVariable));
                b.Add(Instruction.Create(OpCodes.Ldc_I4, magicNumber));
                b.Add(Instruction.Create(OpCodes.Mul));

                b.Add(Instruction.Create(OpCodes.Ldloc, enumeratorVariable));
                b.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.IEnumerator.GetCurrent));
                b.Add(Instruction.Create(OpCodes.Stloc, currentVariable));

                b.If(
                    bc => b.Add(Instruction.Create(OpCodes.Ldloc, currentVariable)),
                    bt =>
                    {
                        bt.Add(Instruction.Create(OpCodes.Ldloc, currentVariable));
                        bt.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.Object.GetHashcode));
                    },
                    et => et.Add(Instruction.Create(OpCodes.Ldc_I4_0)));
                b.Add(Instruction.Create(OpCodes.Xor));
                b.Add(Instruction.Create(OpCodes.Stloc, resultVariable));
            });
    }

    static void AddResultInit(Collection<Instruction> ins, VariableDefinition resultVariable)
    {
        ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
        ins.Add(Instruction.Create(OpCodes.Stloc, resultVariable));
    }

    static void GetEnumerator(Collection<Instruction> ins, VariableDefinition variable, PropertyDefinition property)
    {
        if (property.PropertyType.IsValueType)
        {
            var imported = ReferenceFinder.ImportCustom(property.PropertyType);
            ins.Add(Instruction.Create(OpCodes.Box, imported));
        }

        ins.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.IEnumerable.GetEnumerator));
        ins.Add(Instruction.Create(OpCodes.Stloc, variable));
    }
}