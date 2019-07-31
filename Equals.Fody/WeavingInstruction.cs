using System;
using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class WeavingInstruction
{
    readonly MethodDefinition weaveMethodWithoutParameters;
    readonly MethodDefinition weaveMethodWithParameters;

    public WeavingInstruction(IReadOnlyList<MethodDefinition> weaveMethods)
    {
        weaveMethodWithoutParameters = weaveMethods.Single(x => !x.Parameters.Any());
        weaveMethodWithParameters = weaveMethods.Single(x => x.Parameters.Count == 2);
    }

    public MethodDefinition RetrieveOperatorAndAssertHasWeavingInstruction(TypeDefinition type, Operator @operator)
    {
        if (!@operator.TryGetOperator(type, out var operatorMethod))
        {
            throw CreateException(
                $"Type {type.Name} marked with the [Equals] attribute does not contain {@operator.MethodName}. Fix this by adding a method {@operator.MethodSourceExample} or, if you don't want the operator to be woven: set `[Equals].DoNotAddEqualityOperators = true`.");
        }

        if (!IsWeavingInstruction(operatorMethod))
        {
            throw CreateException(
                $"Type {type.Name} marked with the [Equals] attribute contains {@operator.MethodName}, but it does not contain the instruction to weave it. Either set implement the method like {@operator.MethodSourceExample} or, if you don't want the operator to be woven: set `[Equals].DoNotAddEqualityOperators = true`.");
        }

        return operatorMethod;
    }

    public void AssertNotHasWeavingInstruction(TypeDefinition type, Operator @operator)
    {
        if (@operator.TryGetOperator(type, out var operatorMethod))
        {
            if (IsWeavingInstruction(operatorMethod))
            {
                throw CreateException(
                    $"Type {type.Name} marked with [Equals(DoNotAddEqualityOperators = true)] contains {@operator.MethodName} with the instruction to weave it. Either set `DoNotAddEqualityOperators` to `false` or implement the operator properly.");
            }
        }
    }

    bool IsWeavingInstruction(MethodDefinition method)
    {
        var instructions = method.Body.Instructions;

        switch (instructions.Count)
        {
            case 2:
                return IsWeavingInstruction(instructions[0], weaveMethodWithoutParameters);
            case 4:
                return IsWeavingInstruction(instructions[2], weaveMethodWithParameters);
            default:
                return false;
        }
    }

    bool IsWeavingInstruction(Instruction instruction, MethodDefinition weavingMethod)
        => instruction.OpCode == OpCodes.Call &&
           IsOperatorWeaveTarget(instruction.Operand, weavingMethod);

    bool IsOperatorWeaveTarget(object operand, MethodDefinition weavingMethod)
    {
        if (operand is MethodReference method)
        {
            return method.Resolve() == weavingMethod;
        }

        return false;
    }

    static WeavingException CreateException(FormattableString message)
    {
        return new WeavingException(FormattableString.Invariant(message));
    }
}
