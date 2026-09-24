using System;
using System.Linq;
using System.Threading.Tasks;
using VerifyTUnit;

public partial class IntegrationTests
{
    // To ensure that the equivalency operator actually uses the overriden object.Equals(object) method the overriden method behaves unexpectedly
    // See OnlyOperator.Equals(object)!
    [Test]
    public async Task Equality_operator_should_return_true_for_equal_class_instances()
    {
        var type = testResult.Assembly.GetType("OnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 2;

        await Assert.That((bool) (first == second)).IsTrue();
        await Assert.That((bool) (second != first)).IsTrue();
    }

    [Test]
    public async Task Equality_operator_should_return_false_for_not_class_equal_instances()
    {
        var type = testResult.Assembly.GetType("OnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 3;

        await Assert.That((bool) (first != second)).IsTrue();
        await Assert.That((bool) (first == second)).IsFalse();
    }

    [Test]
    public async Task Equality_operator_should_return_true_for_equal_struct_instances()
    {
        var type = testResult.Assembly.GetType("StructWithOnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 2;

        await Assert.That((bool) (first == second)).IsTrue();
        await Assert.That((bool) (second != first)).IsTrue();
    }

    [Test]
    public async Task Equality_operator_should_return_true_for_equal_class_with_generic_property()
    {
        var genericClassType = testResult.Assembly.GetType("GenericProperty`1");
        var propType = typeof(int);
        var type = genericClassType.MakeGenericType(propType);

        dynamic first = Activator.CreateInstance(type);
        first.Prop = 1;
        dynamic second = Activator.CreateInstance(type);
        second.Prop = 1;

        await Assert.That((bool) (first == second)).IsTrue();
#pragma warning disable CS1718 // Comparison made to same variable
        await Assert.That((bool) (first != first)).IsFalse();
#pragma warning restore CS1718 // Comparison made to same variable
    }

    [Test]
    public async Task Equality_operator_should_return_false_for_not_equal_struct_instances()
    {
        var type = testResult.Assembly.GetType("StructWithOnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 3;

        await Assert.That((bool) (first != second)).IsTrue();
        await Assert.That((bool) (first == second)).IsFalse();
    }

    [Test]
    public async Task Equality_operator_should_return_true_for_equal_guid_instances()
    {
        var type = testResult.Assembly.GetType("GuidClass");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        var newGuid = Guid.NewGuid();
        first.Key = newGuid;
        second.Key = newGuid;

        await Assert.That((bool) (first == second)).IsTrue();
        await Assert.That((bool) (first != second)).IsFalse();
    }

    [Test]
    public async Task Equality_operator_should_return_true_for_empty_object_collections()
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

        await Assert.That((bool) (first == second)).IsTrue();
        await Assert.That((bool) (first != second)).IsFalse();
    }

    [Test]
    public async Task Equality_operator_should_return_true_for_equal_object_collections()
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

        await Assert.That((bool) (first == second)).IsTrue();
        await Assert.That((bool) (first != second)).IsFalse();
    }

    [Test]
    public Task IncorrectAttributes()
    {
        return Verifier.Verify(testResult.Errors.Select(_ => _.Text));
    }

    [Test]
    public async Task Equality_operator_should_return_false_for_collections_with_different_size()
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

        await Assert.That((bool) (first != second)).IsTrue();
        await Assert.That((bool) (first == second)).IsFalse();
        await Assert.That((bool) (second != first)).IsTrue();
        await Assert.That((bool) (second == first)).IsFalse();
    }

    [Test]
    public async Task Equality_operator_should_return_false_for_collections_with_elements_and_empty_collection()
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

        await Assert.That((bool) (first != second)).IsTrue();
        await Assert.That((bool) (first == second)).IsFalse();
        await Assert.That((bool) (second != first)).IsTrue();
        await Assert.That((bool) (second == first)).IsFalse();
    }

    [Test]
    public async Task Equality_operator_should_return_false_for_different_object_collections()
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

        await Assert.That((bool) (first != second)).IsTrue();
        await Assert.That((bool) (first == second)).IsFalse();
    }

    [Test]
    public async Task When_opting_out_of_operators_should_not_add_operators()
    {
        var type = testResult.Assembly.GetType("DoNotAddEqualityOperators");

        var methodNames = type.GetMethods().Select(_ => _.Name).ToList();

        await Assert.That(methodNames).DoesNotContain("op_Equality");
        await Assert.That(methodNames).DoesNotContain("op_Inequality");
    }

    [Test]
    public async Task When_opting_out_of_operators_should_not_replace_operators()
    {
        var type = testResult.Assembly.GetType("DoNotReplaceEqualityOperators");

        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        await Assert.That((bool) (first == second)).IsTrue();
        await Assert.That((bool) (first != second)).IsTrue();
    }
}
