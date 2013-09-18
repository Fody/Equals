using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Equals.Fody.Extensions
{
    public static class CollectionVariablerDefinitionExtension
    {
        public static VariableDefinition Add(this Collection<VariableDefinition> variables, string name, TypeReference type)
        {
            var variable = new VariableDefinition(name, type);
            variables.Add(variable);
            return variable;
        }
    }
}