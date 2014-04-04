
[Equals(TypeCheck = TypeCheck.ExactlyTheSameTypeAsThis)]
public class ExactlyTheSameTypeAsThisClass
{
    public int A { get; set; }
}

public class ExactlyTheSameTypeAsThisSubClass : ExactlyTheSameTypeAsThisClass
{
    public int B { get; set; }
}