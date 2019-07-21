using System;

public static class Operator
{
    public static bool Weave() => throw new Exception("Equals.Fody was supposed to replace this method call with an implementation. Either weaving has not worked or you have called this method from an unsupported place. Only supported places are implementations of the `==` and `!=` operators.");
}