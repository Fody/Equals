## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

![Icon](https://raw.github.com/Fody/Equals/master/Icons/package_icon.png)

Generate Equals, GetHashCode and operators methods from properties for class decorated with a `[Equals]` Attribute.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage).

## The nuget package  [![NuGet Status](http://img.shields.io/nuget/v/Equals.Fody.svg?style=flat)](https://www.nuget.org/packages/Equals.Fody/)

https://nuget.org/packages/Equals.Fody/

    PM> Install-Package Equals.Fody
    
## Your Code

    [Equals]
    public class Point
    {
        public int X { get; set; }
        
        public int Y { get; set; }
        
        [IgnoreDuringEquals]
        public int Z { get; set; }
        
        [CustomEqualsInternal]
        private bool CustomLogic(Point other)
        {
            return Z == other.Z || Z == 0 || other.Z == 0;
        }
    }

## What gets compiled

    public class Point : IEquatable<Point>
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }
        
        private bool CustomLogic(Point other)
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

        private static bool EqualsInternal(Point left, Point right)
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
            return this.X.GetHashCode() * 397 ^ this.Y.GetHashCode();
        }
    }

## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)
