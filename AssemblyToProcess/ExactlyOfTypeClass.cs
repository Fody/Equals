[Equals(TypeCheck = TypeCheck.ExactlyOfType)]
public class ExactlyOfTypeClass
{
    public int A { get; set; }
}

public class ExactlyOfTypeSubClass : ExactlyOfTypeClass
{
    public int B { get; set; }
}