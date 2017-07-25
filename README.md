[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg?style=flat)](https://gitter.im/Fody/Fody)
[![NuGet Status](http://img.shields.io/nuget/v/Equals.Fody.svg?style=flat)](https://www.nuget.org/packages/Equals.Fody/)

![Icon](https://raw.github.com/Fody/Equals/master/Icons/package_icon.png)

Generate `Equals`, `GetHashCode` and operators methods from properties for class decorated with a `[Equals]` Attribute.

This is an add-in for [Fody](https://github.com/Fody/Fody/); it is available via [NuGet](https://www.nuget.org/packages/Equals.Fody/):

    PM> Install-Package Equals.Fody

# Overview

Your code:

```csharp
[Equals]
public class Point 
{
    public int X { get; set; }
    public int Y { get; set; }
    
    [IgnoreDuringEquals]
    public int Z { get; set; }
    
    [CustomEqualsInternal]
    bool CustomLogic(Point other) => Z == other.Z || Z == 0 || other.Z == 0;
}

[Equals]
public class CustomGetHashCode
{
    public int X { get; set; }

    [IgnoreDuringEquals]
    public int Z { get; set; }

    [CustomGetHashCode]
    int CustomGetHashCodeMethod() => 42;
}
```
What gets compiled:

```csharp
public class Point : IEquatable<Point>
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    
    bool CustomLogic(Point other) => Z == other.Z || Z == 0 || other.Z == 0;

    public static bool operator == (Point left, Point right) => object.Equals((object)left, (object)right);
    public static bool operator != (Point left, Point right) => !object.Equals((object)left, (object)right);

    static bool EqualsInternal(Point left, Point right) => 
        left.X == right.X 
        && left.Y == right.Y 
        && leftt.CustomLogic(right);

    public virtual bool Equals(Point right) => 
        !object.ReferenceEquals((object)null, (object)right) && (
            object.ReferenceEquals((object)this, (object)right) 
            || Point.EqualsInternal(this, right)
        );

    public override bool Equals(object right) => 
        !object.ReferenceEquals((object)null, right) && (
            object.ReferenceEquals((object)this, right) 
            || this.GetType() == right.GetType() && Point.EqualsInternal(this, (Point)right)
        );
    }

    public override int GetHashCode() => unchecked(this.X.GetHashCode() * 397 ^ this.Y.GetHashCode());
}

public class CustomGetHashCode : IEquatable<CustomGetHashCode>
{
    public int X { get; set; }
    public int Z { get; set;}

    private int CustomGetHashCodeMethod() => 42;

    private static bool EqualsInternal(CustomGetHashCode left, CustomGetHashCode right) => left.X == right.X;

    public override bool Equals(CustomGetHashCode other) => 
        !object.ReferenceEquals(null, other) && (
            object.ReferenceEquals(this, other) 
            || CustomGetHashCode.EqualsInternal(this, other)
        );

    public override bool Equals(object obj) => 
        !object.ReferenceEquals(null, obj) && (
            object.ReferenceEquals(this, obj) 
            || (base.GetType() == obj.GetType() && CustomGetHashCode.EqualsInternal(this, (CustomGetHashCode)obj))
        );

    public override int GetHashCode() => (this.X.GetHashCode() * 397) ^ this.CustomGetHashCodeMethod();

    public static bool operator == (CustomGetHashCode left, CustomGetHashCode right) => object.Equals(left, right);

    public static bool operator != (CustomGetHashCode left, CustomGetHashCode right) => !object.Equals(left, right);
}
```

---

Icon courtesy of [The Noun Project](http://thenounproject.com)
