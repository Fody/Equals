using Fody;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public partial class IntegrationTests :
    VerifyBase,
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