using Mono.Cecil;
using Mono.Cecil.Cil;

public static class MethodReferenceExtensions
{
    public static bool IsMatch(this MethodReference methodReference, params string[] paramTypes)
    {
        if (methodReference.Parameters.Count != paramTypes.Length)
        {
            return false;
        }

        for (var index = 0; index < methodReference.Parameters.Count; index++)
        {
            var parameterDefinition = methodReference.Parameters[index];
            var paramType = paramTypes[index];

            if (parameterDefinition.ParameterType.ContainsGenericParameter)
            {
                // Have to get the REAL type
            }

            if (parameterDefinition.ParameterType.Name != paramType)
            {
                return false;
            }
        }

        return true;
    }

    public static OpCode GetCallForMethod(this MethodReference methodReference)
    {
        // TODO: Aren't there other cases where Call can be preferred for perf? Like sealed classes?
        if (methodReference.DeclaringType.IsValueType)
        {
            return OpCodes.Call;
        }
        return OpCodes.Callvirt;
    }
}