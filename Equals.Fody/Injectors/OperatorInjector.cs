using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

public partial class ModuleWeaver
{
    public void InjectOperator(TypeDefinition type, Operator @operator)
    {
        var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Static;
        var method = new MethodDefinition(@operator.MethodName, methodAttributes, TypeSystem.BooleanReference);
        MarkAsGeneratedCode(method.CustomAttributes);

        var parameterType = type.GetGenericInstanceType(type);

        method.Parameters.Add("left", parameterType);
        method.Parameters.Add("right", parameterType);

        var body = method.Body;
        var ins = body.Instructions;

        AddStaticEqualsCall(type, ins);
        ins.AddReturnValue(@operator.IsEquality);

        body.OptimizeMacros();

        type.Methods.AddOrReplace(method);
    }

    void AddStaticEqualsCall(TypeDefinition type, Collection<Instruction> ins)
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

        ins.Add(Instruction.Create(OpCodes.Call, StaticEquals));
    }
}