[Equals]
public class GenericProperty<T>
{
    public T Prop { get; set; }

    static bool Z(T a, T b)
    {
        return Equals(a, b);
    }

    public static bool operator ==(GenericProperty<T> left, GenericProperty<T> right) => Operator.Weave();
    public static bool operator !=(GenericProperty<T> left, GenericProperty<T> right) => Operator.Weave();
}