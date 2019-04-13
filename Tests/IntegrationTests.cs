using Fody;
using Xunit.Abstractions;

public partial class IntegrationTests :
    XunitLoggingBase
{

    static TestResult testResult;
#pragma warning restore 618

    static IntegrationTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
    }

    public IntegrationTests(ITestOutputHelper output) : base(output)
    {
    }
}