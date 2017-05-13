using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Equals.Fody.Extensions
{
    public static class CollectionVariableDefinitionExtension
    {
        public static VariableDefinition Add(this Collection<VariableDefinition> variables, TypeReference type)
        {
            var variable = new VariableDefinition(type);
            variables.Add(variable);
            return variable;
        }
    }
}