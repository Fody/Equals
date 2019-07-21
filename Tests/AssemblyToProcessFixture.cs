using Fody;

public class AssemblyToProcessFixture
{
    public AssemblyToProcessFixture()
    {
        var weavingTask = new ModuleWeaver();
        TestResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
    }

    public TestResult TestResult { get; }
}