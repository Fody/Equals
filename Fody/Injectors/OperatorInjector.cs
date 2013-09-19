using Equals.Fody.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

namespace Equals.Fody.Injectors
{
    public static class OperatorInjector
    {
        public static MethodDefinition InjectEqualityOperator( TypeDefinition type )
        {
            return AddOperator(type, true);
        }

        public static MethodDefinition InjectInequalityOperator( TypeDefinition type )
        {
            return  AddOperator(type, false);
        }

        private static MethodDefinition AddOperator( TypeDefinition type, bool isEquality )
        {
            var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Static;
            var method = new MethodDefinition(isEquality ? "op_Equality" : "op_Inequality", methodAttributes, ReferenceFinder.Boolean.TypeReference);
            method.CustomAttributes.MarkAsGeneratedCode();

            method.Parameters.Add("left", type);
            method.Parameters.Add("right", type);

            var body = method.Body;
            var ins = body.Instructions;

            AddStaticEqualsCall(type, ins);
            AddReturnValue(isEquality, ins);

            body.OptimizeMacros();

            type.Methods.Add(method);

            return method;
        }

        private static void AddReturnValue(bool isEquality, Collection<Instruction> ins)
        {
            if (!isEquality)
            {
                AddNegateValue(ins);
            }

            ins.Add(Instruction.Create(OpCodes.Ret));
        }

        private static void AddNegateValue(Collection<Instruction> ins)
        {
            ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
            ins.Add(Instruction.Create(OpCodes.Ceq));
        }

        private static void AddStaticEqualsCall(TypeDefinition type, Collection<Instruction> ins)
        {
            if (type.IsValueType)
            {
                ins.Add(Instruction.Create(OpCodes.Ldarg_0));
                ins.Add(Instruction.Create(OpCodes.Box, type));
                ins.Add(Instruction.Create(OpCodes.Ldarg_1));
                ins.Add(Instruction.Create(OpCodes.Box, type));
            }
            else
            {
                ins.Add(Instruction.Create(OpCodes.Ldarg_0));
                ins.Add(Instruction.Create(OpCodes.Ldarg_1));
            }

            ins.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.Object.StaticEquals));
        }
    }
}
