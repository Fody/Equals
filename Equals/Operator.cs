using System;

public static class Operator
{
    public static bool Weave<T>(T left, T right) => throw WeavingNotWorkingException();

    static Exception WeavingNotWorkingException() => new Exception("Equals.Fody was supposed to replace this method call with an implementation. Either weaving has not worked or you have called this method from an unsupported place. Only supported places are implementations of the `==` and `!=` operators.");
}