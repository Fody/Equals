using System.Linq;
using Equals.Fody.Extensions;
using Mono.Cecil;

namespace Equals.Fody
{
    public static class ReferenceFinder
    {
        public static class Type
        {
            public static TypeReference TypeReference;
            public static MethodReference GetTypeFromHandle;
        }

        public static class Object
        {
            public static TypeReference TypeReference;
            public static MethodReference StaticEquals;
            public static MethodReference ReferenceEquals;
            public static MethodReference GetType;
            public static MethodReference GetHashcode;
        }

        public static class Boolean
        {
            public static TypeReference TypeReference;
        }

        public static class Int32
        {
            public static TypeReference TypeReference;
        }

        public static class IEnumerable
        {
            public static TypeReference TypeReference;
            public static MethodReference GetEnumerator;
        }

        public static class IEnumerator
        {
            public static TypeReference TypeReference;
            public static MethodReference MoveNext;
            public static MethodReference GetCurrent;
        }

        public static class IEquatable
        {
            public static TypeReference TypeReference;
        }

        public static void FindReferences(IAssemblyResolver assemblyResolver, ModuleDefinition moduleDefinition)
        {
            var baseLib = assemblyResolver.Resolve("mscorlib");
            var baseLibTypes = baseLib.MainModule.Types;

            var winrt = !baseLibTypes.Any(type => type.Name == "Object");
            if (winrt)
            {
                baseLib = assemblyResolver.Resolve("System.Runtime");
                baseLibTypes = baseLib.MainModule.Types;
            }

            Boolean.TypeReference = moduleDefinition.Import(baseLibTypes.First(t => t.Name == "Boolean"));
            
            Int32.TypeReference = moduleDefinition.Import(baseLibTypes.First(t => t.Name == "Int32"));

            Type.TypeReference = moduleDefinition.Import(baseLibTypes.First(t => t.Name == "Type"));
            Type.GetTypeFromHandle = moduleDefinition.Import(Type.TypeReference.Resolve().FindMethod("GetTypeFromHandle", "RuntimeTypeHandle"));

            Object.TypeReference = moduleDefinition.Import(baseLibTypes.First(t => t.Name == "Object"));
            Object.GetHashcode = moduleDefinition.Import(Object.TypeReference.Resolve().FindMethod("GetHashCode"));
            Object.GetType = moduleDefinition.Import(Object.TypeReference.Resolve().FindMethod("GetType"));
            Object.StaticEquals = moduleDefinition.Import(Object.TypeReference.Resolve().FindMethod("Equals", "Object", "Object"));
            Object.ReferenceEquals = moduleDefinition.Import(Object.TypeReference.Resolve().FindMethod("ReferenceEquals", "Object", "Object"));

            IEnumerable.TypeReference = moduleDefinition.Import(baseLibTypes.First(t => t.Name == "IEnumerable"));
            IEnumerable.GetEnumerator = moduleDefinition.Import(IEnumerable.TypeReference.Resolve().FindMethod("GetEnumerator"));

            IEnumerator.TypeReference = moduleDefinition.Import(baseLibTypes.First(t => t.Name == "IEnumerator"));
            IEnumerator.MoveNext = moduleDefinition.Import(IEnumerator.TypeReference.Resolve().FindMethod("MoveNext"));
            IEnumerator.GetCurrent = moduleDefinition.Import(IEnumerator.TypeReference.Resolve().FindMethod("get_Current"));

            IEquatable.TypeReference = moduleDefinition.Import(baseLibTypes.First(t => t.Name == "IEquatable`1"));
        }
    }
}
