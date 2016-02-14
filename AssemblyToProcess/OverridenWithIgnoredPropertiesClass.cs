[Equals]
public class ProjectClass : ProjectBaseClass
{
    [IgnoreDuringEquals]
    public override string Location { get; set; }
}

public class ProjectBaseClass
{
    public virtual string Location { get; set; }

    public int X { get; set; }
}