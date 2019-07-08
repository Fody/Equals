[Equals]
public class ProjectClass : ProjectBaseClass
{
    [IgnoreDuringEquals]
    public override string Location { get; set; }

    public static bool operator ==(ProjectClass left, ProjectClass right) => Operator.Weave();
    public static bool operator !=(ProjectClass left, ProjectClass right) => Operator.Weave();
}

public class ProjectBaseClass
{
    public virtual string Location { get; set; }

    public int X { get; set; }
}