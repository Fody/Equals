using System.Linq;
using Equals.Fody.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

namespace Equals.Fody.Injectors
{
    public static class GetHashCodeInjector
    {
        const string ignoreAttributeName = "IgnoreDuringEqualsAttribute";

        const int magicNumber = 397;

        public static MethodDefinition Inject(TypeDefinition type)
        {
            var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
            var method = new MethodDefinition("GetHashCode", methodAttributes, ReferenceFinder.Int32.TypeReference);
            method.CustomAttributes.MarkAsGeneratedCode();

            var resultVariable = method.Body.Variables.Add("result", ReferenceFinder.Int32.TypeReference);

            var body = method.Body;
            body.InitLocals = true;
            var ins = body.Instructions;

            ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
            ins.Add(Instruction.Create(OpCodes.Stloc, resultVariable));

            var properties = ReferenceFinder.ImportCustom(type).Resolve().GetPropertiesWithoutIgnores(ignoreAttributeName);
            if (properties.Length == 0)
            {
                AddResultInit(ins, resultVariable);
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

            AddReturnCode(ins, resultVariable);

            body.OptimizeMacros();

            type.Methods.AddOrReplace(method);

            return method;
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
                if (isCollection)
                {
                    AddCollectionCode(property, isFirst, ins, resultVariable, method, type);
                }
                else
                {
                    LoadVariable(property, ins, type);
                    AddNormalCode(property, ins, type);
                }
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
            if (isFirst)
            {
                ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                ins.Add(Instruction.Create(OpCodes.Stloc, resultVariable));
            }

            ins.If(
                c => LoadVariable(property, c, type),
                t =>
                {
                    LoadVariable(property, t, type);
                    var enumeratorVariable = method.Body.Variables.Add(property.Name + "Enumerator", ReferenceFinder.IEnumerator.TypeReference);
                    var currentVariable = method.Body.Variables.Add(property.Name + "Current", ReferenceFinder.Object.TypeReference);

                    GetEnumerator(t, enumeratorVariable);

                    AddCollectionLoop(resultVariable, t, enumeratorVariable, currentVariable);
                },
                f => { });
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

        static void GetEnumerator(Collection<Instruction> ins, VariableDefinition variable)
        {
            ins.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.IEnumerable.GetEnumerator));
            ins.Add(Instruction.Create(OpCodes.Stloc, variable));
        }
    }
}