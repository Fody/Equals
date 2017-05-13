using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using Mono.Cecil;

public class MockAssemblyResolver : IAssemblyResolver
{
    public AssemblyDefinition Resolve(AssemblyNameReference name)
    {
        var fileName = Path.Combine(Directory, name.Name) + ".dll";
        if (File.Exists(fileName))
        {
            return AssemblyDefinition.ReadAssembly(fileName);
        }
        return Resolve(name.Name);
    }

    public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
    {
        throw new NotImplementedException();
    }

    AssemblyDefinition Resolve(string fullName)
    {
        string codeBase;
        if (fullName == "System")
        {
            codeBase = typeof(GeneratedCodeAttribute).Assembly.CodeBase;
        }
        else
        {
            codeBase = Assembly.Load(fullName).CodeBase;
        }

        var file = codeBase.Replace("file:///", "");
        return AssemblyDefinition.ReadAssembly(file);
    }

    public void Dispose()
    {
    }

    public string Directory;
}