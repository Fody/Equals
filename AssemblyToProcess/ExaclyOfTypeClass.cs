[Equals(TypeCheck = TypeCheck.ExaclyOfType)]
public class ExaclyOfTypeClass
{
    public int A { get; set; }
}

public class ExaclyOfTypeSubClass : ExaclyOfTypeClass
{
    public int B { get; set; }
}