using System;
using System.Collections.Generic;
using System.Linq;
using Equals.Fody.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

namespace Equals.Fody.Injectors
{
    public static class EqualsInjector
    {
        const string ignoreAttributeName = "IgnoreDuringEqualsAttribute";
        const string customAttribute = "CustomEqualsInternalAttribute";

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

        public static MethodDefinition InjectEqualsObject(TypeDefinition type, TypeReference typeRef, MethodReference newEquals, int typeCheck)
        {
            var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
            var method = new MethodDefinition("Equals", methodAttributes, ReferenceFinder.Boolean.TypeReference);
            method.CustomAttributes.MarkAsGeneratedCode();

            var obj = method.Parameters.Add("obj", ReferenceFinder.Object.TypeReference);

            var body = method.Body;
            body.InitLocals = true;
            var result = body.Variables.Add("result", ReferenceFinder.Boolean.TypeReference);

            var ins = body.Instructions;

            var labelRet = Instruction.Create(OpCodes.Nop);

            AddCheckEqualsReference(type, ins, true);

            ins.If(
                c => AddTypeChecking(type, typeRef, typeCheck, c),
                t => AddInternalEqualsCall(type, typeRef, newEquals, t, result),
                AddReturnFalse);

            AddReturn(ins, labelRet, result);

            body.OptimizeMacros();

            type.Methods.AddOrReplace(method);

            return method;
        }

        public static MethodDefinition InjectEqualsType(TypeDefinition type, TypeReference typeRef, MethodReference newEquals)
        {
            var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
            var method = new MethodDefinition("Equals", methodAttributes, ReferenceFinder.Boolean.TypeReference);
            method.CustomAttributes.MarkAsGeneratedCode();
            var body = method.Body;
            var ins = body.Instructions;

            var other = method.Parameters.Add("other", typeRef);

            AddCheckEqualsReference(type, ins, false);
            AddEqualsTypeReturn(newEquals, ins, type);

            body.OptimizeMacros();
            type.Methods.AddOrReplace(method);

            return method;
        }

        public static MethodReference InjectEqualsInternal(TypeDefinition type, TypeReference typeRef, MethodDefinition collectionEquals)
        {
            var methodAttributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static;
            var method = new MethodDefinition("EqualsInternal", methodAttributes, ReferenceFinder.Boolean.TypeReference);
            method.CustomAttributes.MarkAsGeneratedCode();

            var left = method.Parameters.Add("left", typeRef);
            var right = method.Parameters.Add("right", typeRef);

            var body = method.Body;
            body.InitLocals = true;
            var ins = body.Instructions;

            var properties = type.GetPropertiesWithoutIgnores(ignoreAttributeName);

            foreach (var property in properties)
            {
                AddPropertyCode(type, collectionEquals, property, ins);
            }

            var methods = type.GetMethods();
            var customLogic = methods
                .Where(x => x.CustomAttributes.Any(y => y.AttributeType.Name == customAttribute)).ToArray();

            if (customLogic.Length > 2)
            {
                throw new WeavingException("Only one custom method can be specified.");
            }

            if (customLogic.Length == 1)
            {
                AddCustomLogicCall(type, body, ins, customLogic);
            }

            AddReturnTrue(ins);

            body.OptimizeMacros();

            type.Methods.AddOrReplace(method);

            var methodToCall = new MethodReference(method.Name, method.ReturnType, typeRef);
            foreach (var parameter in method.Parameters)
            {
                methodToCall.Parameters.Add(parameter);
            }

            return methodToCall;
        }

