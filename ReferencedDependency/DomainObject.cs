public class DomainObject
{
    private static StatePrinter StatePrinter = new StatePrinter();

    public override string ToString()
    {
        return StatePrinter.PrintObject( this );
    }
}