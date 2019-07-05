using System;
using System.Collections.Generic;
using Xunit;

public partial class IntegrationTests
{
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
    public void Equals_should_return_true_for_StructWithArray()
    {
        var type = testResult.Assembly.GetType("StructWithArray");
        dynamic first = Activator.CreateInstance(type);
        first.X = new[] { 1, 2 };
        first.Y = new[] { 3, 4 };
        dynamic second = Activator.CreateInstance(type);
        second.X = new[] { 1, 2 };
        second.Y = new[] { 3, 4 };

        Assert.True(first.Equals(second));
    }
    [Fact]
    public void Equals_should_return_false_for_StructWithArray()
    {
        var type = testResult.Assembly.GetType("StructWithArray");
        dynamic first = Activator.CreateInstance(type);
        first.X = new[] { 1, 2 };
        first.Y = new[] { 3, 4 };
        dynamic second = Activator.CreateInstance(type);
        second.X = new[] { 1, 2 };
        second.Y = new[] { 1, 4 };

        Assert.False(first.Equals(second));
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
        dynamic array = Activator.CreateInstance(propType.MakeArrayType(), 1);
        array[0] = propInstance;
        instance.B = array;

        dynamic instance2 = Activator.CreateInstance(withoutGenericParameterType);
        instance2.Z = 12;
        instance2.A = 1;
        dynamic array2 = Activator.CreateInstance(propType.MakeArrayType(), 1);
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
        dynamic array = Activator.CreateInstance(propType.MakeArrayType(), 1);
        array[0] = propInstance;
        instance.B = array;

        dynamic instance2 = Activator.CreateInstance(instanceType);
        instance2.X = 12;
        instance2.A = 1;
        dynamic propInstance2 = Activator.CreateInstance(propType);
        dynamic array2 = Activator.CreateInstance(propType.MakeArrayType(), 1);
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

        return leftInstance.Equals((object)rightInstance);
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
        dynamic first = Activator.CreateInstance(type, 3, 6);
        dynamic second = Activator.CreateInstance(type, 3, 6);

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

                dynamic array = Activator.CreateInstance(propType.MakeArrayType(), 1);
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
        first.Collection = new[] { 1, 2, 3 };

        dynamic second = Activator.CreateInstance(type);
        second.Collection = new[] { 1, 2, 3 };

        var result = first.Equals(second);

        Assert.True(result);
    }

    [Fact]
    public void Equals_should_return_true_for_equal_string_arrays()
    {
        var type = testResult.Assembly.GetType("StringArray");
        dynamic first = Activator.CreateInstance(type);
        first.Collection = new[] { "one", "two", "three" };

        dynamic second = Activator.CreateInstance(type);
        second.Collection = new[] { "one", "two", "three" };

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
        first.Collection = new[] { 1 };
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
    public void Equals_should_return_true_for_class_with_guid_in_parent()
    {
        var guid = "{f6ab1abe-5811-40e9-8154-35776d2e5106}";

        var type = testResult.Assembly.GetType("ReferenceObject");
        dynamic first = Activator.CreateInstance(type);
        first.Name = "Test";
        first.Id = Guid.Parse(guid);

        dynamic second = Activator.CreateInstance(type);
        second.Name = "Test";
        second.Id = Guid.Parse(guid);

        var result = first.Equals(second);

        Assert.True(result);
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
}