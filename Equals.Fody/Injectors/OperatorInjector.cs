using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

public partial class ModuleWeaver
{
    public void ReplaceOperator(TypeDefinition type, Operator @operator)
    {
        var method = WeavingInstruction.RetrieveOperatorAndAssertHasWeavingInstruction(type, @operator);
        MarkAsGeneratedCode(method.CustomAttributes);

        var body = method.Body;
        var ins = body.Instructions;
        ins.Clear();

        AddStaticEqualsCall(type, ins);
        ins.AddReturnValue(@operator.IsEquality);

        body.OptimizeMacros();
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