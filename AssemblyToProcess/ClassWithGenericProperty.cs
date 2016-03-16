[Equals(DoNotAddEqualityOperators = true,
    DoNotAddEquals = true)]
public class ClassWithGenericProperty
{
    public GenericDependency<int> Prop { get; set; }
}