
public partial class IntegrationTests
{
    [Test]
    public async Task Equals_should_return_true_for_child_with_parent_in_other_assembly()
    {
        var first = testResult.GetInstance("Child");
        first.InParent = 10;
        first.InChild = 5;

        var second = testResult.GetInstance("Child");
        second.InParent = 10;
        second.InChild = 5;

        var result = first.Equals(second);

        await Assert.That((bool) (result)).IsTrue();
    }

    [Test]
    public async Task GetHashCode_should_return_true_for_child_with_parent_in_other_assembly()
    {
        var first = testResult.GetInstance("Child");
        first.InParent = 10;
        first.InChild = 5;

        var result = first.GetHashCode();

        await Assert.That((int) (result)).IsNotEqualTo(0);
    }

    [Test]
    public async Task Equality_operator_should_return_true_for_child_with_parent_in_other_assembly()
    {
        var first = testResult.GetInstance("Child");
        first.InParent = 10;
        first.InChild = 5;

        var second = testResult.GetInstance("Child");
        second.InParent = 10;
        second.InChild = 5;

        await Assert.That((bool) (first == second)).IsTrue();
        await Assert.That((bool) (first != second)).IsFalse();
    }

    [Test]
    public async Task Equals_should_return_true_for_child_with_complex_parent_in_other_assembly()
    {
        var first = testResult.GetInstance("ComplexChild");
        first.InChildNumber = 1;
        first.InChildText = "test";
        first.InChildCollection = new[] {1, 2};
        first.InParentNumber = 1;
        first.InParentText = "test";
        first.InParentCollection = new[] {1, 2};

        var second = testResult.GetInstance("ComplexChild");
        second.InChildNumber = 1;
        second.InChildText = "test";
        second.InChildCollection = new[] {1, 2};
        second.InParentNumber = 1;
        second.InParentText = "test";
        second.InParentCollection = new[] {1, 2};

        var result = first.Equals(second);

        await Assert.That((bool) (result)).IsTrue();
    }

    [Test]
    public async Task GetHashCode_should_return_true_for_child_with_complex_parent_in_other_assembly()
    {
        var first = testResult.GetInstance("ComplexChild");
        first.InChildNumber = 1;
        first.InChildText = "test";
        first.InChildCollection = new[] {1, 2};
        first.InParentNumber = 1;
        first.InParentText = "test";
        first.InParentCollection = new[] {1, 2};

        var result = first.GetHashCode();

        await Assert.That((int) (result)).IsNotEqualTo(0);
    }

    [Test]
    public async Task Equality_operator_should_return_true_for_child_with_complex_parent_in_other_assembly()
    {
        var first = testResult.GetInstance("ComplexChild");
        first.InChildNumber = 1;
        first.InChildText = "test";
        first.InChildCollection = new[] {1, 2};
        first.InParentNumber = 1;
        first.InParentText = "test";
        first.InParentCollection = new[] {1, 2};

        var second = testResult.GetInstance("ComplexChild");
        second.InChildNumber = 1;
        second.InChildText = "test";
        second.InChildCollection = new[] {1, 2};
        second.InParentNumber = 1;
        second.InParentText = "test";
        second.InParentCollection = new[] {1, 2};

        await Assert.That((bool) (first == second)).IsTrue();
        await Assert.That((bool) (first != second)).IsFalse();
    }

    [Test]
    public async Task Equals_should_return_true_for_generic_child_with_parent_in_other_assembly()
    {
        var first = testResult.GetInstance("GenericChild");
        first.InChild = "1";
        first.GenericInParent = 2;

        var second = testResult.GetInstance("GenericChild");
        second.InChild = "1";
        second.GenericInParent = 2;

        var result = first.Equals(second);

        await Assert.That((bool) (result)).IsTrue();
    }

    [Test]
    public async Task GetHashCode_should_return_true_for_generic_child_with_parent_in_other_assembly()
    {
        var first = testResult.GetInstance("GenericChild");
        first.InChild = "1";
        first.GenericInParent = 2;

        var result = first.GetHashCode();

        await Assert.That((int) (result)).IsNotEqualTo(0);
    }

    [Test]
    public async Task Equality_operator_should_return_true_for_generic_child_with_parent_in_other_assembly()
    {
        var first = testResult.GetInstance("GenericChild");
        first.InChild = "1";
        first.GenericInParent = 2;

        var second = testResult.GetInstance("GenericChild");
        second.InChild = "1";
        second.GenericInParent = 2;

        await Assert.That((bool) (first == second)).IsTrue();
        await Assert.That((bool) (first != second)).IsFalse();
    }

    [Test]
    public async Task GetHashCode_should_return_value_class_with_generic_base()
    {
        var instance = testResult.GetInstance("ClassWithGenericBase");
        instance.Prop = 1;

        var result = instance.GetHashCode();

        await Assert.That((int) (result)).IsNotEqualTo(0);
    }

    [Test]
    public async Task Equals_should_return_value_class_with_generic_base()
    {
        var first = testResult.GetInstance("ClassWithGenericBase");
        first.Prop = 1;

        var second = testResult.GetInstance("ClassWithGenericBase");
        second.Prop = 1;
        var result = first.Equals(second);

        await Assert.That((bool) (result)).IsTrue();
    }
}
