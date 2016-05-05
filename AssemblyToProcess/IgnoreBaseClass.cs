public class IgnoreBaseStubClass
{
    public int A { get; set; }
}

[Equals(IgnoreBaseClassProperties = true)]
public class IgnoreBaseClass : IgnoreBaseStubClass
{
    public int B { get; set; }
}