
[Equals(TypeCheck = TypeCheck.ExactlyTheSameTypeAsThis)]
public class ExactlyTheSameTypeAsThisClass
{
    public int A { get; set; }

    public static bool operator ==(ExactlyTheSameTypeAsThisClass left, ExactlyTheSameTypeAsThisClass right) => Operator.Weave();
    public static bool operator !=(ExactlyTheSameTypeAsThisClass left, ExactlyTheSameTypeAsThisClass right) => Operator.Weave();
}

public class ExactlyTheSameTypeAsThisSubClass : ExactlyTheSameTypeAsThisClass
{
    public int B { get; set; }
}