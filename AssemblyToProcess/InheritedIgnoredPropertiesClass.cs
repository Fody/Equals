[Equals]
public class InheritedIgnoredPropertiesClass :
    IgnoredPropertiesClass
{
    public static bool operator ==(InheritedIgnoredPropertiesClass left, InheritedIgnoredPropertiesClass right) => Operator.Weave();
    public static bool operator !=(InheritedIgnoredPropertiesClass left, InheritedIgnoredPropertiesClass right) => Operator.Weave();
}