        static void AddCustomLogicCall(TypeDefinition type, MethodBody body, Collection<Instruction> ins, MethodDefinition[] customLogic)
        {
            ins.IfNot(
                c =>
                {
                    var customMethod = ReferenceFinder.ImportCustom(customLogic[0]);

                    var parameters = customMethod.Parameters;
                    if (parameters.Count != 1)
                    {
                        throw new WeavingException(
                            string.Format("Custom equals of type {0} have to have one parameter.", type.FullName));
                    }
                    if (parameters[0].ParameterType.Resolve().FullName != type.FullName)
                    {
                        throw new WeavingException(
                            string.Format("Custom equals of type {0} have to have one parameter of type {0}.", type.FullName));
                    }

                    MethodReference selectedMethod;
                    TypeReference resolverType;
                    if (customMethod.DeclaringType.HasGenericParameters)
                    {
                        var genericInstanceType = type.GetGenericInstanceType(type);
                        resolverType = ReferenceFinder.ImportCustom(genericInstanceType);
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

                    var imported = ReferenceFinder.ImportCustom(selectedMethod);
                    if (!type.IsValueType)
                    {
                        ins.Add(Instruction.Create(OpCodes.Ldarg_0));
                    }
                    else
                    {
                        var argVariable = body.Variables.Add("argVariable", resolverType);
                        ins.Add(Instruction.Create(OpCodes.Ldarg_0));
                        ins.Add(Instruction.Create(OpCodes.Stloc, argVariable));

                        ins.Add(Instruction.Create(OpCodes.Ldloca, argVariable));
                    }
                    ins.Add(Instruction.Create(OpCodes.Ldarg_1));
                    ins.Add(Instruction.Create(imported.GetCallForMethod(), imported));
                },
                AddReturnFalse);
        }

        public static OpCode GetCallForMethod(this MethodReference methodReference)
        {
            if (methodReference.DeclaringType.IsValueType)
            {
                return OpCodes.Call;
            }
            return OpCodes.Callvirt;
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

        static void AddReturnTrue(Collection<Instruction> e)
        {
            e.Add(Instruction.Create(OpCodes.Ldc_I4_1));
            e.Add(Instruction.Create(OpCodes.Ret));
        }

        static void AddReturnFalse(Collection<Instruction> e)
        {
            e.Add(Instruction.Create(OpCodes.Ldc_I4_0));
            e.Add(Instruction.Create(OpCodes.Ret));
        }

        static void AddTypeChecking(TypeDefinition type, TypeReference typeRef, int typeCheck, Collection<Instruction> c)
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

        static void AddExactlyOfType(TypeDefinition type, TypeReference typeRef, Collection<Instruction> c)
        {
            c.Add(Instruction.Create(OpCodes.Ldarg_0));
            if (type.IsValueType)
            {
                var resolved = type.GetGenericInstanceType(type);
                c.Add(Instruction.Create(OpCodes.Ldobj, resolved));
                c.Add(Instruction.Create(OpCodes.Box, resolved));
            }
            c.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.Object.GetType));

            c.Add(Instruction.Create(OpCodes.Ldtoken, typeRef));
            c.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.Type.GetTypeFromHandle));
            c.Add(Instruction.Create(OpCodes.Ceq));
        }

        static void AddExactlyTheSameTypeAsThis(TypeDefinition type, Collection<Instruction> c)
        {
            c.Add(Instruction.Create(OpCodes.Ldarg_0));
            if (type.IsValueType)
            {
                var resolved = type.GetGenericInstanceType(type);
                c.Add(Instruction.Create(OpCodes.Ldobj, resolved));
                c.Add(Instruction.Create(OpCodes.Box, resolved));
            }
            c.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.Object.GetType));

            c.Add(Instruction.Create(OpCodes.Ldarg_1));
            c.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.Object.GetType));

            c.Add(Instruction.Create(OpCodes.Ceq));
        }

        static void AddCheckEqualsReference(TypeDefinition type, Collection<Instruction> ins, bool skipBoxingSecond)
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
                    c.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.Object.ReferenceEquals));
                },
                AddReturnFalse);

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
                    c.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.Object.ReferenceEquals));
                },
                AddReturnTrue);
        }

        static void AddPropertyCode(TypeDefinition type, MethodDefinition collectionEquals, PropertyDefinition property, Collection<Instruction> ins)
        {
            ins.IfNot(
                c =>
                {
                    if (!property.PropertyType.IsGenericParameter)
                    {
                        var propType = property.PropertyType.Resolve();
                        var isCollection = propType.IsCollection();
                        if (simpleTypes.Contains(propType.FullName) || propType.IsEnum)
                        {
                            AddSimpleValueCheck(c, property, type);
                        }
                        else if (!isCollection || propType.FullName == typeof (string).FullName)
                        {
                            AddNormalCheck(type, c, property);
                        }
                        else
                        {
                            AddCollectionCheck(type, collectionEquals, c, property, propType);
                        }
                    }
                    else
                    {
                        var genericType = property.PropertyType.GetGenericInstanceType(type);
                        AddNormalCheck(type, c, property);
                    }
                },
                AddReturnFalse);
        }

        static void AddNormalCheck(TypeDefinition type, Collection<Instruction> c, PropertyDefinition property)
        {
            var genericInstance = new Lazy<TypeReference>(() => property.PropertyType.GetGenericInstanceType(type));
            var getMethodImported = ReferenceFinder.ImportCustom(property.GetGetMethod(type));

            c.Add(Instruction.Create(OpCodes.Ldarg_0));
            c.Add(Instruction.Create(getMethodImported.GetCallForMethod(), getMethodImported));
            if (property.PropertyType.IsValueType || property.PropertyType.IsGenericParameter)
            {
                c.Add(Instruction.Create(OpCodes.Box, genericInstance.Value));
            }
            c.Add(Instruction.Create(OpCodes.Ldarg_1));
            c.Add(Instruction.Create(getMethodImported.GetCallForMethod(), getMethodImported));
            if (property.PropertyType.IsValueType || property.PropertyType.IsGenericParameter)
            {
                c.Add(Instruction.Create(OpCodes.Box, genericInstance.Value));
            }
            c.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.Object.StaticEquals));
        }

        static void AddCollectionCheck(TypeDefinition type, MethodDefinition collectionEquals, Collection<Instruction> c, PropertyDefinition property, TypeDefinition propType)
        {
            c.If(
                AddCollectionFirstArgumentCheck,
                AddCollectionSecondArgumentCheck,
                e => AddCollectionEquals(type, collectionEquals, property, propType, e));
        }

        static void AddCollectionEquals(TypeDefinition type, MethodDefinition collectionEquals, PropertyDefinition property, TypeDefinition propType, Collection<Instruction> e)
        {
            e.If(
                cf =>
                {
                    cf.Add(Instruction.Create(OpCodes.Ldarg_1));
                    cf.Add(Instruction.Create(OpCodes.Ldnull));
                    cf.Add(Instruction.Create(OpCodes.Ceq));
                },
                t => t.Add(Instruction.Create(OpCodes.Ldc_I4_0)),
                es =>
                {
                    var getMethod = property.GetGetMethod(type);
                    var getMethodImported = ReferenceFinder.ImportCustom(getMethod);

                    es.Add(Instruction.Create(OpCodes.Ldarg_0));
                    es.Add(Instruction.Create(getMethodImported.GetCallForMethod(), getMethodImported));
                    if (propType.IsValueType)
                    {
                        es.Add(Instruction.Create(OpCodes.Box, propType));
                    }

                    es.Add(Instruction.Create(OpCodes.Ldarg_1));
                    es.Add(Instruction.Create(getMethodImported.GetCallForMethod(), getMethodImported));
                    if (propType.IsValueType)
                    {
                        es.Add(Instruction.Create(OpCodes.Box, propType));
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

        static void AddSimpleValueCheck(Collection<Instruction> c, PropertyDefinition property, TypeDefinition type)
        {
            var getMethod = property.GetGetMethod(type);
            var getMethodImported = ReferenceFinder.ImportCustom(getMethod);

            c.Add(Instruction.Create(OpCodes.Ldarg_0));
            c.Add(Instruction.Create(getMethodImported.GetCallForMethod(), getMethodImported));

            c.Add(Instruction.Create(OpCodes.Ldarg_1));
            c.Add(Instruction.Create(getMethodImported.GetCallForMethod(), getMethodImported));

            c.Add(Instruction.Create(OpCodes.Ceq));
        }
    }
}