using System;

public class IdObject :
    DomainObject
{
    public Guid Id { get; set; }

    public bool IsEmpty()
    {
        return Id == Guid.Empty;
    }
}