using ObjectRationalMapper.Attributes;

namespace ObjectRationalMapper.DataClass;

[Tablename("test", ChildClasses = new[] { typeof(TestInheritance) })]
[Id("id")]
public class TestClass
{
    [Field("name", typeof(string))]
    public string Name { get; set; }
    [Field("age", typeof(int))]
    public int Age { get; set; }
    [Field("height", typeof(double))]
    public double Height { get; set; }
    [Field("weight", typeof(double))]
    public double Weight { get; set; }
}