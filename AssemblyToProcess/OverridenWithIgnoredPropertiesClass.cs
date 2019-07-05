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

    // TODO: adding the following code here leads to a PEVerify problem. It shouldn't (but should throw an exception at runtime instead!)
    // TODO: or should we also scan base-classes for replacement instructions?!
    //public static bool operator ==(ProjectBaseClass left, ProjectBaseClass right) => Operator.Weave();
    //public static bool operator !=(ProjectBaseClass left, ProjectBaseClass right) => Operator.Weave();
}