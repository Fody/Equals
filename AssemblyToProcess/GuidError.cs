[Equals]
public class ReferenceObject :
    NameObject
{
    public static bool operator ==(ReferenceObject left, ReferenceObject right) => Operator.Weave();
    public static bool operator !=(ReferenceObject left, ReferenceObject right) => Operator.Weave();
}