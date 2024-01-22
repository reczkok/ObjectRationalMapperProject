using ObjectRationalMapper.Attributes;

namespace ObjectRationalMapper.DataClass;

[Tablename("test5")]
public class TestClass2
{
    [Field("id", typeof(int))]
    public int Id { get; set; }
    [Field("name", typeof(string))]
    public string Name { get; set; }
    [Field("age", typeof(int))]
    public int Age { get; set; }
    [Field("height", typeof(double))]
    public double Height { get; set; }
    [Field("weight", typeof(double))]
    public double Weight { get; set; }
}