namespace ObjectRationalMapper.Attributes;

public class IdAttribute : Attribute
{
    public string Name { get; set; }
    public IdAttribute(string name)
    {
        Name = name;
    }
}