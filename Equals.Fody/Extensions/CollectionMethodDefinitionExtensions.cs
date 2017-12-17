using System.Linq;
using Mono.Collections.Generic;
using Mono.Cecil;

public static class CollectionMethodDefinitionExtensions
{
    public static void AddOrReplace(this Collection<MethodDefinition> methods, MethodDefinition method)
    {
        var current = methods.Where(x => x.Name == method.Name)
            .FirstOrDefault(x => x.Parameters.Count == method.Parameters.Count &&
                                 x.Parameters.Zip(method.Parameters, (a, b) => new {First = a, Second = b})
                                     .All(y => y.First.ParameterType.FullName == y.Second.ParameterType.FullName));

        if (current != null)
        {
            methods.Remove(current);
        }

        methods.Add(method);
    }
}