using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Equals.Fody.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

namespace Equals.Fody.Injectors
{
    public static class EqualsInjector
    {
        private const string ignoreAttributeName = "IgnoreDuringEqualsAttribute";

        private const int ExaclyTheSameTypeAsThis = 0;
        private const int ExaclyOfType = 1;
        private const int EqualsOrSubtype = 2;

        private static ISet<string> simpleTypes = new HashSet<string>(new[]
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
            var result = body.Variables.Add("result", ReferenceFinder.Boolean.TypeReference);

            var ins = body.Instructions;

            var labelRet = Instruction.Create(OpCodes.Nop);

            AddCheckEqualsReference(type, ins, result, labelRet);

            ins.If(
                c => AddTypeChecking(type, typeRef, typeCheck, c),
                t => AddInternalEqualsCall(type, typeRef, newEquals, t, result),
                e => AddReturnFalse(e, result));

            AddReturn(ins, labelRet, result);

            body.OptimizeMacros();

            type.Methods.Add(method);

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
            var result = body.Variables.Add("result", ReferenceFinder.Boolean.TypeReference);

            var labelRet = Instruction.Create(OpCodes.Nop);

            AddCheckEqualsReference(type, ins, result, labelRet);

            AddEqualsTypeReturn(newEquals, ins, result, labelRet);

            body.OptimizeMacros();
            type.Methods.Add(method);

            return method;
        }

        public static MethodReference InjectEqualsInternal( TypeDefinition type, TypeReference typeRef, MethodDefinition collectionEquals )
        {
            var methodAttributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static;
            var method = new MethodDefinition("EqualsInternal", methodAttributes, ReferenceFinder.Boolean.TypeReference);
            method.CustomAttributes.MarkAsGeneratedCode();

            var left = method.Parameters.Add("left", typeRef);
            var right = method.Parameters.Add("right", typeRef);

            var body = method.Body;
            var ins = body.Instructions;

            var result = body.Variables.Add( "result", ReferenceFinder.Boolean.TypeReference );
            var labelRet = Instruction.Create( OpCodes.Nop );

            var properties = type.GetPropertiesWithoutIgnores(ignoreAttributeName);

            ins.Add( Instruction.Create( OpCodes.Ldc_I4_1 ) );
            ins.Add( Instruction.Create( OpCodes.Stloc, result ) );

            foreach (var property in properties)
            {
                AddPropertyCode( type, collectionEquals, property, ins, result , labelRet);
            }

            AddReturn( ins, labelRet, result );

            body.OptimizeMacros();

            type.Methods.Add(method);

            var methodToCall = new MethodReference(method.Name, method.ReturnType, typeRef);
            foreach (var parameter in method.Parameters)
            {
                methodToCall.Parameters.Add(parameter);
            }

            return methodToCall;
        }

        private static void AddEqualsTypeReturn(MethodReference newEquals, Collection<Instruction> ins, VariableDefinition result, Instruction labelRet)
        {
            ins.Add(Instruction.Create(OpCodes.Ldarg_0));
            ins.Add(Instruction.Create(OpCodes.Ldarg_1));
            ins.Add(Instruction.Create(OpCodes.Call, newEquals));
            ins.Add(Instruction.Create(OpCodes.Stloc, result));

            ins.Add(labelRet);
            ins.Add(Instruction.Create(OpCodes.Ldloc, result));
            ins.Add(Instruction.Create(OpCodes.Ret));
        }

        private static void AddInternalEqualsCall(TypeDefinition type, TypeReference typeRef, MethodReference newEquals, Collection<Instruction> t, VariableDefinition result)
        {
            t.Add(Instruction.Create(OpCodes.Ldarg_0));
            if (type.IsValueType)
            {
                t.Add(Instruction.Create(OpCodes.Box, type));
            }

            t.Add(Instruction.Create(OpCodes.Ldarg_1));
            t.Add(Instruction.Create(OpCodes.Castclass, typeRef));

            t.Add(Instruction.Create(OpCodes.Call, newEquals));

            t.Add(Instruction.Create(OpCodes.Stloc, result));
        }

