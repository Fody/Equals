using Mono.Cecil;
using Mono.Collections.Generic;

public static class CollectionParameterDefinitionExtensions
{
    public static ParameterDefinition Add(this Collection<ParameterDefinition> parameters, string name, TypeReference type)
    {
        var parameter = new ParameterDefinition(name, ParameterAttributes.None, type);
        parameters.Add(parameter);
        return parameter;
    }
}