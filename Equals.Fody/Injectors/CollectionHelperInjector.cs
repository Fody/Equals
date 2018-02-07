using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

public static class CollectionHelperInjector
{
    public static MethodDefinition Inject(ModuleDefinition moduleDefinition)
    {
        var mod = 0;
        TypeDefinition typeDef = null;
        do
        {
            var name = mod == 0 ? "Equals.Helpers" : "Equals.Helpers" + mod;
            typeDef = moduleDefinition.Types.FirstOrDefault(x => x.FullName == name);
            if (typeDef != null)
            {
                mod++;
            }
        } while (typeDef != null);

        var selectedName = mod == 0 ? "Helpers" : "Helpers" + mod;
        var typeAttributes = TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit;
        var helperType = new TypeDefinition("Equals", selectedName, typeAttributes);
        helperType.CustomAttributes.MarkAsGeneratedCode();
        helperType.BaseType = ReferenceFinder.ObjectType;
        moduleDefinition.Types.Add(helperType);

        var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static;
        var method = new MethodDefinition("CollectionEquals", methodAttributes, ReferenceFinder.BooleanType);
        helperType.Methods.Add(method);

        var left = method.Parameters.Add("left", ReferenceFinder.IEnumerableType);
        var right = method.Parameters.Add("right", ReferenceFinder.IEnumerableType);

        var body = method.Body;
        var ins = body.Instructions;

        body.InitLocals = true;

        var leftEnumerator = body.Variables.Add(ReferenceFinder.IEnumeratorType);
        var rightEnumerator = body.Variables.Add(ReferenceFinder.IEnumeratorType);
        var leftHasNext = body.Variables.Add(ReferenceFinder.BooleanType);
        var rightHasNext = body.Variables.Add(ReferenceFinder.BooleanType);

        AddLeftAndRightReferenceEquals(ins, left, right);
        AddLeftAndNullReferenceEquals(ins, left);
        AddRightAndNullReferenceEquals(ins, right);

        AddGetEnumerator(ins, left, leftEnumerator);
        AddGetEnumerator(ins, right, rightEnumerator);

        AddCollectionLoop(ins, leftEnumerator, leftHasNext, rightEnumerator, rightHasNext);

        body.OptimizeMacros();
        method.CustomAttributes.MarkAsGeneratedCode();

        return method;
    }

    static void AddCollectionLoop(Collection<Instruction> ins, VariableDefinition leftEnumerator, VariableDefinition leftHasNext,
        VariableDefinition rightEnumerator, VariableDefinition rightHasNext)
    {
        var loopBegin = Instruction.Create(OpCodes.Nop);
        ins.Add(loopBegin);

        AddMoveNext(ins, leftEnumerator, leftHasNext);
        AddMoveNext(ins, rightEnumerator, rightHasNext);

        ins.IfAnd(
            c => AddCheckHasNext(c, leftHasNext, false),
            c => AddCheckHasNext(c, rightHasNext, false),
            AddReturnTrue,
            e => e.IfAnd(
                c => AddCheckHasNext(c, leftHasNext, true),
                c => AddCheckHasNext(c, rightHasNext, true),
                t => t.If(
                    c => AddCheckCurrent(c, leftEnumerator, rightEnumerator),
                    AddReturnFalse),
                e2 =>
                {
                    ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                    ins.Add(Instruction.Create(OpCodes.Ret));
                }));

        ins.Add(Instruction.Create(OpCodes.Br, loopBegin));
    }

    static void AddReturnFalse(Collection<Instruction> tt)
    {
        tt.Add(Instruction.Create(OpCodes.Ldc_I4_0));
        tt.Add(Instruction.Create(OpCodes.Ret));
    }

    static void AddCheckCurrent(Collection<Instruction> c, VariableDefinition leftEnumerator, VariableDefinition rightEnumerator)
    {
        c.Add(Instruction.Create(OpCodes.Ldloc, leftEnumerator));
        c.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.GetCurrent));

        c.Add(Instruction.Create(OpCodes.Ldloc, rightEnumerator));
        c.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.GetCurrent));

        c.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.StaticEquals));

        c.Add(Instruction.Create(OpCodes.Ldc_I4_0));
        c.Add(Instruction.Create(OpCodes.Ceq));
    }

    static void AddReturnTrue(Collection<Instruction> t)
    {
        t.Add(Instruction.Create(OpCodes.Ldc_I4_1));
        t.Add(Instruction.Create(OpCodes.Ret));
    }

    static void AddCheckHasNext(Collection<Instruction> ins, VariableDefinition hasNext, bool isTrue)
    {
        ins.Add(Instruction.Create(OpCodes.Ldloc, hasNext));
        ins.Add(Instruction.Create(isTrue ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1));
        ins.Add(Instruction.Create(OpCodes.Ceq));
    }

    static void AddMoveNext(Collection<Instruction> ins, VariableDefinition enumerator, VariableDefinition hasNext)
    {
        ins.Add(Instruction.Create(OpCodes.Ldloc, enumerator));
        ins.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.MoveNext));
        ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
        ins.Add(Instruction.Create(OpCodes.Ceq));
        ins.Add(Instruction.Create(OpCodes.Stloc, hasNext));
    }

    static void AddGetEnumerator(Collection<Instruction> ins, ParameterDefinition argument, VariableDefinition enumerator)
    {
        ins.Add(Instruction.Create(OpCodes.Ldarg, argument));
        ins.Add(Instruction.Create(OpCodes.Callvirt, ReferenceFinder.GetEnumerator));
        ins.Add(Instruction.Create(OpCodes.Stloc, enumerator));
    }

    static void AddRightAndNullReferenceEquals(Collection<Instruction> ins, ParameterDefinition right)
    {
        ins.If(
            c =>
            {
                ins.Add(Instruction.Create(OpCodes.Ldarg, right));
                ins.Add(Instruction.Create(OpCodes.Ldnull));
                ins.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.ReferenceEquals));
            },
            t =>
            {
                ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                ins.Add(Instruction.Create(OpCodes.Ret));
            });
    }

    static void AddLeftAndNullReferenceEquals(Collection<Instruction> ins, ParameterDefinition left)
    {
        ins.If(
            c =>
            {
                ins.Add(Instruction.Create(OpCodes.Ldarg, left));
                ins.Add(Instruction.Create(OpCodes.Ldnull));
                ins.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.ReferenceEquals));
            },
            t =>
            {
                ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                ins.Add(Instruction.Create(OpCodes.Ret));
            });
    }

    static void AddLeftAndRightReferenceEquals(Collection<Instruction> ins, ParameterDefinition left, ParameterDefinition right)
    {
        ins.If(
            c =>
            {
                ins.Add(Instruction.Create(OpCodes.Ldarg, left));
                ins.Add(Instruction.Create(OpCodes.Ldarg, right));
                ins.Add(Instruction.Create(OpCodes.Call, ReferenceFinder.ReferenceEquals));
            },
            t =>
            {
                ins.Add(Instruction.Create(OpCodes.Ldc_I4_1));
                ins.Add(Instruction.Create(OpCodes.Ret));
            });
    }

    public static OpCode GetLdArgForType(this TypeReference type)
    {
        if (type.IsValueType)
        {
            return OpCodes.Ldarga;
        }
        return OpCodes.Ldarg;
    }
}