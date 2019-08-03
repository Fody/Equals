using Fody;
using Xunit;

public class AssemblyWithoutReferenceTests
{
    [Fact]
    public void When_assembly_to_weave_does_not_reference_equals_assembly_weaving_should_not_fail()
    {
        var testResult = new ModuleWeaver().ExecuteTestRun("AssemblyToProcessWithoutReference.dll");

        Assert.NotNull(testResult.GetInstance("Foo"));
    }
}