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
        var codeBase = Assembly.Load(name.FullName).CodeBase.Replace("file:///", "");
        return AssemblyDefinition.ReadAssembly(codeBase);
    }

    public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
    {

        throw new NotImplementedException();
    }

    public AssemblyDefinition Resolve(string fullName)
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

    public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
    {
        throw new NotImplementedException();
    }

    public string Directory;
}