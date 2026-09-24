
public class OperatorBadCaseIntegrationTests
{
    ModuleWeaver weavingTask;

    public OperatorBadCaseIntegrationTests()
    {
        weavingTask = new();
    }

    [Test]
    public async Task ClassWithoutOperatorsOptOutButWeavingInstruction()
    {
        var exception = await Assert.That(() => weavingTask.ExecuteTestRun("ClassWithOperatorsOptOutButWeavingInstruction.dll")).Throws<WeavingException>();

        await Assert.That(exception!.Message).IsEqualTo("Type OperatorsOptOutButHasWeavingInstruction marked with [Equals(DoNotAddEqualityOperators = true)] contains op_Equality with the instruction to weave it. Either set `DoNotAddEqualityOperators` to `false` or implement the operator properly.");
    }

    [Test]
    public async Task StructWithoutOperatorsOptOutButWeavingInstruction()
    {
        var exception = await Assert.That(() => weavingTask.ExecuteTestRun("StructWithOperatorsOptOutButWeavingInstruction.dll")).Throws<WeavingException>();

        await Assert.That(exception!.Message).IsEqualTo("Type OperatorsOptOutButHasWeavingInstruction marked with [Equals(DoNotAddEqualityOperators = true)] contains op_Equality with the instruction to weave it. Either set `DoNotAddEqualityOperators` to `false` or implement the operator properly.");
    }

    [Test]
    public async Task ClassWithoutOperators()
    {
        var exception = await Assert.That(() => weavingTask.ExecuteTestRun("ClassWithoutOperators.dll")).Throws<WeavingException>();

        await Assert.That(exception!.Message).IsEqualTo("Type WithoutOperators marked with the [Equals] attribute does not contain op_Equality. Fix this by adding a method `public static bool operator ==(T left, T right) => Operator.Weave(left, right);` or, if you don't want the operator to be woven: set `[Equals].DoNotAddEqualityOperators = true`.");
    }

    [Test]
    public async Task StructWithoutOperators()
    {
        var exception = await Assert.That(() => weavingTask.ExecuteTestRun("StructWithoutOperators.dll")).Throws<WeavingException>();

        await Assert.That(exception!.Message).IsEqualTo("Type WithoutOperators marked with the [Equals] attribute does not contain op_Equality. Fix this by adding a method `public static bool operator ==(T left, T right) => Operator.Weave(left, right);` or, if you don't want the operator to be woven: set `[Equals].DoNotAddEqualityOperators = true`.");
    }

    [Test]
    public async Task ClassWithoutWeavingInstruction()
    {
        var exception = await Assert.That(() => weavingTask.ExecuteTestRun("ClassWithoutWeavingInstruction.dll")).Throws<WeavingException>();

        await Assert.That(exception!.Message).IsEqualTo("Type WithoutWeavingInstruction marked with the [Equals] attribute contains op_Equality, but it does not contain the instruction to weave it. Either set implement the method like `public static bool operator ==(T left, T right) => Operator.Weave(left, right);` or, if you don't want the operator to be woven: set `[Equals].DoNotAddEqualityOperators = true`.");
    }

    [Test]
    public async Task StructWithoutWeavingInstruction()
    {
        var exception = await Assert.That(() => weavingTask.ExecuteTestRun("StructWithoutWeavingInstruction.dll")).Throws<WeavingException>();

        await Assert.That(exception!.Message).IsEqualTo("Type WithoutWeavingInstruction marked with the [Equals] attribute contains op_Equality, but it does not contain the instruction to weave it. Either set implement the method like `public static bool operator ==(T left, T right) => Operator.Weave(left, right);` or, if you don't want the operator to be woven: set `[Equals].DoNotAddEqualityOperators = true`.");
    }
}