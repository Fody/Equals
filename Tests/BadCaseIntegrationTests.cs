using Fody;
using Xunit;

public class OperatorBadCaseIntegrationTests
{
    readonly ModuleWeaver weavingTask;

    public OperatorBadCaseIntegrationTests()
    {
        weavingTask = new ModuleWeaver();
    }

    [Fact]
    public void ClassWithoutOperatorsOptOutButWeavingInstruction()
    {
        var exception = Assert.Throws<WeavingException>(
            () => weavingTask.ExecuteTestRun("ClassWithOperatorsOptOutButWeavingInstruction.dll"));

        Assert.Equal(
            "Type OperatorsOptOutButHasWeavingInstruction marked with [Equals(DoNotAddEqualityOperators = true)] contains op_Equality with the instruction to weave it. Either set `DoNotAddEqualityOperators` to `false` or implement the operator properly.",
            exception.Message);
    }

    [Fact]
    public void StructWithoutOperatorsOptOutButWeavingInstruction()
    {
        var exception = Assert.Throws<WeavingException>(
            () => weavingTask.ExecuteTestRun("StructWithOperatorsOptOutButWeavingInstruction.dll"));

        Assert.Equal(
            "Type OperatorsOptOutButHasWeavingInstruction marked with [Equals(DoNotAddEqualityOperators = true)] contains op_Equality with the instruction to weave it. Either set `DoNotAddEqualityOperators` to `false` or implement the operator properly.",
            exception.Message);
    }

    [Fact]
    public void ClassWithoutOperators()
    {
        var exception = Assert.Throws<WeavingException>(
            () => weavingTask.ExecuteTestRun("ClassWithoutOperators.dll"));

        Assert.Equal(
            "Type WithoutOperators marked with the [Equals] attribute does not contain op_Equality. Fix this by adding a method `public static bool operator ==(T left, T right) => Operator.Weave();` or, if you don't want the operator to be woven: set `[Equals].DoNotAddEqualityOperators = true`.",
            exception.Message);
    }

    [Fact]
    public void StructWithoutOperators()
    {
        var exception = Assert.Throws<WeavingException>(
            () => weavingTask.ExecuteTestRun("StructWithoutOperators.dll"));

        Assert.Equal(
            "Type WithoutOperators marked with the [Equals] attribute does not contain op_Equality. Fix this by adding a method `public static bool operator ==(T left, T right) => Operator.Weave();` or, if you don't want the operator to be woven: set `[Equals].DoNotAddEqualityOperators = true`.",
            exception.Message);
    }

    [Fact]
    public void ClassWithoutWeavingInstruction()
    {
        var exception = Assert.Throws<WeavingException>(
            () => weavingTask.ExecuteTestRun("ClassWithoutWeavingInstruction.dll"));

        Assert.Equal(
            "TType WithoutWeavingInstruction marked with the [Equals] attribute contains op_Equality, but it does not contain the instruction to weave it. Either set implement the method like `public static bool operator ==(T left, T right) => Operator.Weave();` or, if you don't want the operator to be woven: set `[Equals].DoNotAddEqualityOperators = true`.",
            exception.Message);
    }

    [Fact]
    public void StructWithoutWeavingInstruction()
    {
        var exception = Assert.Throws<WeavingException>(
            () => weavingTask.ExecuteTestRun("StructWithoutWeavingInstruction.dll"));

        Assert.Equal(
            "TType WithoutWeavingInstruction marked with the [Equals] attribute contains op_Equality, but it does not contain the instruction to weave it. Either set implement the method like `public static bool operator ==(T left, T right) => Operator.Weave();` or, if you don't want the operator to be woven: set `[Equals].DoNotAddEqualityOperators = true`.",
            exception.Message);
    }
}