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
    public static class GetHashCodeInjector
    {
        private const string ignoreAttributeName = "IgnoreDuringEqualsAttribute";

        private const int magicNumber = 397;

        public static MethodDefinition Inject( TypeDefinition type )
        {
            var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
            var method = new MethodDefinition("GetHashCode", methodAttributes, ReferenceFinder.Int32.TypeReference);
            method.CustomAttributes.MarkAsGeneratedCode();

            var resultVariable = method.Body.Variables.Add("result", ReferenceFinder.Int32.TypeReference);

            var body = method.Body;
            var ins = body.Instructions;

            ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
            ins.Add(Instruction.Create(OpCodes.Stloc, resultVariable));

            var properties = type.GetPropertiesWithoutIgnores(ignoreAttributeName);
            if (properties.Length == 0)
            {
                AddResutInit(ins, resultVariable);
            }

            var isFirst = true;
            foreach (var property in properties)
            {
                AddPropertyCode(property, isFirst, ins, resultVariable, method, type);
                isFirst = false;
            }

            AddReturnCode(ins, resultVariable);

            body.OptimizeMacros();

            type.Methods.Add(method);

            return method;
        }

        private static void AddReturnCode(Collection<Instruction> ins, VariableDefinition resultVariable)
        {
            ins.Add(Instruction.Create(OpCodes.Ldloc, resultVariable));
            ins.Add(Instruction.Create(OpCodes.Ret));
        }

        private static void AddPropertyCode(PropertyDefinition property, bool isFirst, Collection<Instruction> ins, VariableDefinition resultVariable, MethodDefinition method, TypeDefinition type)
        {
            bool isCollection;
            TypeReference propType;
            if (property.PropertyType.IsGenericParameter)
            {
                isCollection = false;
                propType = property.PropertyType.GetGenericInstanceType(type);
            }
            else
            {
                var resolved = property.PropertyType.Resolve();
                propType = resolved;
                isCollection = resolved.IsCollection();
            }

            AddMultiplicytyByMagicNumber(isFirst, ins, resultVariable, isCollection);

            if (property.PropertyType.IsValueType || property.PropertyType.IsGenericParameter)
            {
                LoadVariable(property, ins, type);
                AddValueTypeCode(property, ins);
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
                    AddNormalCode(property, ins, method, type);
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
        }

        private static void LoadVariable(PropertyDefinition property, Collection<Instruction> ins, TypeDefinition type)
        {
            var get = property.GetGetMethod(type);
            ins.Add(Instruction.Create(OpCodes.Ldarg_0));
            ins.Add(Instruction.Create(OpCodes.Call, get));
        }

        private static void AddMultiplicytyByMagicNumber(bool isFirst, Collection<Instruction> ins, VariableDefinition resultVariable,
            bool isCollection)
        {
            if (!isFirst && !isCollection)
            {
                ins.Add(Instruction.Create(OpCodes.Ldloc, resultVariable));
                ins.Add(Instruction.Create(OpCodes.Ldc_I4, magicNumber));
                ins.Add(Instruction.Create(OpCodes.Mul));
            }
        }

        private static void AddValueTypeCode(PropertyDefinition property, Collection<Instruction> ins)
        {
            ins.Add(Instruction.Create(OpCodes.Box, property.PropertyType));
            ins.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.Object.GetHashcode));
        }

        private static void AddNormalCode(PropertyDefinition property, Collection<Instruction> ins, MethodDefinition method, TypeDefinition type)
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
                f => { f.Add(Instruction.Create(OpCodes.Ldc_I4_0)); });
        }

        private static void AddCollectionCode(PropertyDefinition property, bool isFirst, Collection<Instruction> ins, VariableDefinition resultVariable, MethodDefinition method, TypeDefinition type)
        {
            if (isFirst)
            {
                ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                ins.Add(Instruction.Create(OpCodes.Stloc, resultVariable));
            }

            ins.If(
                c =>
                {
                    LoadVariable(property, c, type);

                },
                t =>
                {
                    LoadVariable(property, t, type);
                    var enumeratorVariable = method.Body.Variables.Add(property.Name + "Enumarator", ReferenceFinder.IEnumerator.TypeReference);
                    var currentVariable = method.Body.Variables.Add(property.Name + "Current", ReferenceFinder.Object.TypeReference);

                    GetEnumerator(t, enumeratorVariable);

                    AddCollectionLoop(resultVariable, t, enumeratorVariable, currentVariable);
                },
                f => { });
        }

        private static void AddCollectionLoop(VariableDefinition resultVariable, Collection<Instruction> t, VariableDefinition enumeratorVariable, VariableDefinition currentVariable)
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
                        bc => { b.Add(Instruction.Create(OpCodes.Ldloc, currentVariable)); },
                        bt =>
                        {
                            bt.Add(Instruction.Create(OpCodes.Ldloc, currentVariable));
                            bt.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.Object.GetHashcode));
                        },
                        et => { et.Add(Instruction.Create(OpCodes.Ldc_I4_0)); });
                    b.Add(Instruction.Create(OpCodes.Xor));
                    b.Add(Instruction.Create(OpCodes.Stloc, resultVariable));
                });
        }

        private static void AddResutInit(Collection<Instruction> ins, VariableDefinition resultVariable)
        {
            ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
            ins.Add(Instruction.Create(OpCodes.Stloc, resultVariable));
        }

        private static void GetEnumerator(Collection<Instruction> ins, VariableDefinition variable)
        {
            ins.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.IEnumerable.GetEnumerator));
            ins.Add(Instruction.Create(OpCodes.Stloc, variable));
        }
    }
}
