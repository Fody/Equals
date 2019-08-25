[Equals]
public class ProjectClass :
    ProjectBaseClass
{
    [IgnoreDuringEquals]
    public override string Location { get; set; }

    public static bool operator ==(ProjectClass left, ProjectClass right) => Operator.Weave(left, right);
    public static bool operator !=(ProjectClass left, ProjectClass right) => Operator.Weave(left, right);
}