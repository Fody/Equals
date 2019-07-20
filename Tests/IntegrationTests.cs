using Fody;
using Xunit;
using Xunit.Abstractions;

public partial class IntegrationTests :
    XunitLoggingBase,
    IClassFixture<AssemblyToProcessFixture>
{
    TestResult testResult;
#pragma warning restore 618

    public IntegrationTests(ITestOutputHelper output, AssemblyToProcessFixture fixture) :
        base(output)
    {
        testResult = fixture.TestResult;
    }
}
