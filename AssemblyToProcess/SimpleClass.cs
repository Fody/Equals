using System;

[Equals]
public class SimpleClass
{
    public int Value { get; set; }

    public string Text { get; set; }

    public DateTime Date { get; set; }

    public static bool operator ==(SimpleClass left, SimpleClass right) => Operator.Weave();
    public static bool operator !=(SimpleClass left, SimpleClass right) => Operator.Weave();
}