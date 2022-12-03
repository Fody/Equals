public class DomainObject
{
    private static StatePrinter StatePrinter = new();

    public override string ToString()
    {
        return StatePrinter.PrintObject( this );
    }
}