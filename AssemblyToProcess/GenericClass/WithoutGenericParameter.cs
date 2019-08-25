[Equals]
public class WithoutGenericParameter :
    GenericClass<GenericClassBaseClass>
{
    public int Z { get; set; }

    public static bool operator ==(WithoutGenericParameter left, WithoutGenericParameter right) => Operator.Weave(left, right);
    public static bool operator !=(WithoutGenericParameter left, WithoutGenericParameter right) => Operator.Weave(left, right);
}