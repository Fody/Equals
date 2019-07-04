using System;
using Xunit;

public partial class IntegrationTests
{
    /// To ensure that the equivalency operator actually uses the overridne object.Equals(object) method the overriden method behaves unexpectedly
    /// See OnlyOperator.Equals(object)!
    [Fact]
    public void Equality_operator_should_return_true_for_equal_class_instances()
    {
        var type = testResult.Assembly.GetType("OnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 2;

        Assert.True(first == second);
        Assert.True(second != first);
    }

    [Fact]
    public void Equality_operator_should_return_false_for_not_class_equal_instances()
    {
        var type = testResult.Assembly.GetType("OnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 3;

        Assert.True(first != second);
        Assert.False(first == second);
    }

    [Fact]
    public void Equality_operator_should_return_true_for_equal_struct_instances()
    {
        var type = testResult.Assembly.GetType("StructWithOnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 2;

        Assert.True(first == second);
        Assert.False(first != second);
    }


    [Fact]
    public void Equality_operator_should_return_true_for_equal_class_with_generic_property()
    {
        var genericClassType = testResult.Assembly.GetType("GenericProperty`1");
        var propType = typeof(int);
        var type = genericClassType.MakeGenericType(propType);

        dynamic first = Activator.CreateInstance(type);
        first.Prop = 1;
        dynamic second = Activator.CreateInstance(type);
        second.Prop = 1;

        Assert.True(first == second);
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.False(first != first);
#pragma warning restore CS1718 // Comparison made to same variable
    }

    [Fact]
    public void Equality_operator_should_return_false_for_not_equal_struct_instances()
    {
        var type = testResult.Assembly.GetType("StructWithOnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 3;

        Assert.True(first != second);
        Assert.False(first == second);
    }

    [Fact]
    public void Equality_operator_should_return_true_for_equal_guid_instances()
    {
        var type = testResult.Assembly.GetType("GuidClass");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        var newGuid = Guid.NewGuid();
        first.Key = newGuid;
        second.Key = newGuid;

        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equality_operator_should_return_true_for_empty_object_collections()
    {
        var type = testResult.Assembly.GetType("ObjectCollection");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Collection = new object[]
        {
        };
        second.Collection = new object[]
        {
        };

        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equality_operator_should_return_true_for_equal_object_collections()
    {
        var type = testResult.Assembly.GetType("ObjectCollection");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Collection = new object[]
        {
            "foo",
            1.23456
        };
        second.Collection = new object[]
        {
            "foo",
            1.23456
        };

        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equality_operator_should_return_false_for_collections_with_different_size()
    {
        var type = testResult.Assembly.GetType("ObjectCollection");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Collection = new object[]
        {
            "foo",
            1.23456,
            1
        };
        second.Collection = new object[]
        {
            "foo",
            1.23456
        };

        Assert.True(first != second);
        Assert.False(first == second);
        Assert.True(second != first);
        Assert.False(second == first);
    }

    [Fact]
    public void Equality_operator_should_return_false_for_collections_with_elements_and_empty_collection()
    {
        var type = testResult.Assembly.GetType("ObjectCollection");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Collection = new object[]
        {
        };
        second.Collection = new object[]
        {
            "foo",
            1.23456
        };

        Assert.True(first != second);
        Assert.False(first == second);
        Assert.True(second != first);
        Assert.False(second == first);
    }

    [Fact]
    public void Equality_operator_should_return_false_for_different_object_collections()
    {
        var type = testResult.Assembly.GetType("ObjectCollection");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Collection = new object[]
        {
            "foo",
            1.23456
        };
        second.Collection = new object[]
        {
            "bar",
            65432.1
        };

        Assert.True(first != second);
        Assert.False(first == second);
    }

    [Fact]
    public void Equality_should_return_false_for_class_with_method_to_remove()
    {
        var type = testResult.Assembly.GetType("ClassWithMethodToRemove");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;
        first.Y = 2;

        dynamic second = Activator.CreateInstance(type);
        second.X = 1;
        second.Y = 3;

        Assert.True(first != second);
        Assert.False(first == second);
    }
}