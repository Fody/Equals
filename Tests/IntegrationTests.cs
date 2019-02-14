using Fody;

public partial class IntegrationTests
{
#pragma warning disable 618
    static TestResult testResult;
#pragma warning restore 618

    static IntegrationTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
    }
}