using System;

public enum NormalEnum
{
    A = 0,
    B = 1,
    C = 2,
    D = 3,
    E = 4,
}

[Flags]
public enum FlagsEnum
{
    G = 0,
    H = 1,
    I = 2,
    J = 4,
    K = 8,
}

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
        this.NormalEnum = (NormalEnum)normalEnum;
        this.FlagsEnum = (FlagsEnum)flagsEnum;
    }
}