using Fody;
using VerifyXunit;

[UsesVerify]
public partial class IntegrationTests
{
    static IntegrationTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
    }

    static TestResult testResult;
#pragma warning restore 618
}