        private static void AddReturn(Collection<Instruction> ins, Instruction labelRet, VariableDefinition result)
        {
            ins.Add(labelRet);
            ins.Add(Instruction.Create(OpCodes.Ldloc, result));
            ins.Add(Instruction.Create(OpCodes.Ret));
        }

        private static void AddReturnFalse(Collection<Instruction> e, VariableDefinition result)
        {
            e.Add(Instruction.Create(OpCodes.Ldc_I4_0));
            e.Add(Instruction.Create(OpCodes.Stloc, result));
        }

        private static void AddTypeChecking(TypeDefinition type, TypeReference typeRef, int typeCheck, Collection<Instruction> c)
        {
            switch (typeCheck)
            {
                case ExaclyTheSameTypeAsThis:
                    AddExaclyTheSameTypeAsThis(type, c);
                    break;

                case ExaclyOfType:
                    AddExaclyOfType(type, typeRef, c);
                    break;

                case EqualsOrSubtype:
                    AddEqualsOrSubtype(typeRef, c);
                    break;

                default:
                    throw new WeavingException("Unknown TypeCheck: " + type);
            }
        }

        private static void AddEqualsOrSubtype(TypeReference typeRef, Collection<Instruction> c)
        {
            c.Add(Instruction.Create(OpCodes.Ldarg_1));
            c.Add(Instruction.Create(OpCodes.Isinst, typeRef));
        }

        private static void AddExaclyOfType(TypeDefinition type, TypeReference typeRef, Collection<Instruction> c)
        {
            c.Add(Instruction.Create(OpCodes.Ldarg_0));
            if (type.IsValueType)
            {
                c.Add(Instruction.Create(OpCodes.Box, type));
            }
            c.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.Object.GetType));

