using Xunit;

public partial class IntegrationTests
{
    [Theory]
    [InlineData("123", "123")]
    [InlineData("123", "456")]
    public void Equals_should_ignore_marked_overridden_properties(string location1, string location2)
    {
        var first = _testResult.GetInstance("ProjectClass");
        first.Location = location1;
        first.X = 42;

        var second = _testResult.GetInstance("ProjectClass");
        second.Location = location2;
        second.X = 42;

        Assert.Equal(first, second);
    }
}