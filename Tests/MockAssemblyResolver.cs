using Mono.Cecil;

public class TestAssemblyResolver : DefaultAssemblyResolver
{
    public TestAssemblyResolver(string searchDirectory) {
        AddSearchDirectory(searchDirectory);
    }
}