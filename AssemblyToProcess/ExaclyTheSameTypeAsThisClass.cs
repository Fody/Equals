
[Equals(TypeCheck = TypeCheck.ExaclyTheSameTypeAsThis)]
public class ExaclyTheSameTypeAsThisClass
{
    public int A { get; set; }
}

public class ExaclyTheSameTypeAsThisSubClass : ExaclyTheSameTypeAsThisClass
{
    public int B { get; set; }
}