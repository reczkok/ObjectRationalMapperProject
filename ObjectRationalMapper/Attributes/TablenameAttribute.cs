namespace ObjectRationalMapper.Attributes;

public class TablenameAttribute : Attribute
{
    public string? Inheritance { get; set; }
    public string Name { get; set; }
    public TablenameAttribute(string name, string? inheritance = null)
    {
        Name = name;
        Inheritance = inheritance;
    }
}