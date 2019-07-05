using Fody;
using Xunit;
using Xunit.Abstractions;

public partial class IntegrationTests :
    XunitLoggingBase, IClassFixture<AssemblyToProcessFixture>
{
    readonly TestResult _testResult;
#pragma warning restore 618

    public IntegrationTests(ITestOutputHelper output, AssemblyToProcessFixture fixture) : base(output)
    {
        _testResult = fixture.TestResult;
    }
}