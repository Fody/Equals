using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

public partial class ModuleWeaver
{
    public MethodDefinition InjectCollectionEquals(ModuleDefinition moduleDefinition)
    {
        var mod = 0;
        TypeDefinition typeDef;
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
        MarkAsGeneratedCode(helperType.CustomAttributes);
        helperType.BaseType = TypeSystem.ObjectReference;
        moduleDefinition.Types.Add(helperType);

        var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Static;
        var method = new MethodDefinition("CollectionEquals", methodAttributes, TypeSystem.BooleanReference);
        helperType.Methods.Add(method);

        var left = method.Parameters.Add("left", IEnumerableType);
        var right = method.Parameters.Add("right", IEnumerableType);

        var body = method.Body;
        var ins = body.Instructions;

        body.InitLocals = true;

        var leftEnumerator = body.Variables.Add(IEnumeratorType);
        var rightEnumerator = body.Variables.Add(IEnumeratorType);
        var leftHasNext = body.Variables.Add(TypeSystem.BooleanReference);
        var rightHasNext = body.Variables.Add(TypeSystem.BooleanReference);

        AddLeftAndRightReferenceEquals(ins, left, right);
        AddLeftAndNullReferenceEquals(ins, left);
        AddRightAndNullReferenceEquals(ins, right);

        AddGetEnumerator(ins, left, leftEnumerator);
        AddGetEnumerator(ins, right, rightEnumerator);

        AddCollectionLoop(ins, leftEnumerator, leftHasNext, rightEnumerator, rightHasNext);

        body.OptimizeMacros();
        MarkAsGeneratedCode(method.CustomAttributes);

        return method;
    }

    void AddCollectionLoop(Collection<Instruction> ins, VariableDefinition leftEnumerator, VariableDefinition leftHasNext, VariableDefinition rightEnumerator, VariableDefinition rightHasNext)
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
                    TypeDefinitionExtensions.AddReturnFalse),
                e2 =>
                {
                    ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                    ins.Add(Instruction.Create(OpCodes.Ret));
                }));

        ins.Add(Instruction.Create(OpCodes.Br, loopBegin));
    }

    void AddCheckCurrent(Collection<Instruction> c, VariableDefinition leftEnumerator, VariableDefinition rightEnumerator)
    {
        c.Add(Instruction.Create(OpCodes.Ldloc, leftEnumerator));
        c.Add(Instruction.Create(OpCodes.Callvirt, GetCurrent));

        c.Add(Instruction.Create(OpCodes.Ldloc, rightEnumerator));
        c.Add(Instruction.Create(OpCodes.Callvirt, GetCurrent));

        c.Add(Instruction.Create(OpCodes.Call, StaticEquals));

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

    void AddMoveNext(Collection<Instruction> ins, VariableDefinition enumerator, VariableDefinition hasNext)
    {
        ins.Add(Instruction.Create(OpCodes.Ldloc, enumerator));
        ins.Add(Instruction.Create(OpCodes.Callvirt, MoveNext));
        ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
        ins.Add(Instruction.Create(OpCodes.Ceq));
        ins.Add(Instruction.Create(OpCodes.Stloc, hasNext));
    }

    void AddGetEnumerator(Collection<Instruction> ins, ParameterDefinition argument, VariableDefinition enumerator)
    {
        ins.Add(Instruction.Create(OpCodes.Ldarg, argument));
        ins.Add(Instruction.Create(OpCodes.Callvirt, GetEnumerator));
        ins.Add(Instruction.Create(OpCodes.Stloc, enumerator));
    }

    void AddRightAndNullReferenceEquals(Collection<Instruction> ins, ParameterDefinition right)
    {
        ins.If(
            c =>
            {
                ins.Add(Instruction.Create(OpCodes.Ldarg, right));
                ins.Add(Instruction.Create(OpCodes.Ldnull));
                ins.Add(Instruction.Create(OpCodes.Call, ReferenceEquals));
            },
            t =>
            {
                ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                ins.Add(Instruction.Create(OpCodes.Ret));
            });
    }

    void AddLeftAndNullReferenceEquals(Collection<Instruction> ins, ParameterDefinition left)
    {
        ins.If(
            c =>
            {
                ins.Add(Instruction.Create(OpCodes.Ldarg, left));
                ins.Add(Instruction.Create(OpCodes.Ldnull));
                ins.Add(Instruction.Create(OpCodes.Call, ReferenceEquals));
            },
            t =>
            {
                ins.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                ins.Add(Instruction.Create(OpCodes.Ret));
            });
    }

    void AddLeftAndRightReferenceEquals(Collection<Instruction> ins, ParameterDefinition left, ParameterDefinition right)
    {
        ins.If(
            c =>
            {
                ins.Add(Instruction.Create(OpCodes.Ldarg, left));
                ins.Add(Instruction.Create(OpCodes.Ldarg, right));
                ins.Add(Instruction.Create(OpCodes.Call, ReferenceEquals));
            },
            t =>
            {
                ins.Add(Instruction.Create(OpCodes.Ldc_I4_1));
                ins.Add(Instruction.Create(OpCodes.Ret));
            });
    }
}