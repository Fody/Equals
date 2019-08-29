[Equals]
public class InheritedIgnoredPropertiesClass :
    IgnoredPropertiesClass
{
    public static bool operator ==(InheritedIgnoredPropertiesClass left, InheritedIgnoredPropertiesClass right) => Operator.Weave(left, right);
    public static bool operator !=(InheritedIgnoredPropertiesClass left, InheritedIgnoredPropertiesClass right) => Operator.Weave(left, right);
}