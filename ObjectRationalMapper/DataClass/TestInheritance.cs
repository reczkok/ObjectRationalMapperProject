using ObjectRationalMapper.Attributes;

namespace ObjectRationalMapper.DataClass;

[Tablename("testInheritance", ParentClass = typeof(TestClass), ChildClasses = new[] { typeof(TestInheritance2) })]
public class TestInheritance : TestClass
{
    [Field("inhId", typeof(int))]
    public int TestInheritanceId { get; set; }
    [Field("inhName", typeof(string))]
    public string TestInheritanceName { get; set; }
}