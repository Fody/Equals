using System;
using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Equals.Fody
{
    public class WeavingInstruction
    {
        static Instruction NotWeavingInstruction = Instruction.Create(OpCodes.Throw);
        MethodDefinition weaveMethod;

        public WeavingInstruction(MethodDefinition weaveMethod)
        {
            this.weaveMethod = weaveMethod;
        }

        public void AssertHasWeavingInstruction(TypeDefinition type)
        {
            AssertHasWeavingInstruction(type, Operator.Equality);
            AssertHasWeavingInstruction(type, Operator.Inequality);
        }

        void AssertHasWeavingInstruction(TypeDefinition type, Operator @operator)
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

        public void AssertNotHasWeavingInstruction(TypeDefinition type)
        {
            AssertNotHasWeavingInstruction(type, Operator.Equality);
            AssertNotHasWeavingInstruction(type, Operator.Inequality);
        }

        void AssertNotHasWeavingInstruction(TypeDefinition type, Operator @operator)
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
            var firstInstruction = method.Body.Instructions.FirstOrDefault() ?? NotWeavingInstruction;
            return IsWeavingInstruction(firstInstruction);
        }

        bool IsWeavingInstruction(Instruction instruction) =>
            instruction.OpCode == OpCodes.Call &&
            IsOperatorWeaveTarget(instruction.Operand);

        bool IsOperatorWeaveTarget(object operand)
        {
            if (operand is MethodReference method)
            {
                return method.Resolve() == weaveMethod;
            }

            return false;
        }

        static WeavingException CreateException(FormattableString message)
        {
            return new WeavingException(FormattableString.Invariant(message));
        }
    }
}
