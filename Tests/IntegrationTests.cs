using Fody;
using Xunit.Abstractions;

public partial class IntegrationTests :
    XunitLoggingBase
{
    // TODO use xunit features to create this just for the test collection - to ensure we don't fail the bad case tests, too
    static TestResult testResult;
#pragma warning restore 618

    static IntegrationTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
    }

    public IntegrationTests(ITestOutputHelper output) :
        base(output)
    {
    }
}