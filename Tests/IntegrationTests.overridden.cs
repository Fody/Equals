using System;
using NUnit.Framework;

public partial class IntegrationTests
{
    [TestCase("123", "123")]
    [TestCase("123", "456")]
    public void Equals_should_ignore_marked_overridden_properties(string location1, string location2)
    {
        var type = assembly.GetType("ProjectClass");
        dynamic first = Activator.CreateInstance(type);
        first.Location = location1;
        first.X = 42;

        dynamic second = Activator.CreateInstance(type);
        second.Location = location2;
        second.X = 42;

        Assert.AreEqual(first, second);
    }
}