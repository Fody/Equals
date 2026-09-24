
public partial class IntegrationTests
{
    [Test]
    [Arguments("123", "123")]
    [Arguments("123", "456")]
    public async Task Equals_should_ignore_marked_overridden_properties(string location1, string location2)
    {
        var first = testResult.GetInstance("ProjectClass");
        first.Location = location1;
        first.X = 42;

        var second = testResult.GetInstance("ProjectClass");
        second.Location = location2;
        second.X = 42;

        await Assert.That((object) second).IsEqualTo((object) first);
    }
}