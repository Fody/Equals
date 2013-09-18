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

        public static void Inject(TypeDefinition type)
        {
            var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
            var method = new MethodDefinition("GetHashCode", methodAttributes, ReferenceFinder.Int32.TypeReference);

            var resultVariable = method.Body.Variables.Add("result", ReferenceFinder.Int32.TypeReference);

            var body = method.Body;
            var ins = body.Instructions;

            var properties = type.GetPropertiesWithoutIgnores(ignoreAttributeName);
            if (properties.Length == 0)
            {
                AddResutInit(ins, resultVariable);
            }

            var isFirst = true;
            foreach (var property in properties)
            {
                AddPropertyCode(property, isFirst, ins, resultVariable, method);
                isFirst = false;
            }

            AddReturnCode(ins, resultVariable);

            body.OptimizeMacros();

            type.Methods.Add(method);
        }

        private static void AddReturnCode(Collection<Instruction> ins, VariableDefinition resultVariable)
        {
            ins.Add(Instruction.Create(OpCodes.Ldloc, resultVariable));
            ins.Add(Instruction.Create(OpCodes.Ret));
        }

        private static void AddPropertyCode(PropertyDefinition property, bool isFirst, Collection<Instruction> ins, VariableDefinition resultVariable, MethodDefinition method)
        {
            var propType = property.PropertyType.Resolve();
            var isCollection = propType.IsCollection();

            AddMultiplicytyByMagicNumber(isFirst, ins, resultVariable, isCollection);

            var get = property.GetGetMethod();
            ins.Add(Instruction.Create(OpCodes.Ldarg_0));
            ins.Add(Instruction.Create(OpCodes.Call, get));

            if (property.PropertyType.IsValueType)
            {
                AddValueTypeCOde(property, ins);
            }
            else
            {
                if (isCollection)
                {
                    AddCollectionCode(property, isFirst, ins, resultVariable, method);
                }
                else
                {
                    AddNormalCode(property, ins, method);
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

        private static void AddValueTypeCOde(PropertyDefinition property, Collection<Instruction> ins)
        {
            ins.Add(Instruction.Create(OpCodes.Box, property.PropertyType));
            ins.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.Object.GetHashcode));
        }

        private static void AddNormalCode(PropertyDefinition property, Collection<Instruction> ins, MethodDefinition method)
        {
            var variable = new VariableDefinition(property.PropertyType);
            method.Body.Variables.Add(variable);
            ins.If(
                c =>
                {
                    c.Add(Instruction.Create(OpCodes.Stloc, variable));
                    c.Add(Instruction.Create(OpCodes.Ldloc, variable));
                },
                t =>
                {
                    t.Add(Instruction.Create(OpCodes.Ldloc, variable));
                    t.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.Object.GetHashcode));
                },
                f => { f.Add(Instruction.Create(OpCodes.Ldc_I4_0)); });
        }

        private static void AddCollectionCode(PropertyDefinition property, bool isFirst, Collection<Instruction> ins, VariableDefinition resultVariable, MethodDefinition method)
        {
            var variable = new VariableDefinition(property.Name, property.PropertyType);
            method.Body.Variables.Add(variable);
            ins.Add(Instruction.Create(OpCodes.Stloc, variable));

            if (isFirst)
            {
                ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                ins.Add(Instruction.Create(OpCodes.Stloc, resultVariable));
            }

            ins.If(
                c => { c.Add(Instruction.Create(OpCodes.Ldloc, variable)); },
                t =>
                {
                    t.Add(Instruction.Create(OpCodes.Ldloc, variable));
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
