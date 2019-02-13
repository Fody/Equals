using System;
using System.Collections.Generic;
using Fody;
using Xunit;

public partial class IntegrationTests
{
#pragma warning disable 618
    static TestResult testResult;
#pragma warning restore 618

    static IntegrationTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
    }

    #region operators

    [Fact]
    public void Equality_operator_should_return_true_for_equal_class_instances()
    {
        var type = testResult.Assembly.GetType("OnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 2;

        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equality_operator_should_return_false_for_not_class_equal_instances()
    {
        var type = testResult.Assembly.GetType("OnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 3;

        Assert.True(first != second);
        Assert.False(first == second);
    }

    [Fact]
    public void Equality_operator_should_return_true_for_equal_struct_instances()
    {
        var type = testResult.Assembly.GetType("StructWithOnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 2;

        Assert.True(first == second);
        Assert.False(first != second);
    }


    [Fact]
    public void Equality_operator_should_return_true_for_equal_class_with_generic_property()
    {
        var genericClassType = testResult.Assembly.GetType("GenericProperty`1");
        var propType = typeof(int);
        var type = genericClassType.MakeGenericType(propType);

        dynamic first = Activator.CreateInstance(type);
        first.Prop = 1;
        dynamic second = Activator.CreateInstance(type);
        second.Prop = 1;

        Assert.True(first == second);
#pragma warning disable CS1718 // Comparison made to same variable
        Assert.False(first != first);
#pragma warning restore CS1718 // Comparison made to same variable
    }

    [Fact]
    public void Equality_operator_should_return_false_for_not_equal_struct_instances()
    {
        var type = testResult.Assembly.GetType("StructWithOnlyOperator");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Value = 1;
        second.Value = 3;

        Assert.True(first != second);
        Assert.False(first == second);
    }

    [Fact]
    public void Equality_operator_should_return_true_for_equal_guid_instances()
    {
        var type = testResult.Assembly.GetType("GuidClass");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        var newGuid = Guid.NewGuid();
        first.Key = newGuid;
        second.Key = newGuid;

        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equality_operator_should_return_true_for_empty_object_collections()
    {
        var type = testResult.Assembly.GetType("ObjectCollection");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Collection = new object[]
        {
        };
        second.Collection = new object[]
        {
        };

        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equality_operator_should_return_true_for_equal_object_collections()
    {
        var type = testResult.Assembly.GetType("ObjectCollection");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Collection = new object[]
        {
            "foo",
            1.23456
        };
        second.Collection = new object[]
        {
            "foo",
            1.23456
        };

        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equality_operator_should_return_false_for_collections_with_different_size()
    {
        var type = testResult.Assembly.GetType("ObjectCollection");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Collection = new object[]
        {
            "foo",
            1.23456,
            1
        };
        second.Collection = new object[]
        {
            "foo",
            1.23456
        };

        Assert.True(first != second);
        Assert.False(first == second);
        Assert.True(second != first);
        Assert.False(second == first);
    }

    [Fact]
    public void Equality_operator_should_return_false_for_collections_with_elements_and_empty_collection()
    {
        var type = testResult.Assembly.GetType("ObjectCollection");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Collection = new object[]
        {
        };
        second.Collection = new object[]
        {
            "foo",
            1.23456
        };

        Assert.True(first != second);
        Assert.False(first == second);
        Assert.True(second != first);
        Assert.False(second == first);
    }

    [Fact]
    public void Equality_operator_should_return_false_for_different_object_collections()
    {
        var type = testResult.Assembly.GetType("ObjectCollection");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        first.Collection = new object[]
        {
            "foo",
            1.23456
        };
        second.Collection = new object[]
        {
            "bar",
            65432.1
        };

        Assert.True(first != second);
        Assert.False(first == second);
    }

    [Fact]
    public void Equality_should_return_false_for_class_with_method_to_remove()
    {
        var type = testResult.Assembly.GetType("ClassWithMethodToRemove");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;
        first.Y = 2;

        dynamic second = Activator.CreateInstance(type);
        second.X = 1;
        second.Y = 3;

        Assert.True(first != second);
        Assert.False(first == second);
    }

    #endregion

    #region GetHashCode

    [Fact]
    public void GetHashCode_should_return_value_for_class_with_generic_property()
    {
        var genericClassType = testResult.Assembly.GetType("GenericProperty`1");
        var propType = typeof(int);
        var type = genericClassType.MakeGenericType(propType);

        dynamic instance = Activator.CreateInstance(type);
        instance.Prop = 1;

        Assert.NotEqual(0, instance.GetHashCode());
    }

    [Fact]
    public void GetHashCode_should_return_value_for_empty_type()
    {
        var type = testResult.Assembly.GetType("EmptyClass");
        dynamic instance = Activator.CreateInstance(type);

        var result = instance.GetHashCode();

        Assert.Equal(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_null_string()
    {
        var type = testResult.Assembly.GetType("SimpleClass");
        dynamic instance = Activator.CreateInstance(type);
        instance.Text = null;

        var result = instance.GetHashCode();

        Assert.Equal(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_null_nullable()
    {
        var type = testResult.Assembly.GetType("ClassWithNullable");
        dynamic instance = Activator.CreateInstance(type);
        instance.NullableDate = null;

        var result = instance.GetHashCode();

        Assert.Equal(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_date_nullable()
    {
        var type = testResult.Assembly.GetType("ClassWithNullable");
        dynamic instance = Activator.CreateInstance(type);
        instance.NullableDate = new DateTime(1988, 5, 23);

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_different_value_for_changed_property_in_base_class()
    {
        var type = testResult.Assembly.GetType("InheritedClass");
        dynamic instance = Activator.CreateInstance(type);
        instance.A = 1;
        instance.B = 2;

        var firstResult = instance.GetHashCode();
        instance.A = 3;
        var secondResult = instance.GetHashCode();

        Assert.NotEqual(firstResult, secondResult);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_struct()
    {
        var type = testResult.Assembly.GetType("SimpleStruct");
        dynamic instance = Activator.CreateInstance(type);
        instance.X = 1;
        instance.Y = 2;

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_guid_class()
    {
        var type = testResult.Assembly.GetType("GuidClass");
        dynamic instance = Activator.CreateInstance(type);
        instance.Key = Guid.NewGuid();

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_normal_class()
    {
        var type = testResult.Assembly.GetType("NormalClass");
        dynamic instance = Activator.CreateInstance(type);
        instance.X = 1;
        instance.Y = "2";
        instance.Z = 4.5;
        instance.V = 'C';

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_should_ignored_marked_properties()
    {
        var type = testResult.Assembly.GetType("IgnoredPropertiesClass");
        dynamic instance = Activator.CreateInstance(type);
        instance.X = 1;
        instance.Y = 2;

        var firstResult = instance.GetHashCode();
        instance.Y = 3;
        var secondResult = instance.GetHashCode();

        Assert.Equal(firstResult, secondResult);
    }

    [Fact]
    public void GetHashCode_should_should_ignored_inherited_marked_properties()
    {
        var type = testResult.Assembly.GetType("InheritedIgnoredPropertiesClass");
        dynamic instance = Activator.CreateInstance(type);
        instance.X = 1;
        instance.Y = 2;

        var firstResult = instance.GetHashCode();
        instance.Y = 3;
        var secondResult = instance.GetHashCode();

        Assert.Equal(firstResult, secondResult);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_array()
    {
        var type = testResult.Assembly.GetType("IntCollection");
        dynamic instance = Activator.CreateInstance(type);
        instance.Collection = new[] { 1, 2, 3, 4, 5, 6 };
        instance.Count = 2;

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_int_array()
    {
        var type = testResult.Assembly.GetType("IntArray");
        dynamic instance = Activator.CreateInstance(type);
        instance.Collection = new[] { 1, 2, 3};

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }
    
    [Fact]
    public void GetHashCode_should_return_value_for_string_array()
    {
        var type = testResult.Assembly.GetType("StringArray");
        dynamic instance = Activator.CreateInstance(type);
        instance.Collection = new[] { "one", "two", "three"};

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_null_array()
    {
        var type = testResult.Assembly.GetType("IntCollection");
        dynamic instance = Activator.CreateInstance(type);
        instance.Collection = null;
        instance.Count = 0;

        var result = instance.GetHashCode();

        Assert.Equal(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_empty_array()
    {
        var type = testResult.Assembly.GetType("IntCollection");
        dynamic instance = Activator.CreateInstance(type);
        instance.Collection = new int[0];
        instance.Count = 0;

        var result = instance.GetHashCode();

        Assert.Equal(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_type_with_only_array()
    {
        var type = testResult.Assembly.GetType("OnlyIntCollection");
        dynamic instance = Activator.CreateInstance(type);
        instance.Collection = new[] { 1, 2, 3, 4, 5};

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_generic_class()
    {
        var genericClassType = testResult.Assembly.GetType("GenericClass`1");
        var propType = testResult.Assembly.GetType("GenericClassNormalClass");
        var instanceType = genericClassType.MakeGenericType(propType);

        dynamic instance = Activator.CreateInstance(instanceType);
        instance.A = 1;

        dynamic propInstance = Activator.CreateInstance(propType);

        dynamic array = Activator.CreateInstance(propType.MakeArrayType(), new object[] { 1 });
        array[0] = propInstance;

        instance.B = array;

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_enums()
    {
        var type = testResult.Assembly.GetType("EnumClass");
        dynamic instance = Activator.CreateInstance(type, new object[] { 3, 6 });

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_nested_class()
    {
        var normalType = testResult.Assembly.GetType("NormalClass");
        dynamic normalInstance = Activator.CreateInstance(normalType);
        normalInstance.X = 1;
        normalInstance.Y = "2";
        normalInstance.Z = 4.5;
        normalInstance.V = 'V';
        var nestedType = testResult.Assembly.GetType("NestedClass");
        dynamic nestedInstance = Activator.CreateInstance(nestedType);
        nestedInstance.A = 10;
        nestedInstance.B = "11";
        nestedInstance.C = 12.25;
        nestedInstance.D = normalInstance;

        var result = nestedInstance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_class_without_generic_parameter()
    {
        var withoutGenericParameterType = testResult.Assembly.GetType("WithoutGenericParameter");
        var propType = testResult.Assembly.GetType("GenericClassNormalClass");

        dynamic instance = Activator.CreateInstance(withoutGenericParameterType);
        instance.Z = 12;
        instance.A = 1;
        dynamic propInstance = Activator.CreateInstance(propType);
        dynamic array = Activator.CreateInstance(propType.MakeArrayType(), new object[] {1});
        array[0] = propInstance;
        instance.B = array;

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_class_with_generic_parameter()
    {
        var withGenericParameterType = testResult.Assembly.GetType("WithGenericParameter`1");
        var propType = testResult.Assembly.GetType("GenericClassNormalClass");
        var instanceType = withGenericParameterType.MakeGenericType(propType);

        dynamic instance = Activator.CreateInstance(instanceType);
        instance.X = 12;
        instance.A = 1;
        dynamic propInstance = Activator.CreateInstance(propType);
        dynamic array = Activator.CreateInstance(propType.MakeArrayType(), new object[] {1});
        array[0] = propInstance;
        instance.B = array;

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_class_with_static_properties()
    {
        var type = testResult.Assembly.GetType("ClassWithStaticProperties");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;
        first.Y = "2";

        var result = first.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_class_with_indexer()
    {
        var type = testResult.Assembly.GetType("ClassWithIndexer");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;
        first.Y = 2;

        var result = first.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_class_with_method_to_remove()
    {
        var type = testResult.Assembly.GetType("ClassWithMethodToRemove");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;
        first.Y = 2;

        var result = first.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_class_with_guid_in_parent()
    {
        var guid = "{f6ab1abe-5811-40e9-8154-35776d2e5106}";

        var type = testResult.Assembly.GetType( "ReferenceObject" );
        dynamic first = Activator.CreateInstance( type );
        first.Name = "Test";
        first.Id = Guid.Parse( guid );

        var result = first.GetHashCode();

        Assert.NotEqual( 0, result );
    }

    [Fact]
    public void GetHashCode_should_return_value_for_class_with_generic_property2()
    {
        var type = testResult.Assembly.GetType("ClassWithGenericProperty");
        dynamic first = Activator.CreateInstance(type);
        first.Prop = new GenericDependency<int> {Prop = 1};

        var result = first.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_ignore_properties_in_base_class_when_class_is_marked()
    {
        var type = testResult.Assembly.GetType("IgnoreBaseClass");

        dynamic instance = Activator.CreateInstance(type);
        instance.A = 1;
        instance.B = 2;

        dynamic instance2 = Activator.CreateInstance(type);
        instance2.A = 3;
        instance2.B = 2;

        var first = instance.GetHashCode();
        var second = instance2.GetHashCode();

        Assert.Equal(first, second);
    }

    #endregion

    #region Equals

    [Fact]
    public void Equals_should_return_value_for_class_with_generic_property()
    {
        var genericClassType = testResult.Assembly.GetType("GenericProperty`1");
        var propType = typeof(int);
        var type = genericClassType.MakeGenericType(propType);

        dynamic first = Activator.CreateInstance(type);
        first.Prop = 1;
        dynamic second = Activator.CreateInstance(type);
        second.Prop = 1;
        dynamic third = Activator.CreateInstance(type);
        third.Prop = 2;

        Assert.True(first.Equals(second));
        Assert.False(first.Equals(third));
    }

    [Fact]
    public void Equals_should_return_value_for_class_without_generic_parameter()
    {
        var withoutGenericParameterType = testResult.Assembly.GetType("WithoutGenericParameter");
        var propType = testResult.Assembly.GetType("GenericClassNormalClass");

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

    [Fact]
    public void Equals_should_return_value_for_class_with_generic_parameter()
    {
        var withGenericParameterType = testResult.Assembly.GetType("WithGenericParameter`1");
        var propType = testResult.Assembly.GetType("GenericClassNormalClass");
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

        bool result = instance.Equals(instance2);

        Assert.True(result);
    }

    bool CheckEqualityOnTypesForTypeCheck(string left, string right)
    {
        var leftType = testResult.Assembly.GetType(left);
        dynamic leftInstance = Activator.CreateInstance(leftType);
        leftInstance.A = 1;

        var rightType = testResult.Assembly.GetType(right);
        dynamic rightInstance = Activator.CreateInstance(rightType);
        rightInstance.A = 1;

        return leftInstance.Equals((object) rightInstance);
    }

    [Theory]
    [InlineData("EqualsOrSubtypeClass", "EqualsOrSubtypeClass", true)]
    [InlineData("EqualsOrSubtypeClass", "EqualsOrSubtypeSubClass", true)]
    [InlineData("EqualsOrSubtypeSubClass", "EqualsOrSubtypeClass", true)]
    [InlineData("EqualsOrSubtypeSubClass", "EqualsOrSubtypeSubClass", true)]
    [InlineData("ExactlyOfTypeClass", "ExactlyOfTypeClass", true)]
    [InlineData("ExactlyOfTypeSubClass", "ExactlyOfTypeClass", false)]
    [InlineData("ExactlyOfTypeClass", "ExactlyOfTypeSubClass", true)]
    [InlineData("ExactlyOfTypeSubClass", "ExactlyOfTypeSubClass", false)]
    [InlineData("ExactlyTheSameTypeAsThisClass", "ExactlyTheSameTypeAsThisClass", true)]
    [InlineData("ExactlyTheSameTypeAsThisClass", "ExactlyTheSameTypeAsThisSubClass", false)]
    [InlineData("ExactlyTheSameTypeAsThisSubClass", "ExactlyTheSameTypeAsThisClass", false)]
    //TODO: support sub classes
    //[InlineData("ExactlyTheSameTypeAsThisSubClass", "ExactlyTheSameTypeAsThisSubClass", true)]
    public void Equals_should_use_type_check_option(string left, string right, bool result)
    {
        Assert.Equal(result, CheckEqualityOnTypesForTypeCheck(left, right));
    }

    [Fact]
    public void Equals_should_return_true_for_empty_type()
    {
        var type = testResult.Assembly.GetType("EmptyClass");
        dynamic first = Activator.CreateInstance(type);
        dynamic second = Activator.CreateInstance(type);

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_return_true_for_enums()
    {
        var type = testResult.Assembly.GetType("EnumClass");
        dynamic first = Activator.CreateInstance(type, new object[] { 3, 6 });
        dynamic second = Activator.CreateInstance(type, new object[] { 3, 6 });

        var result = ((object)first).Equals((object)second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_return_true_for_generic_class()
    {
        var genericClassType = testResult.Assembly.GetType("GenericClass`1");
        var propType = testResult.Assembly.GetType("GenericClassNormalClass");
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

    [Fact]
    public void Equals_should_should_ignored_marked_properties()
    {
        var type = testResult.Assembly.GetType("IgnoredPropertiesClass");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;
        first.Y = 2;

        dynamic second = Activator.CreateInstance(type);
        second.X = 1;
        second.Y = 3;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_should_inherited_ignored_marked_properties()
    {
        var type = testResult.Assembly.GetType("InheritedIgnoredPropertiesClass");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;
        first.Y = 2;

        dynamic second = Activator.CreateInstance(type);
        second.X = 1;
        second.Y = 3;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_return_false_for_different_value_for_changed_property_in_base_class()
    {
        var type = testResult.Assembly.GetType("InheritedClass");
        dynamic first = Activator.CreateInstance(type);
        first.A = 1;
        first.B = 2;

        dynamic second = Activator.CreateInstance(type);
        second.A = 3;
        second.B = 2;

        var result = first.Equals(second);

        Assert.False(result);
    }

    [Fact]
    public void Equals_should_return_true_for_class_with_indexer()
    {
        var type = testResult.Assembly.GetType("ClassWithIndexer");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;
        first.Y = 2;

        dynamic second = Activator.CreateInstance(type);
        second.X = 1;
        second.Y = 2;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_return_true_for_equal_collections()
    {
        var type = testResult.Assembly.GetType("IntCollection");
        dynamic first = Activator.CreateInstance(type);
        first.Collection = new[] { 1, 2, 3, 4, 5, 6 };
        first.Count = 2;

        dynamic second = Activator.CreateInstance(type);
        second.Collection = new List<int> { 1, 2, 3, 4, 5, 6 };
        second.Count = 2;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_return_true_for_equal_arrays()
    {
        var type = testResult.Assembly.GetType("IntArray");
        dynamic first = Activator.CreateInstance(type);
        first.Collection = new[] { 1, 2, 3};

        dynamic second = Activator.CreateInstance(type);
        second.Collection = new [] { 1, 2, 3 };

        var result = first.Equals(second);

        Assert.True(result);
    }
    
    [Fact]
    public void Equals_should_return_true_for_equal_string_arrays()
    {
        var type = testResult.Assembly.GetType("StringArray");
        dynamic first = Activator.CreateInstance(type);
        first.Collection = new[] { "one", "two", "three"};

        dynamic second = Activator.CreateInstance(type);
        second.Collection = new[] { "one", "two", "three"};

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_return_true_for_reference_equal_array()
    {
        var type = testResult.Assembly.GetType("IntCollection");
        dynamic first = Activator.CreateInstance(type);
        first.Collection = new[] { 1, 2, 3, 4, 5, 6 };
        first.Count = 2;

        dynamic second = Activator.CreateInstance(type);
        second.Collection = first.Collection;
        second.Count = 2;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_return_true_for_null_array()
    {
        var type = testResult.Assembly.GetType("IntCollection");
        dynamic first = Activator.CreateInstance(type);
        first.Collection = null;
        first.Count = 0;

        dynamic second = Activator.CreateInstance(type);
        second.Collection = null;
        second.Count = 0;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_return_false_for_null_array_and_fill_array()
    {
        var type = testResult.Assembly.GetType("IntCollection");
        dynamic first = Activator.CreateInstance(type);
        first.Collection = new[]{1};
        first.Count = 0;

        dynamic second = Activator.CreateInstance(type);
        second.Collection = null;
        second.Count = 0;

        var result = first.Equals(second);

        Assert.False(result);
    }

    [Fact]
    public void Equals_should_return_false_for_fill_array_and_null_array()
    {
        var type = testResult.Assembly.GetType("IntCollection");
        dynamic first = Activator.CreateInstance(type);
        first.Collection = null;
        first.Count = 0;

        dynamic second = Activator.CreateInstance(type);
        second.Collection = new[] { 1 };
        second.Count = 0;

        var result = first.Equals(second);

        Assert.False(result);
    }

    [Fact]
    public void Equals_should_return_true_for_nested_class()
    {
        var nestedInstanceFirst = GetNestedClassInstance();
        var nestedInstanceSecond = GetNestedClassInstance();

        var result = nestedInstanceFirst.Equals(nestedInstanceSecond);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_return_false_for_changed_nested_class()
    {
        var nestedInstanceFirst = GetNestedClassInstance();
        var nestedInstanceSecond = GetNestedClassInstance();
        nestedInstanceSecond.D.X = 11;

        var result = nestedInstanceFirst.Equals(nestedInstanceSecond);

        Assert.False(result);
    }

    [Fact]
    public void Equals_should_return_false_for_null_nested_class()
    {
        var nestedInstanceFirst = GetNestedClassInstance();
        var nestedInstanceSecond = GetNestedClassInstance();
        nestedInstanceSecond.D = null;

        var result = nestedInstanceFirst.Equals(nestedInstanceSecond);

        Assert.False(result);
    }

    dynamic GetNestedClassInstance()
    {
        var normalType = testResult.Assembly.GetType("NormalClass");
        dynamic normalInstance = Activator.CreateInstance(normalType);
        normalInstance.X = 1;
        normalInstance.Y = "2";
        normalInstance.Z = 4.5;
        normalInstance.V = 'V';
        var nestedType = testResult.Assembly.GetType("NestedClass");
        dynamic nestedInstance = Activator.CreateInstance(nestedType);
        nestedInstance.A = 10;
        nestedInstance.B = "11";
        nestedInstance.C = 12.25;
        nestedInstance.D = normalInstance;
        return nestedInstance;
    }

    [Fact]
    public void Equals_should_return_true_for_equal_structs()
    {
        var type = testResult.Assembly.GetType("SimpleStruct");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;
        first.Y = 2;
        dynamic second = Activator.CreateInstance(type);
        second.X = 1;
        second.Y = 2;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_return_false_for_changed_struct()
    {
        var type = testResult.Assembly.GetType("SimpleStruct");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;
        first.Y = 2;
        dynamic second = Activator.CreateInstance(type);
        second.X = 1;
        second.Y = 3;

        var result = first.Equals(second);

        Assert.False(result);
    }

    [Fact]
    public void Equals_should_return_true_for_equal_struct_property()
    {
        var type = testResult.Assembly.GetType("StructPropertyClass");
        var propertyType = testResult.Assembly.GetType("SimpleStruct");
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

    [Fact]
    public void Equals_should_return_true_for_equal_normal_class()
    {
        var type = testResult.Assembly.GetType("NormalClass");
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
        Assert.True(result1);
    }

    [Fact]
    public void Equals_should_return_true_for_class_with_static_properties()
    {
        var type = testResult.Assembly.GetType("ClassWithStaticProperties");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;
        first.Y = "2";

        dynamic second = Activator.CreateInstance(type);
        second.X = 1;
        second.Y = "2";

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_return_true_for_class_with_method_to_remove()
    {
        var type = testResult.Assembly.GetType("ClassWithMethodToRemove");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;
        first.Y = 2;

        dynamic second = Activator.CreateInstance(type);
        second.X = 1;
        second.Y = 2;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_return_true_for_class_with_guid_in_parent()
    {
        var guid = "{f6ab1abe-5811-40e9-8154-35776d2e5106}";

        var type = testResult.Assembly.GetType( "ReferenceObject" );
        dynamic first = Activator.CreateInstance( type );
        first.Name = "Test";
        first.Id = Guid.Parse( guid );

        dynamic second = Activator.CreateInstance( type );
        second.Name = "Test";
        second.Id = Guid.Parse( guid );

        var result = first.Equals( second );

        Assert.True( result );
    }

    [Fact]
    public void Equals_should_return_for_class_with_generic_property()
    {
        var type = testResult.Assembly.GetType("ClassWithGenericProperty");
        dynamic first = Activator.CreateInstance(type);
        first.Prop = new GenericDependency<int> { Prop = 1 };

        dynamic second = Activator.CreateInstance(type);
        second.Prop = new GenericDependency<int> { Prop = 1 };

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_ignore_properties_in_base_class_when_class_is_marked()
    {
        var type = testResult.Assembly.GetType("IgnoreBaseClass");

        dynamic instance = Activator.CreateInstance(type);
        instance.A = 1;
        instance.B = 2;

        dynamic instance2 = Activator.CreateInstance(type);
        instance2.A = 3;
        instance2.B = 2;

        var result = instance.Equals(instance2);

        Assert.True(result);
    }

    #endregion

    #region Custom

    [Fact]
    public void Equals_should_use_custom_logic()
    {
        var type = testResult.Assembly.GetType("CustomEquals");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;

        dynamic second = Activator.CreateInstance(type);
        second.X = 2;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_use_custom_logic_for_structure()
    {
        var type = testResult.Assembly.GetType("CustomStructEquals");
        dynamic first = Activator.CreateInstance(type);
        first.X = 1;

        dynamic second = Activator.CreateInstance(type);
        second.X = 2;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_use_custom_logic_for_generic_type()
    {
        var genericClassType = testResult.Assembly.GetType("CustomGenericEquals`1");
        var propType = typeof(int);
        var type = genericClassType.MakeGenericType(propType);

        dynamic first = Activator.CreateInstance(type);
        first.Prop = 1;
        dynamic second = Activator.CreateInstance(type);
        second.Prop = 1;
        dynamic third = Activator.CreateInstance(type);
        third.Prop = 2;

        Assert.True(first.Equals(second));
        Assert.False(first.Equals(third));
    }

    [Fact]
    public void GetHashCode_should_use_custom_logic()
    {
        var type = testResult.Assembly.GetType("CustomGetHashCode");
        dynamic instance = Activator.CreateInstance(type);
        instance.X = 1;

        var result = instance.GetHashCode();

        Assert.Equal(423, result);
    }

    [Fact]
    public void GetHashCode_should_use_custom_logic_for_structure()
    {
        var type = testResult.Assembly.GetType("CustomStructEquals");
        dynamic instance = Activator.CreateInstance(type);
        instance.X = 1;

        var result = instance.GetHashCode();

        Assert.Equal(42, result);
    }

    [Fact]
    public void GetHashCode_should_use_custom_logic_for_generic_type()
    {
        var genericClassType = testResult.Assembly.GetType("CustomGenericEquals`1");
        var propType = typeof(int);
        var type = genericClassType.MakeGenericType(propType);

        dynamic instance = Activator.CreateInstance(type);
        instance.Prop = 1;

        var result = instance.GetHashCode();

        Assert.Equal(42, result);
    }
    #endregion

    #region ParentInOtherAssembly

    [Fact]
    public void Equals_should_return_true_for_child_with_parent_in_other_assembly()
    {
        var child = testResult.Assembly.GetType("Child");
        dynamic first = Activator.CreateInstance(child);
        first.InParent = 10;
        first.InChild = 5;

        dynamic second = Activator.CreateInstance(child);
        second.InParent = 10;
        second.InChild = 5;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void GetHashCode_should_return_true_for_child_with_parent_in_other_assembly()
    {
        var child = testResult.Assembly.GetType("Child");
        dynamic first = Activator.CreateInstance(child);
        first.InParent = 10;
        first.InChild = 5;

        var result = first.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void Equality_operator_should_return_true_for_child_with_parent_in_other_assembly()
    {
        var child = testResult.Assembly.GetType("Child");
        dynamic first = Activator.CreateInstance(child);
        first.InParent = 10;
        first.InChild = 5;

        dynamic second = Activator.CreateInstance(child);
        second.InParent = 10;
        second.InChild = 5;

        var result = first.GetHashCode();

        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equals_should_return_true_for_child_with_complex_parent_in_other_assembly()
    {
        var child = testResult.Assembly.GetType("ComplexChild");
        dynamic first = Activator.CreateInstance(child);
        first.InChildNumber  = 1;
        first.InChildText = "test";
        first.InChildCollection = new[] { 1, 2 };
        first.InParentNumber = 1;
        first.InParentText = "test";
        first.InParentCollection = new[] { 1, 2 };

        dynamic second = Activator.CreateInstance(child);
        second.InChildNumber = 1;
        second.InChildText = "test";
        second.InChildCollection = new[] { 1, 2 };
        second.InParentNumber = 1;
        second.InParentText = "test";
        second.InParentCollection = new[] { 1, 2 };

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void GetHashCode_should_return_true_for_child_with_complex_parent_in_other_assembly()
    {
        var child = testResult.Assembly.GetType("ComplexChild");
        dynamic first = Activator.CreateInstance(child);
        first.InChildNumber = 1;
        first.InChildText = "test";
        first.InChildCollection = new[] { 1, 2 };
        first.InParentNumber = 1;
        first.InParentText = "test";
        first.InParentCollection = new[] { 1, 2 };

        var result = first.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void Equality_operator_should_return_true_for_child_with_complex_parent_in_other_assembly()
    {
        var child = testResult.Assembly.GetType("ComplexChild");
        dynamic first = Activator.CreateInstance(child);
        first.InChildNumber = 1;
        first.InChildText = "test";
        first.InChildCollection = new[] { 1, 2 };
        first.InParentNumber = 1;
        first.InParentText = "test";
        first.InParentCollection = new[] { 1, 2 };

        dynamic second = Activator.CreateInstance(child);
        second.InChildNumber = 1;
        second.InChildText = "test";
        second.InChildCollection = new[] { 1, 2 };
        second.InParentNumber = 1;
        second.InParentText = "test";
        second.InParentCollection = new[] { 1, 2 };

        var result = first.GetHashCode();

        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void Equals_should_return_true_for_generic_child_with_parent_in_other_assembly()
    {
        var child = testResult.Assembly.GetType("GenericChild");
        dynamic first = Activator.CreateInstance(child);
        first.InChild = "1";
        first.GenericInParent = 2;

        dynamic second = Activator.CreateInstance(child);
        second.InChild = "1";
        second.GenericInParent = 2;

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void GetHashCode_should_return_true_for_generic_child_with_parent_in_other_assembly()
    {
        var child = testResult.Assembly.GetType("GenericChild");
        dynamic first = Activator.CreateInstance(child);
        first.InChild = "1";
        first.GenericInParent = 2;

        var result = first.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void Equality_operator_should_return_true_for_generic_child_with_parent_in_other_assembly()
    {
        var child = testResult.Assembly.GetType("GenericChild");
        dynamic first = Activator.CreateInstance(child);
        first.InChild = "1";
        first.GenericInParent = 2;

        dynamic second = Activator.CreateInstance(child);
        second.InChild = "1";
        second.GenericInParent = 2;

        var result = first.GetHashCode();

        Assert.True(first == second);
        Assert.False(first != second);
    }

    [Fact]
    public void GetHashCode_should_return_value_class_with_generic_base()
    {
        var type = testResult.Assembly.GetType("ClassWithGenericBase");
        dynamic instance = Activator.CreateInstance(type);
        instance.Prop = 1;

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void Equals_should_return_value_class_with_generic_base()
    {
        var child = testResult.Assembly.GetType("ClassWithGenericBase");
        dynamic first = Activator.CreateInstance(child);
        first.Prop = 1;

        dynamic second = Activator.CreateInstance(child);
        second.Prop = 1;
        var result = first.Equals(second);

        Assert.True(result);
    }

    #endregion
}