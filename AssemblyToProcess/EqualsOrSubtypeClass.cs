[Equals(TypeCheck = TypeCheck.EqualsOrSubtype)]
public class EqualsOrSubtypeClass
{
    public int A { get; set; }
}

public class EqualsOrSubtypeSubClass : EqualsOrSubtypeClass
{
    public int B { get; set; }
}