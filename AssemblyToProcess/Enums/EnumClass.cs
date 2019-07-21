[Equals]
public class EnumClass
{
    public NormalEnum NormalEnum { get; set; }

    public FlagsEnum FlagsEnum { get; set; }

    public EnumClass()
    {
    }

    public EnumClass(int normalEnum, int flagsEnum)
    {
        NormalEnum = (NormalEnum)normalEnum;
        FlagsEnum = (FlagsEnum)flagsEnum;
    }

    public static bool operator ==(EnumClass left, EnumClass right) => Operator.Weave();
    public static bool operator !=(EnumClass left, EnumClass right) => Operator.Weave();
}