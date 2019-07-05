#pragma warning disable CS0660, CS0661
[Equals]
public struct WithoutWeavingInstruction
{
    public static bool operator ==(WithoutWeavingInstruction left, WithoutWeavingInstruction right) => true;
    public static bool operator !=(WithoutWeavingInstruction left, WithoutWeavingInstruction right) => false;
}