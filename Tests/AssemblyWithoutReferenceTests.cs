using Fody;
using Xunit;

public class AssemblyWithoutReferenceTests
{
    [Fact]
    public void When_assembly_to_weave_does_not_reference_equals_assembly_weaving_should_not_fail()
    {
        var exception = Assert.Throws<WeavingException>(() => new ModuleWeaver().ExecuteTestRun("AssemblyToProcessWithoutReference.dll"));
    }
}