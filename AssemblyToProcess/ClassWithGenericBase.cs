[Equals]
public class ClassWithGenericBase : GenericBase<int>
{
    public int Prop { get; set; }

    public static bool operator ==(ClassWithGenericBase left, ClassWithGenericBase right) => Operator.Weave();
    public static bool operator !=(ClassWithGenericBase left, ClassWithGenericBase right) => Operator.Weave();
}