            c.Add(Instruction.Create(OpCodes.Ldtoken, typeRef));
            c.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.Type.GetTypeFromHandle));
            c.Add(Instruction.Create(OpCodes.Ceq));
        }

        private static void AddExaclyTheSameTypeAsThis(TypeDefinition type, Collection<Instruction> c)
        {
            c.Add(Instruction.Create(OpCodes.Ldarg_0));
            if (type.IsValueType)
            {
                c.Add(Instruction.Create(OpCodes.Box, type));
            }
            c.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.Object.GetType));

            c.Add(Instruction.Create(OpCodes.Ldarg_1));
            c.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.Object.GetType));

            c.Add(Instruction.Create(OpCodes.Ceq));
        }

        private static void AddCheckEqualsReference(TypeDefinition type, Collection<Instruction> ins, VariableDefinition result, Instruction resultReturn)
        {
            ins.If(
                c =>
                {
                    c.Add(Instruction.Create(OpCodes.Ldnull));
                    c.Add(Instruction.Create(OpCodes.Ldarg_1));
                    c.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.Object.ReferenceEquals));
                },
                t =>
                {
                    t.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                    t.Add(Instruction.Create(OpCodes.Stloc, result));
                    t.Add(Instruction.Create(OpCodes.Br, resultReturn));
                });

            ins.If(
                c =>
                {
                    c.Add(Instruction.Create(OpCodes.Ldarg_0));
                    if (type.IsValueType)
                    {
                        c.Add(Instruction.Create(OpCodes.Box, type));
                    }
                    c.Add(Instruction.Create(OpCodes.Ldarg_1));
                    c.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.Object.ReferenceEquals));
                },
                t =>
                {
                    t.Add(Instruction.Create(OpCodes.Ldc_I4_1));
                    t.Add(Instruction.Create(OpCodes.Stloc, result));
                    t.Add(Instruction.Create(OpCodes.Br, resultReturn));
                });
        }

        private static void AddPropertyCode(TypeDefinition type, MethodDefinition collectionEquals, PropertyDefinition property, Collection<Instruction> ins, VariableDefinition result, Instruction ret)
        {
            var propType = property.PropertyType.Resolve();
            var isCollection = propType.IsCollection();

            ins.IfNot(
                c =>
                {
                    if (simpleTypes.Contains(propType.FullName) || propType.IsEnum)
                    {
                        AddSimpleValueCheck(c, property, type);
                    }
                    else if (!isCollection || propType.FullName == typeof (string).FullName)
                    {
                        AddNormalCheck(type, c, property, propType);
                    }
                    else
                    {
                        AddCollectionCheck(type, collectionEquals, c, property, propType);
                    }
                },
                t =>
                {
                    t.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                    t.Add( Instruction.Create( OpCodes.Stloc, result ) );
                    t.Add(Instruction.Create(OpCodes.Br, ret));
                });
        }

        private static void AddNormalCheck(TypeDefinition type, Collection<Instruction> c, PropertyDefinition property, TypeDefinition propType)
        {
            c.Add(Instruction.Create(OpCodes.Ldarg_0));
            c.Add(Instruction.Create(OpCodes.Callvirt, property.GetGetMethod(type)));
            c.Add(Instruction.Create(OpCodes.Ldarg_1));
            c.Add(Instruction.Create(OpCodes.Callvirt, property.GetGetMethod(type)));
            c.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.Object.StaticEquals));
        }

        private static void AddCollectionCheck(TypeDefinition type, MethodDefinition collectionEquals, Collection<Instruction> c, PropertyDefinition property, TypeDefinition propType)
        {
            c.If(
                cf => AddCollectionFirstArgumentCheck(cf),
                t => AddCollectionSecondArgumentCheck(t),
                e => AddCollectionEquals(type, collectionEquals, property, propType, e));
        }

        private static void AddCollectionEquals(TypeDefinition type, MethodDefinition collectionEquals, PropertyDefinition property, TypeDefinition propType, Collection<Instruction> e)
        {
            e.If(
                cf =>
                {
                    cf.Add(Instruction.Create(OpCodes.Ldarg_1));
                    cf.Add(Instruction.Create(OpCodes.Ldnull));
                    cf.Add(Instruction.Create(OpCodes.Ceq));
                },
                t => { t.Add(Instruction.Create(OpCodes.Ldc_I4_0)); },
                es =>
                {
                    es.Add(Instruction.Create(OpCodes.Ldarg_0));
                    es.Add(Instruction.Create(OpCodes.Callvirt, property.GetGetMethod(type)));
                    if (propType.IsValueType)
                    {
                        es.Add( Instruction.Create( OpCodes.Box, propType ) );
                    }

                    es.Add(Instruction.Create(OpCodes.Ldarg_1));
                    es.Add(Instruction.Create(OpCodes.Callvirt, property.GetGetMethod(type)));
                    if (propType.IsValueType)
                    {
                        es.Add( Instruction.Create( OpCodes.Box, propType ) );
                    }

                    es.Add(Instruction.Create(OpCodes.Call, collectionEquals));
                });
        }

        private static void AddCollectionSecondArgumentCheck(Collection<Instruction> t)
        {
            t.If(
                ct =>
                {
                    ct.Add(Instruction.Create(OpCodes.Ldarg_1));
                    ct.Add(Instruction.Create(OpCodes.Ldnull));
                    ct.Add(Instruction.Create(OpCodes.Ceq));
                },
                tt => { tt.Add(Instruction.Create(OpCodes.Ldc_I4_1)); },
                ft => { ft.Add(Instruction.Create(OpCodes.Ldc_I4_0)); });
        }

        private static void AddCollectionFirstArgumentCheck(Collection<Instruction> cf)
        {
            cf.Add(Instruction.Create(OpCodes.Ldarg_0));
            cf.Add(Instruction.Create(OpCodes.Ldnull));
            cf.Add(Instruction.Create(OpCodes.Ceq));
        }

        private static void AddSimpleValueCheck(Collection<Instruction> c, PropertyDefinition property, TypeDefinition type)
        {
            c.Add(Instruction.Create(OpCodes.Ldarg_0));
            c.Add(Instruction.Create(OpCodes.Callvirt, property.GetGetMethod(type)));

            c.Add(Instruction.Create(OpCodes.Ldarg_1));
            c.Add(Instruction.Create(OpCodes.Callvirt, property.GetGetMethod(type)));

            c.Add(Instruction.Create(OpCodes.Ceq));
        }
    }
}
