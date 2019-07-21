public class DomainObject
{
    private static readonly StatePrinter StatePrinter = new StatePrinter();

    public override string ToString()
    {
        return StatePrinter.PrintObject( this );
    }
}