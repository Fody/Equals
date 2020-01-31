using Fody;
using VerifyXunit;
using Xunit.Abstractions;

public partial class IntegrationTests :
    VerifyBase
{
    static IntegrationTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
    }

    static TestResult testResult;
#pragma warning restore 618

    public IntegrationTests(ITestOutputHelper output) :
        base(output)
    {
    }
}