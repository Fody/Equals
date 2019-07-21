using System;
using Xunit;

public partial class IntegrationTests
{
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
        instance.Collection = new[] { 1, 2, 3 };

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_string_array()
    {
        var type = testResult.Assembly.GetType("StringArray");
        dynamic instance = Activator.CreateInstance(type);
        instance.Collection = new[] { "one", "two", "three" };

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
        instance.Collection = new[] { 1, 2, 3, 4, 5 };

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

        dynamic array = Activator.CreateInstance(propType.MakeArrayType(), 1);
        array[0] = propInstance;

        instance.B = array;

        var result = instance.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_enums()
    {
        var type = testResult.Assembly.GetType("EnumClass");
        dynamic instance = Activator.CreateInstance(type, 3, 6);

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
        dynamic array = Activator.CreateInstance(propType.MakeArrayType(), 1);
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
        dynamic array = Activator.CreateInstance(propType.MakeArrayType(), 1);
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
    public void GetHashCode_should_return_value_for_class_with_guid_in_parent()
    {
        var guid = "{f6ab1abe-5811-40e9-8154-35776d2e5106}";

        var type = testResult.Assembly.GetType("ReferenceObject");
        dynamic first = Activator.CreateInstance(type);
        first.Name = "Test";
        first.Id = Guid.Parse(guid);

        var result = first.GetHashCode();

        Assert.NotEqual(0, result);
    }

    [Fact]
    public void GetHashCode_should_return_value_for_class_with_generic_property2()
    {
        var type = testResult.Assembly.GetType("ClassWithGenericProperty");
        dynamic first = Activator.CreateInstance(type);
        first.Prop = new GenericDependency<int> { Prop = 1 };

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
}