using System;

public partial class IntegrationTests
{
    [Test]
    public async Task Equals_should_use_custom_logic()
    {
        var first = testResult.GetInstance("CustomEquals");
        first.X = 1;

        var second = testResult.GetInstance("CustomEquals");
        second.X = 2;

        var result = first.Equals(second);

        await Assert.That((bool) (result)).IsTrue();
    }

    [Test]
    public async Task Equals_should_use_custom_logic_for_structure()
    {
        var first = testResult.GetInstance("CustomStructEquals");
        first.X = 1;

        var second = testResult.GetInstance("CustomStructEquals");
        second.X = 2;

        var result = first.Equals(second);

        await Assert.That((bool) (result)).IsTrue();
    }

    [Test]
    public async Task Equals_should_use_custom_logic_for_generic_type()
    {
        var genericClassType = testResult.Assembly.GetType("CustomGenericEquals`1");
        var propType = typeof(int);
        var type = genericClassType.MakeGenericType(propType);

        dynamic first = Activator.CreateInstance(type);
        first.Prop = 1;
        dynamic second = Activator.CreateInstance(type);
        second.Prop = 1;
        dynamic third = Activator.CreateInstance(type);
        third.Prop = 2;

        await Assert.That((bool) (first.Equals(second))).IsTrue();
        await Assert.That((bool) (first.Equals(third))).IsFalse();
    }

    [Test]
    public async Task GetHashCode_should_use_custom_logic()
    {
        var instance = testResult.GetInstance("CustomGetHashCode");
        instance.X = 1;

        var result = instance.GetHashCode();

        await Assert.That((int) (result)).IsEqualTo(423);
    }

    [Test]
    public async Task GetHashCode_should_use_custom_logic_for_structure()
    {
        var instance = testResult.GetInstance("CustomStructEquals");
        instance.X = 1;

        var result = instance.GetHashCode();

        await Assert.That((int) (result)).IsEqualTo(42);
    }

    [Test]
    public async Task GetHashCode_should_use_custom_logic_for_generic_type()
    {
        var genericClassType = testResult.Assembly.GetType("CustomGenericEquals`1");
        var propType = typeof(int);
        var type = genericClassType.MakeGenericType(propType);

        dynamic instance = Activator.CreateInstance(type);
        instance.Prop = 1;

        var result = instance.GetHashCode();

        await Assert.That((int) (result)).IsEqualTo(42);
    }
}
