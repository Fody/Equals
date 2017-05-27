using System;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

public static class CollectionInstructionExtensions
{
    public static void If(this Collection<Instruction> ins,
        Action<Collection<Instruction>> condition,
        Action<Collection<Instruction>> thenStatement,
        Action<Collection<Instruction>> elseStatement)
    {
        var ifEnd = Instruction.Create(OpCodes.Nop);
        var ifElse = Instruction.Create(OpCodes.Nop);

        condition(ins);

        if (ins[ins.Count - 1].OpCode == OpCodes.Ceq)
        {
            ins[ins.Count - 1] = Instruction.Create(OpCodes.Bne_Un, ifElse);
        }
        else
        {
            ins.Add(Instruction.Create(OpCodes.Brfalse, ifElse));
        }

        thenStatement(ins);

        ins.Add(Instruction.Create(OpCodes.Br, ifEnd));
        ins.Add(ifElse);

        elseStatement(ins);

        ins.Add(ifEnd);
    }

    public static void IfAnd(this Collection<Instruction> ins,
        Action<Collection<Instruction>> condition1,
        Action<Collection<Instruction>> condition2,
        Action<Collection<Instruction>> thenStatement,
        Action<Collection<Instruction>> elseStatement)
    {
        var ifEnd = Instruction.Create(OpCodes.Nop);
        var ifElse = Instruction.Create(OpCodes.Nop);

        condition1(ins);

        if (ins[ins.Count - 1].OpCode == OpCodes.Ceq)
        {
            ins[ins.Count - 1] = Instruction.Create(OpCodes.Bne_Un, ifElse);
        }
        else
        {
            ins.Add(Instruction.Create(OpCodes.Brfalse, ifElse));
        }

        condition2(ins);

        if (ins[ins.Count - 1].OpCode == OpCodes.Ceq)
        {
            ins[ins.Count - 1] = Instruction.Create(OpCodes.Bne_Un, ifElse);
        }
        else
        {
            ins.Add(Instruction.Create(OpCodes.Brfalse, ifElse));
        }

        thenStatement(ins);

        ins.Add(Instruction.Create(OpCodes.Br, ifEnd));
        ins.Add(ifElse);

        elseStatement(ins);

        ins.Add(ifEnd);
    }

    public static void If(this Collection<Instruction> ins,
        Action<Collection<Instruction>> condition,
        Action<Collection<Instruction>> thenStatement)
    {
        var ifEnd = Instruction.Create(OpCodes.Nop);

        condition(ins);

        if (ins[ins.Count - 1].OpCode == OpCodes.Ceq)
        {
            ins[ins.Count - 1] = Instruction.Create(OpCodes.Bne_Un, ifEnd);
        }
        else
        {
            ins.Add(Instruction.Create(OpCodes.Brfalse, ifEnd));
        }

        thenStatement(ins);

        ins.Add(ifEnd);
    }

    public static void IfNot(this Collection<Instruction> ins,
        Action<Collection<Instruction>> condition,
        Action<Collection<Instruction>> thenStatement)
    {
        var ifEnd = Instruction.Create(OpCodes.Nop);

        condition(ins);

        if (ins[ins.Count - 1].OpCode == OpCodes.Ceq)
        {
            ins[ins.Count - 1] = Instruction.Create(OpCodes.Beq, ifEnd);
        }
        else
        {
            ins.Add(Instruction.Create(OpCodes.Brtrue, ifEnd));
        }

        thenStatement(ins);

        ins.Add(ifEnd);
    }

    public static void While(this Collection<Instruction> ins,
        Action<Collection<Instruction>> condition,
        Action<Collection<Instruction>> body)
    {
        var loopBegin = Instruction.Create(OpCodes.Nop);
        var loopEnd = Instruction.Create(OpCodes.Nop);

        ins.Add(loopBegin);

        condition(ins);

        if (ins[ins.Count - 1].OpCode == OpCodes.Ceq)
        {
            ins[ins.Count - 1] = Instruction.Create(OpCodes.Bne_Un, loopEnd);
        }
        else
        {
            ins.Add(Instruction.Create(OpCodes.Brfalse, loopEnd));
        }

        body(ins);

        ins.Add(Instruction.Create(OpCodes.Br, loopBegin));
        ins.Add(loopEnd);
    }
}