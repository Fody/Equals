﻿[Equals(TypeCheck = TypeCheck.EqualsOrSubtype)]
public class EqualsOrSubtypeClass
{
    public int A { get; set; }

    public static bool operator ==(EqualsOrSubtypeClass left, EqualsOrSubtypeClass right) => Operator.Weave();
    public static bool operator !=(EqualsOrSubtypeClass left, EqualsOrSubtypeClass right) => Operator.Weave();
}

public class EqualsOrSubtypeSubClass : EqualsOrSubtypeClass
{
    public int B { get; set; }
}