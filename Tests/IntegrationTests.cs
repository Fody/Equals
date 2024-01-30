using Fody;
using VerifyXunit;

[UsesVerify]
public partial class IntegrationTests
{
    static IntegrationTests()
    {
        var weaver = new ModuleWeaver();
        testResult = weaver.ExecuteTestRun("AssemblyToProcess.dll");
    }

    static TestResult testResult;
#pragma warning restore 618
}