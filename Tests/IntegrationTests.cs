public partial class IntegrationTests
{
    static IntegrationTests()
    {
        var weaver = new ModuleWeaver();
        testResult = weaver.ExecuteTestRun("AssemblyToProcess.dll");
    }

    static Fody.TestResult testResult;
#pragma warning restore 618
}