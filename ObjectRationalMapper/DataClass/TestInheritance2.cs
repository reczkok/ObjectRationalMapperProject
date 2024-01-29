using ObjectRationalMapper.Attributes;

namespace ObjectRationalMapper.DataClass;

[Tablename("InheritanceTest", ParentClass = typeof(TestInheritance))]
public class TestInheritance2 : TestInheritance
{
    [Field("inhId2", typeof(int))]
    public int TestInheritanceId2 { get; set; }
    [Field("inhName2", typeof(string))]
    public string TestInheritanceName2 { get; set; }
}