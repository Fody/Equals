using System;
using System.Linq;
using Mono.Cecil;

public class Operator
{
    public static readonly Operator Equality = new Operator("op_Equality", true, "==");
    public static readonly Operator Inequality = new Operator("op_Inequality", false, "!=");

    Operator(string methodName, bool isEquality, string sourceMethodName)
    {
        MethodName = methodName;
        IsEquality = isEquality;
        MethodSourceExample =
            FormattableString.Invariant(
                $"`public static bool operator {sourceMethodName}(T left, T right) => Operator.Weave(left, right);`");
    }

    public string MethodName { get; }
    public bool IsEquality { get; }
    public string MethodSourceExample { get; }

    public bool TryGetOperator(TypeDefinition type, out MethodDefinition operatorMethod)
    {
        operatorMethod = type.Methods.SingleOrDefault(x => x.Name == MethodName);
        return operatorMethod != null;
    }
}