using System;
using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Equals.Fody
{
    static class WeavingInstruction
    {
        private static readonly Instruction NotWeavingInstruction = Instruction.Create(OpCodes.Throw);

        public static void AssertHasWeavingInstruction(TypeDefinition type)
        {
            AssertHasWeavingInstruction(type, Operator.Equality);
            AssertHasWeavingInstruction(type, Operator.Inequality);
        }

        static void AssertHasWeavingInstruction(TypeDefinition type, Operator @operator)
        {
            if (!@operator.TryGetOperator(type, out var operatorMethod))
            {
                throw CreateException(
                    $"Type {type.Name} marked with the [Equals] attribute does not contain {@operator.MethodName}. Fix this by adding a method {@operator.MethodSourceExample} or, if you don't want the operator to be woven: set `[Equals].DoNotAddEqualityOperators = true`.");
            }

            if (!IsWeavingInstruction(operatorMethod))
            {
                throw CreateException(
                    $"TType {type.Name} marked with the [Equals] attribute contains {@operator.MethodName}, but it does not contain the instruction to weave it. Either set implement the method like {@operator.MethodSourceExample} or, if you don't want the operator to be woven: set `[Equals].DoNotAddEqualityOperators = true`.");
            }
        }

        public static void AssertNotHasWeavingInstruction(TypeDefinition type)
        {
            AssertNotHasWeavingInstruction(type, Operator.Equality);
            AssertNotHasWeavingInstruction(type, Operator.Inequality);
        }

        static void AssertNotHasWeavingInstruction(TypeDefinition type, Operator @operator)
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

        static bool IsWeavingInstruction(MethodDefinition method)
        {
            var firstInstruction = method.Body.Instructions.FirstOrDefault() ?? NotWeavingInstruction;
            return IsWeavingInstruction(firstInstruction);
        }

        static bool IsWeavingInstruction(Instruction instruction) =>
            instruction.OpCode == OpCodes.Call &&
            IsOperatorWeaveTarget(instruction.Operand);

        static bool IsOperatorWeaveTarget(object operand)
        {
            if (operand is MethodReference method)
            {
                return method.DeclaringType.Name == "Operator" && method.Name == "Weave";
            }

            return false;
        }

        static WeavingException CreateException(FormattableString message)
        {
            return new WeavingException(FormattableString.Invariant(message));
        }
    }
}