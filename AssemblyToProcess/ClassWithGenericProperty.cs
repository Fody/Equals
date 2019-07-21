[Equals]
public class ClassWithGenericProperty
{
    public GenericDependency<int> Prop { get; set; }

    public static bool operator ==(ClassWithGenericProperty left, ClassWithGenericProperty right) => Operator.Weave();
    public static bool operator !=(ClassWithGenericProperty left, ClassWithGenericProperty right) => Operator.Weave();
}