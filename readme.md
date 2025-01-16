# <img src="/package_icon.png" height="30px"> Equals

[![NuGet Status](https://img.shields.io/nuget/v/Equals.Fody.svg)](https://www.nuget.org/packages/Equals.Fody/)

Generate Equals, GetHashCode and operator methods from properties for classes decorated with an `[Equals]` Attribute

**See [Milestones](../../milestones?state=closed) for release notes.**


### This is an add-in for [Fody](https://github.com/Fody/Home/)

**It is expected that all developers using Fody either [become a Patron on OpenCollective](https://opencollective.com/fody/contribute/patron-3059), or have a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-fody?utm_source=nuget-fody&utm_medium=referral&utm_campaign=enterprise). [See Licensing/Patron FAQ](https://github.com/Fody/Home/blob/master/pages/licensing-patron-faq.md) for more information.**


## Usage

See also [Fody usage](https://github.com/Fody/Home/blob/master/pages/usage.md).


### NuGet installation

Install the [Equals.Fody NuGet package](https://nuget.org/packages/Equals.Fody/) and update the [Fody NuGet package](https://nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
PM> Install-Package Equals.Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.


### Add to FodyWeavers.xml

Add `<Equals/>` to [FodyWeavers.xml](https://github.com/Fody/Home/blob/master/pages/usage.md#add-fodyweaversxml)

```xml
<Weavers>
  <Equals/>
</Weavers>
```


## Your Code

```csharp
[Equals]
public class Point
{
    public int X { get; set; }
    
    public int Y { get; set; }
    
    [IgnoreDuringEquals]
    public int Z { get; set; }
    
    [CustomEqualsInternal]
    bool CustomLogic(Point other)
    {
        return Z == other.Z || Z == 0 || other.Z == 0;
    }
    
    public static bool operator ==(Point left, Point right) => Operator.Weave(left, right);
    public static bool operator !=(Point left, Point right) => Operator.Weave(left, right);
}

[Equals]
public class CustomGetHashCode
{
    public int X { get; set; }

    [IgnoreDuringEquals]
    public int Z { get; set; }

    [CustomGetHashCode]
    int CustomGetHashCodeMethod()
    {
        return 42;
    }
    
    public static bool operator ==(CustomGetHashCode left, CustomGetHashCode right) => Operator.Weave(left, right);
    public static bool operator !=(CustomGetHashCode left, CustomGetHashCode right) => Operator.Weave(left, right);
}
```

Note:
- unless you specify `[Equals(DoNotAddEqualityOperators = true)]`, you must always add the `==` and `!=` method stubs with the `Operator.Weave()` implementation (if you want to know why, see [#10](https://github.com/Fody/Equals/issues/10)).
- adding the `==` and `!=` operators will result in compiler warnings CS0660 and CS0661, which tell you to implement custom `Equals` and `GetHashCode` implementations. Equals.Fody is doing this for you, but after the compiler runs. To suppress the false-positives you can either do so
  - per project, by adding `<PropertyGroup><NoWarn>CS0660;CS0661</NoWarn></PropertyGroup>` to the project file
  - per source file, by adding `#pragma warning disable CS0660, CS0661`.
- implementing a custom hash code method (marked by `[CustomGetHashCode]`) is optional.

## What gets compiled

```csharp
public class Point : IEquatable<Point>
{
    public int X { get; set; }

    public int Y { get; set; }

    public int Z { get; set; }
    
    bool CustomLogic(Point other)
    {
        return Z == other.Z || Z == 0 || other.Z == 0;
    }

    public static bool operator ==(Point left, Point right)
    {
        return object.Equals((object)left, (object)right);
    }

    public static bool operator !=(Point left, Point right)
    {
        return !object.Equals((object)left, (object)right);
    }

    static bool EqualsInternal(Point left, Point right)
    {
        return left.X == right.X && left.Y == right.Y && leftt.CustomLogic(right);
    }

    public virtual bool Equals(Point right)
    {
        return !object.ReferenceEquals((object)null, (object)right) && (object.ReferenceEquals((object)this, (object)right) || Point.EqualsInternal(this, right));
    }

    public override bool Equals(object right)
    {
        return !object.ReferenceEquals((object)null, right) && (object.ReferenceEquals((object)this, right) || this.GetType() == right.GetType() && Point.EqualsInternal(this, (Point)right));
    }

    public override int GetHashCode()
    {
        return unchecked(this.X.GetHashCode() * 397 ^ this.Y.GetHashCode());
    }
}

public class CustomGetHashCode : IEquatable<CustomGetHashCode>
{
    public int X { get; set; }

    public int Z { get; set; }

    int CustomGetHashCodeMethod()
    {
        return 42;
    }

    static bool EqualsInternal(CustomGetHashCode left, CustomGetHashCode right)
    {
        return left.X == right.X;
    }

    public override bool Equals(CustomGetHashCode other)
    {
        return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || CustomGetHashCode.EqualsInternal(this, other));
    }

    public override bool Equals(object obj)
    {
        return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (base.GetType() == obj.GetType() && CustomGetHashCode.EqualsInternal(this, (CustomGetHashCode)obj)));
    }

    public override int GetHashCode()
    {
        return (this.X.GetHashCode() * 397) ^ this.CustomGetHashCodeMethod();
    }

    public static bool operator ==(CustomGetHashCode left, CustomGetHashCode right)
    {
        return object.Equals(left, right);
    }

    public static bool operator !=(CustomGetHashCode left, CustomGetHashCode right)
    {
        return !object.Equals(left, right);
    }
}
```

## Configurability

Through properties on the `[Equals]` attribute the following options can be set:

 - `DoNotAddEqualityOperators` => do not weave `==` and `!=` operators
 - `DoNotAddGetHashCode` => do not override the `int GetHashCode()` methods
 - `DoNotAddEquals` => do not override the `bool Equals(object other)` method, do not add and implement `IEquatable<T>`
 - `IgnoreBaseClassProperties` => equality and hash code do not consider properties of base classes.
 - `TypeCheck` can be used to affect the equality logic.
   - `ExactlyTheSameTypeAsThis` (*default*): only equal, when the other object is of the same type as `this`. Imagine we have a class `Foo` with `[Equals]` and we have a sub-`class Bar : Foo`:
     - `Foo` may equal `Foo` 
     - `Bar` may equal `Bar`
     - but `Foo` may never equal `Bar`
   - `ExactlyOfType`: only equal, when the other object is of the same as the method is added to. Consider a class `Foo` with `[Equals(TypeCheck = ExactlyOfType)]` and a sub-`class Bar : Foo`:
     - `Foo` may equal `Foo`
     - `Bar` may never equal `Bar`
     - `Foo` may never equal `Bar`
   - `EqualsOrSubtype`: equal, when the other object is of same type as the method is added to, or is of a sub type.

## Icon

Icon courtesy of [The Noun Project](https://thenounproject.com)
