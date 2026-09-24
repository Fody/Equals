
public class AssemblyWithoutReferenceTests
{
    [Test]
    public async Task When_assembly_to_weave_does_not_reference_equals_assembly_weaving_should_not_fail()
    {
        var testResult = new ModuleWeaver().ExecuteTestRun("AssemblyToProcessWithoutReference.dll");

        await Assert.That((object) (testResult.GetInstance("Foo"))).IsNotNull();
    }
}
