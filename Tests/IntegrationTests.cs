using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using NUnit.Framework;


[TestFixture]
public class IntegrationTests
{
    Assembly assembly;
    List<string> warnings = new List<string>();
    string beforeAssemblyPath;
    string afterAssemblyPath;

    public IntegrationTests()
    {
        beforeAssemblyPath = Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll");
#if (!DEBUG)

        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif

        afterAssemblyPath = beforeAssemblyPath.Replace(".dll", "2.dll");
        File.Copy(beforeAssemblyPath, afterAssemblyPath, true);
        
        var assemblyResolver = new MockAssemblyResolver
            {
                Directory = Path.GetDirectoryName(beforeAssemblyPath)
            };
        var moduleDefinition = ModuleDefinition.ReadModule(afterAssemblyPath,new ReaderParameters
            {
                AssemblyResolver = assemblyResolver
            });
        var weavingTask = new ModuleWeaver
                              {
                                  ModuleDefinition = moduleDefinition,
                                  AssemblyResolver = assemblyResolver,
                              };

        weavingTask.Execute();
        moduleDefinition.Write(afterAssemblyPath);

        assembly = Assembly.LoadFile(afterAssemblyPath);
    }

    #region operators

    [Test]
    public void Equality_operator_should_return_true_for_equal_class_instances()
    {
        var type = assembly.GetType("OnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 2;

        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Test]
    public void Equality_operator_should_return_false_for_not_class_equal_instances()
    {
        var type = assembly.GetType("OnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 3;

        Assert.True(first != second);
        Assert.False(first == second);
    }

    [Test]
    public void Equality_operator_should_return_true_for_equal_struct_instances()
    {
        var type = assembly.GetType("StructWithOnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 2;

        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Test]
    public void Equality_operator_should_return_false_for_not_equal_struct_instances()
    {
        var type = assembly.GetType("StructWithOnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 3;

        Assert.True(first != second);
        Assert.False(first == second);
    }

    #endregion

    #region GetHashCode

    [Test]
    public void GetHashCode_should_return_value_for_empty_type()
    {
        var type = assembly.GetType("EmptyClass");
        dynamic instance = Activator.CreateInstance(type);

        var result = instance.GetHashCode();

        Assert.AreEqual(0, result);
    }

    [Test]
    public void GetHashCode_should_return_value_for_null_string()
    {
        var type = assembly.GetType("SimpleClass");
        dynamic instance = Activator.CreateInstance(type);
        instance.Text = null;

        var result = instance.GetHashCode();
        
        Assert.AreEqual(0, result);
    }

    [Test]
    public void GetHashCode_should_return_diffrent_value_for_changed_property_in_base_class()
    {
        var type = assembly.GetType("InheritedClass");
        dynamic instance = Activator.CreateInstance(type);
        instance.A = 1;
        instance.B = 2;

        var firstResult = instance.GetHashCode();
        instance.A = 3;
        var secondResult = instance.GetHashCode();

        Assert.AreNotEqual(firstResult, secondResult);
    }

    [Test]
    public void GetHashCode_should_return_value_for_struct()
    {
        var type = assembly.GetType("SimpleStruct");
        dynamic instance = Activator.CreateInstance(type);
        instance.X = 1;
        instance.Y = 2;

        var result = instance.GetHashCode();

        Assert.AreNotEqual(0, result);
    }

    [Test]
    public void GetHashCode_should_return_value_for_normal_class()
    {
        var type = assembly.GetType("NormalClass");
        dynamic instance = Activator.CreateInstance(type);
        instance.X = 1;
        instance.Y = "2";
        instance.Z = 4.5;
        instance.V = 'C';

        var result = instance.GetHashCode();

        Assert.AreNotEqual(0, result);
    }

    [Test]
    public void GetHashCode_should_should_ignored_markder_properties()
    {
        var type = assembly.GetType("IgnoredPropertiesClass");
        dynamic instance = Activator.CreateInstance(type);
        instance.X = 1;
        instance.Y = 2;

        var firstResult = instance.GetHashCode();
        instance.Y = 3;
        var secondResult = instance.GetHashCode();

        Assert.AreEqual(firstResult, secondResult);
    }

    [Test]
    public void GetHashCode_should_return_value_for_array()
    {
        var type = assembly.GetType("IntCollection");
        dynamic instance = Activator.CreateInstance(type);
        instance.Collection = new int[] { 1, 2, 3, 4, 5, 6 };
        instance.Count = 2;

        var result = instance.GetHashCode();

        Assert.AreNotEqual(0, result);
    }

    [Test]
    public void GetHashCode_should_return_value_for_null_array()
    {
        var type = assembly.GetType("IntCollection");
        dynamic instance = Activator.CreateInstance(type);
        instance.Collection = null;
        instance.Count = 0;

        var result = instance.GetHashCode();

        Assert.AreEqual(0, result);
    }

    [Test]
    public void GetHashCode_should_return_value_for_empty_array()
    {
        var type = assembly.GetType("IntCollection");
        dynamic instance = Activator.CreateInstance(type);
        instance.Collection = new int[0];
        instance.Count = 0;

        var result = instance.GetHashCode();

        Assert.AreEqual(0, result);
    }

    [Test]
    public void GetHashCode_should_return_value_for_type_with_only_array()
    {
        var type = assembly.GetType("OnlyIntCollection");
        dynamic instance = Activator.CreateInstance(type);
        instance.Collection = new int[] { 1, 2, 3, 4, 5};

        var result = instance.GetHashCode();

        Assert.AreNotEqual(0, result);
    }

    [Test]
    public void GetHashCode_should_return_value_for_generic_class()
    {
        var genericClassType = assembly.GetType("GenericClass`1");
        var propType = assembly.GetType("GenericClassNormalClass");
        var instanceType = genericClassType.MakeGenericType(propType);

        dynamic instance = Activator.CreateInstance(instanceType);
        instance.A = 1;

        dynamic propInstance = Activator.CreateInstance(propType);

        dynamic array = Activator.CreateInstance(propType.MakeArrayType(), new object[] { 1 });
        array[0] = propInstance;

        instance.B = array;

        var result = instance.GetHashCode();

        Assert.AreNotEqual(0, result);
    }

    [Test]
    public void GetHashCode_should_return_value_for_enums()
    {
        var type = assembly.GetType("EnumClass");
        dynamic instance = Activator.CreateInstance(type, new object[] { 3, 6 });

        var result = instance.GetHashCode();

        Assert.AreNotEqual(0, result);
    }

    [Test]
    public void GetHashCode_should_return_value_for_nested_class()
    {
        var normalType = assembly.GetType("NormalClass");
        dynamic noramlInstance = Activator.CreateInstance(normalType);
        noramlInstance.X = 1;
        noramlInstance.Y = "2";
        noramlInstance.Z = 4.5;
        noramlInstance.V = 'V';
        var nestedType = assembly.GetType("NestedClass");
        dynamic nestedInstance = Activator.CreateInstance(nestedType);
        nestedInstance.A = 10;
        nestedInstance.B = "11";
        nestedInstance.C = 12.25;
        nestedInstance.D = noramlInstance;

        var result = nestedInstance.GetHashCode();

        Assert.AreNotEqual(0, result);
    }

    [Test]
    public void GetHashCode_shoud_return_value_for_class_without_generic_parameter()
    {
        var withoutGenericParameterType = assembly.GetType("WithoutGenericParameter");
        var propType = assembly.GetType("GenericClassNormalClass");

        dynamic instance = Activator.CreateInstance(withoutGenericParameterType);
        instance.Z = 12;
        instance.A = 1;
        dynamic propInstance = Activator.CreateInstance(propType);
        dynamic array = Activator.CreateInstance(propType.MakeArrayType(), new object[] {1});
        array[0] = propInstance;
        instance.B = array;

        var result = instance.GetHashCode();

        Assert.AreNotEqual(0, result);
    }

    [Test]
    public void GetHashCode_shoud_return_value_for_class_with_generic_parameter()
    {
        var withGenericParameterType = assembly.GetType("WithGenericParameter`1");
        var propType = assembly.GetType("GenericClassNormalClass");
        var instanceType = withGenericParameterType.MakeGenericType(propType);

        dynamic instance = Activator.CreateInstance(instanceType);
        instance.X = 12;
        instance.A = 1;
        dynamic propInstance = Activator.CreateInstance(propType);
        dynamic array = Activator.CreateInstance(propType.MakeArrayType(), new object[] {1});
        array[0] = propInstance;
        instance.B = array;

        var result = instance.GetHashCode();

        Assert.AreNotEqual(0, result);
    }
    
    #endregion

    #region Equals

    [Test]
    public void Equals_shoud_return_value_for_class_without_generic_parameter()
    {
        var withoutGenericParameterType = assembly.GetType("WithoutGenericParameter");
        var propType = assembly.GetType("GenericClassNormalClass");

        dynamic instance = Activator.CreateInstance(withoutGenericParameterType);
        instance.Z = 12;
        instance.A = 1;
        dynamic propInstance = Activator.CreateInstance(propType);
        dynamic array = Activator.CreateInstance(propType.MakeArrayType(), new object[] { 1 });
        array[0] = propInstance;
        instance.B = array;

        dynamic instance2 = Activator.CreateInstance(withoutGenericParameterType);
        instance2.Z = 12;
        instance2.A = 1;
        dynamic array2 = Activator.CreateInstance(propType.MakeArrayType(), new object[] { 1 });
        dynamic propInstance2 = Activator.CreateInstance(propType);
        array2[0] = propInstance2;
        instance2.B = array2;

        var result = instance.Equals(instance2);

        Assert.True(result);
    }

    [Test]
    public void Equals_shoud_return_value_for_class_with_generic_parameter()
    {
        var withGenericParameterType = assembly.GetType("WithGenericParameter`1");
        var propType = assembly.GetType("GenericClassNormalClass");
        var instanceType = withGenericParameterType.MakeGenericType(propType);

        dynamic instance = Activator.CreateInstance(instanceType);
        instance.X = 12;
        instance.A = 1;
        dynamic propInstance = Activator.CreateInstance(propType);
        dynamic array = Activator.CreateInstance(propType.MakeArrayType(), new object[] { 1 });
        array[0] = propInstance;
        instance.B = array;

        dynamic instance2 = Activator.CreateInstance(instanceType);
        instance2.X = 12;
        instance2.A = 1;
        dynamic propInstance2 = Activator.CreateInstance(propType);
        dynamic array2 = Activator.CreateInstance(propType.MakeArrayType(), new object[] { 1 });
        array2[0] = propInstance2;
        instance2.B = array;

        var result = instance.Equals(instance2);

        Assert.AreNotEqual(0, result);
    }

    private bool CheckEqualityOnTypesForTypeCheck(string left, string right)
    {
        var leftType = assembly.GetType(left);
        dynamic leftInstance = Activator.CreateInstance(leftType);
        leftInstance.A = 1;

        var rightType = assembly.GetType(right);
        dynamic rightInstance = Activator.CreateInstance(rightType);
        rightInstance.A = 1;

        var result = leftInstance.Equals((object) rightInstance);
        return result;
    }

    [Test]
    public void Equals_should_use_type_check_option()
    {
        Assert.True(this.CheckEqualityOnTypesForTypeCheck("EqualsOrSubtypeClass", "EqualsOrSubtypeClass"));
        Assert.True(this.CheckEqualityOnTypesForTypeCheck("EqualsOrSubtypeClass", "EqualsOrSubtypeSubClass"));
        Assert.True(this.CheckEqualityOnTypesForTypeCheck("EqualsOrSubtypeSubClass", "EqualsOrSubtypeClass"));
        Assert.True(this.CheckEqualityOnTypesForTypeCheck("EqualsOrSubtypeSubClass", "EqualsOrSubtypeSubClass"));

        Assert.True(this.CheckEqualityOnTypesForTypeCheck("ExaclyOfTypeClass", "ExaclyOfTypeClass"));
        Assert.False(this.CheckEqualityOnTypesForTypeCheck("ExaclyOfTypeSubClass", "ExaclyOfTypeClass"));
        Assert.True(this.CheckEqualityOnTypesForTypeCheck("ExaclyOfTypeClass", "ExaclyOfTypeSubClass"));
        Assert.False(this.CheckEqualityOnTypesForTypeCheck("ExaclyOfTypeSubClass", "ExaclyOfTypeSubClass"));

        Assert.True(this.CheckEqualityOnTypesForTypeCheck("ExaclyTheSameTypeAsThisClass", "ExaclyTheSameTypeAsThisClass"));
        Assert.False(this.CheckEqualityOnTypesForTypeCheck("ExaclyTheSameTypeAsThisClass", "ExaclyTheSameTypeAsThisSubClass"));
        Assert.False(this.CheckEqualityOnTypesForTypeCheck("ExaclyTheSameTypeAsThisSubClass", "ExaclyTheSameTypeAsThisClass"));
        Assert.True(this.CheckEqualityOnTypesForTypeCheck("ExaclyTheSameTypeAsThisSubClass", "ExaclyTheSameTypeAsThisSubClass"));
    }

    [Test]
    public void Equals_should_return_true_for_empty_type()
    {
        var type = assembly.GetType("EmptyClass");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Test]
    public void Equals_should_return_true_for_enums()
    {
        var type = assembly.GetType("EnumClass");
        dynamic first = Activator.CreateInstance(type, new object[] { 3, 6 });
        dynamic second = Activator.CreateInstance(type, new object[] { 3, 6 });

        var result = ((object)first).Equals((object)second);

        Assert.True(result);
    }

    [Test]
    public void Equals_should_return_true_for_generic_class()
    {
        var genericClassType = assembly.GetType("GenericClass`1");
        var propType = assembly.GetType("GenericClassNormalClass");
        var instanceType = genericClassType.MakeGenericType(propType);

        Func<dynamic> createInstance = () =>
            {
                dynamic instance = Activator.CreateInstance(instanceType);
                instance.A = 1;
                dynamic propInstance = Activator.CreateInstance(propType);

                dynamic array = Activator.CreateInstance(propType.MakeArrayType(), new object[] {1});
                array[0] = propInstance;

                instance.B = array;
                return instance;
            };
        var first = createInstance();
        var second = createInstance();

        var result = first.Equals((object)second);

        Assert.True(result);
    }

    [Test]
    public void Equals_should_should_ignored_markder_properties()
    {
        var type = assembly.GetType("IgnoredPropertiesClass");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;
        first.Y = 2;

        dynamic second = Activator.CreateInstance(type);
        second.X = 1;
        second.Y = 3;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Test]
    public void Equals_should_return_false_for_diffrent_value_for_changed_property_in_base_class()
    {
        var type = assembly.GetType("InheritedClass");
        dynamic first = Activator.CreateInstance(type);
        first.A = 1;
        first.B = 2;

        dynamic second = Activator.CreateInstance(type);
        second.A = 3;
        second.B = 2;

        var result = first.Equals(second);

        Assert.False(result);
    }

    [Test]
    public void Equals_should_return_true_for_equal_collections()
    {
        var type = assembly.GetType("IntCollection");
        dynamic first = Activator.CreateInstance(type);
        first.Collection = new int[] { 1, 2, 3, 4, 5, 6 };
        first.Count = 2;

        dynamic second = Activator.CreateInstance(type);
        second.Collection = new List<int>() { 1, 2, 3, 4, 5, 6 };
        second.Count = 2;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Test]
    public void Equals_should_return_true_for_reference_equal_array()
    {
        var type = assembly.GetType("IntCollection");
        dynamic first = Activator.CreateInstance(type);
        first.Collection = new int[] { 1, 2, 3, 4, 5, 6 };
        first.Count = 2;

        dynamic second = Activator.CreateInstance(type);
        second.Collection = first.Collection;
        second.Count = 2;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Test]
    public void Equals_should_return_true_for_null_array()
    {
        var type = assembly.GetType("IntCollection");
        dynamic first = Activator.CreateInstance(type);
        first.Collection = null;
        first.Count = 0;

        dynamic second = Activator.CreateInstance(type);
        second.Collection = null;
        second.Count = 0;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Test]
    public void Equals_should_return_false_for_null_array_and_fill_array()
    {
        var type = assembly.GetType("IntCollection");
        dynamic first = Activator.CreateInstance(type);
        first.Collection = new int[]{1};
        first.Count = 0;

        dynamic second = Activator.CreateInstance(type);
        second.Collection = null;
        second.Count = 0;

        var result = first.Equals(second);

        Assert.False(result);
    }

    [Test]
    public void Equals_should_return_false_for_fill_array_and_null_array()
    {
        var type = assembly.GetType("IntCollection");
        dynamic first = Activator.CreateInstance(type);
        first.Collection = null;
        first.Count = 0;

        dynamic second = Activator.CreateInstance(type);
        second.Collection = new int[] { 1 };
        second.Count = 0;

        var result = first.Equals(second);

        Assert.False(result);
    }

    [Test]
    public void Equals_should_return_true_for_nested_class()
    {
        var nestedInstancFirst = GetNestedClassInstance();
        var nestedInstancSecond = GetNestedClassInstance();

        var result = nestedInstancFirst.Equals(nestedInstancSecond);

        Assert.True(result);
    }

    [Test]
    public void Equals_should_return_false_for_changed_nested_class()
    {
        var nestedInstancFirst = GetNestedClassInstance();
        var nestedInstancSecond = GetNestedClassInstance();
        nestedInstancSecond.D.X = 11;

        var result = nestedInstancFirst.Equals(nestedInstancSecond);

        Assert.False(result);
    }

    [Test]
    public void Equals_should_return_false_for_null_nested_class()
    {
        var nestedInstancFirst = GetNestedClassInstance();
        var nestedInstancSecond = GetNestedClassInstance();
        nestedInstancSecond.D = null;

        var result = nestedInstancFirst.Equals(nestedInstancSecond);

        Assert.False(result);
    }

    private dynamic GetNestedClassInstance()
    {
        var normalType = assembly.GetType("NormalClass");
        dynamic noramlInstance = Activator.CreateInstance(normalType);
        noramlInstance.X = 1;
        noramlInstance.Y = "2";
        noramlInstance.Z = 4.5;
        noramlInstance.V = 'V';
        var nestedType = assembly.GetType("NestedClass");
        dynamic nestedInstance = Activator.CreateInstance(nestedType);
        nestedInstance.A = 10;
        nestedInstance.B = "11";
        nestedInstance.C = 12.25;
        nestedInstance.D = noramlInstance;
        return nestedInstance;
    }

    [Test]
    public void Equals_should_return_true_for_equal_structs()
    {
        var type = assembly.GetType("SimpleStruct");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;
        first.Y = 2;
        dynamic second = Activator.CreateInstance(type);
        second.X = 1;
        second.Y = 2;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Test]
    public void Equals_should_return_false_for_changed_struct()
    {
        var type = assembly.GetType("SimpleStruct");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;
        first.Y = 2;
        dynamic second = Activator.CreateInstance(type);
        second.X = 1;
        second.Y = 3;

        var result = first.Equals(second);

        Assert.False(result);
    }

    [Test]
    public void Equals_should_return_true_for_equal_struct_property()
    {
        var type = assembly.GetType("StructPropertyClass");
        var propertyType = assembly.GetType("SimpleStruct");
        dynamic first = Activator.CreateInstance(type);
        first.A = 1;
        dynamic firstProperty = Activator.CreateInstance(propertyType);
        first.B = firstProperty;
        first.B.X = 2;
        first.B.X = 3;
        dynamic second = Activator.CreateInstance(type);
        second.A = 1;
        dynamic secondProperty = Activator.CreateInstance(propertyType);
        second.B = secondProperty;
        second.B.X = 2;
        second.B.X = 3;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Test]
    public void Equals_should_return_true_for_equal_normal_class()
    {
        var type = assembly.GetType("NormalClass");
        dynamic instance = Activator.CreateInstance(type);
        instance.X = 1;
        instance.Y = "2";
        instance.Z = 4.5;
        instance.V = 'C';

        object first = instance;

        instance = Activator.CreateInstance(type);
        instance.X = 1;
        instance.Y = "2";
        instance.Z = 4.5;
        instance.V = 'C';

        object second = instance;

        var result1 = ((dynamic)first).Equals((dynamic)second);
        var result = first.Equals(second);

        Assert.True(result);
    }

    #endregion
}