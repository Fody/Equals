﻿using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

public partial class ModuleWeaver
{
    public static void InjectEqualityOperator(TypeDefinition type)
    {
        AddOperator(type, true);
    }

    public static void InjectInequalityOperator(TypeDefinition type)
    {
        AddOperator(type, false);
    }

    static void AddOperator(TypeDefinition type, bool isEquality)
    {
        var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Static;
        var method = new MethodDefinition(isEquality ? "op_Equality" : "op_Inequality", methodAttributes, ModuleWeaver.BooleanType);
        method.CustomAttributes.MarkAsGeneratedCode();

        var parameterType = type.GetGenericInstanceType(type);

        method.Parameters.Add("left", parameterType);
        method.Parameters.Add("right", parameterType);

        var body = method.Body;
        var ins = body.Instructions;

        AddStaticEqualsCall(type, ins);
        ins.AddReturnValue(isEquality);

        body.OptimizeMacros();

        type.Methods.AddOrReplace(method);
    }

    static void AddStaticEqualsCall(TypeDefinition type, Collection<Instruction> ins)
    {
        if (type.IsValueType)
        {
            var resolved = type.GetGenericInstanceType(type);
            ins.Add(Instruction.Create(OpCodes.Ldarg_0));
            ins.Add(Instruction.Create(OpCodes.Box, resolved));
            ins.Add(Instruction.Create(OpCodes.Ldarg_1));
            ins.Add(Instruction.Create(OpCodes.Box, resolved));
        }
        else
        {
            ins.Add(Instruction.Create(OpCodes.Ldarg_0));
            ins.Add(Instruction.Create(OpCodes.Ldarg_1));
        }

        ins.Add(Instruction.Create(OpCodes.Call, ModuleWeaver.StaticEquals));
    }
}