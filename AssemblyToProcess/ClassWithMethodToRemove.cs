using System;

[Equals]
public class ClassWithMethodToRemove : IEquatable<ClassWithMethodToRemove>
{
    public int X { get; set; }

    public int Y { get; set; }

    public static bool operator ==(ClassWithMethodToRemove left, ClassWithMethodToRemove right)
    {
        return false;
    }

    public static bool operator !=(ClassWithMethodToRemove left, ClassWithMethodToRemove right)
    {
        return true;
    }

    private static bool EqualsInternal(ClassWithMethodToRemove left, ClassWithMethodToRemove right)
    {
        return false;
    }

    public virtual bool Equals(ClassWithMethodToRemove other)
    {
        return false;
    }

    public override bool Equals(object obj)
    {
       return false;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}

namespace Equals
{
    public class Helpers
    {
        void Test()
        {
        }
    }
}