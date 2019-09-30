using System;
using System.Collections.Generic;

[Equals]
public class NormalClass : IEquatable<NormalClass>
{
    static readonly IEqualityComparer<string> YComparer = StringComparer.OrdinalIgnoreCase;

    public int X { get; set; }

    [EqualityComparer(nameof(YComparer))]
    public string Y { get; set; }

    public double Z { get; set; }

    public char V { get; set; }

    public static bool operator ==(NormalClass left, NormalClass right) => Operator.Weave(left, right);
    public static bool operator !=(NormalClass left, NormalClass right) => Operator.Weave(left, right);

    bool IEquatable<NormalClass>.Equals(NormalClass other)
    {
        throw new NotImplementedException();
    }
}