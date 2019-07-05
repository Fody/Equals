using Xunit;

public partial class IntegrationTests
{
    [Fact]
    public void Equals_should_return_true_for_child_with_parent_in_other_assembly()
    {
        var first = _testResult.GetInstance("Child"); ;
        first.InParent = 10;
        first.InChild = 5;

        var second = _testResult.GetInstance("Child"); ;
        second.InParent = 10;
        second.InChild = 5;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void GetHashCode_should_return_true_for_child_with_parent_in_other_assembly()
    {
        var first = _testResult.GetInstance("Child"); ;
        first.InParent = 10;
        first.InChild = 5;

        var result = first.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void Equality_operator_should_return_true_for_child_with_parent_in_other_assembly()
    {
        var first = _testResult.GetInstance("Child"); ;
        first.InParent = 10;
        first.InChild = 5;

        var second = _testResult.GetInstance("Child");
        second.InParent = 10;
        second.InChild = 5;

        var result = first.GetHashCode();

        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equals_should_return_true_for_child_with_complex_parent_in_other_assembly()
    {
        var first = _testResult.GetInstance("ComplexChild");
        first.InChildNumber = 1;
        first.InChildText = "test";
        first.InChildCollection = new[] { 1, 2 };
        first.InParentNumber = 1;
        first.InParentText = "test";
        first.InParentCollection = new[] { 1, 2 };

        var second = _testResult.GetInstance("ComplexChild");
        second.InChildNumber = 1;
        second.InChildText = "test";
        second.InChildCollection = new[] { 1, 2 };
        second.InParentNumber = 1;
        second.InParentText = "test";
        second.InParentCollection = new[] { 1, 2 };

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void GetHashCode_should_return_true_for_child_with_complex_parent_in_other_assembly()
    {
        var first = _testResult.GetInstance("ComplexChild");
        first.InChildNumber = 1;
        first.InChildText = "test";
        first.InChildCollection = new[] { 1, 2 };
        first.InParentNumber = 1;
        first.InParentText = "test";
        first.InParentCollection = new[] { 1, 2 };

        var result = first.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void Equality_operator_should_return_true_for_child_with_complex_parent_in_other_assembly()
    {
        var first = _testResult.GetInstance("ComplexChild");
        first.InChildNumber = 1;
        first.InChildText = "test";
        first.InChildCollection = new[] { 1, 2 };
        first.InParentNumber = 1;
        first.InParentText = "test";
        first.InParentCollection = new[] { 1, 2 };

        var second = _testResult.GetInstance("ComplexChild");
        second.InChildNumber = 1;
        second.InChildText = "test";
        second.InChildCollection = new[] { 1, 2 };
        second.InParentNumber = 1;
        second.InParentText = "test";
        second.InParentCollection = new[] { 1, 2 };

        var result = first.GetHashCode();

        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equals_should_return_true_for_generic_child_with_parent_in_other_assembly()
    {
        var first = _testResult.GetInstance("GenericChild");
        first.InChild = "1";
        first.GenericInParent = 2;

        var second = _testResult.GetInstance("GenericChild");
        second.InChild = "1";
        second.GenericInParent = 2;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void GetHashCode_should_return_true_for_generic_child_with_parent_in_other_assembly()
    {
        var first = _testResult.GetInstance("GenericChild");
        first.InChild = "1";
        first.GenericInParent = 2;

        var result = first.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void Equality_operator_should_return_true_for_generic_child_with_parent_in_other_assembly()
    {
        var first = _testResult.GetInstance("GenericChild");
        first.InChild = "1";
        first.GenericInParent = 2;

        var second = _testResult.GetInstance("GenericChild");
        second.InChild = "1";
        second.GenericInParent = 2;

        var result = first.GetHashCode();

        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void GetHashCode_should_return_value_class_with_generic_base()
    {
        var instance = _testResult.GetInstance("ClassWithGenericBase");
        instance.Prop = 1;

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void Equals_should_return_value_class_with_generic_base()
    {
        var first = _testResult.GetInstance("ClassWithGenericBase");
        first.Prop = 1;

        var second = _testResult.GetInstance("ClassWithGenericBase");
        second.Prop = 1;
        var result = first.Equals(second);

        Assert.True(result);
    }
}