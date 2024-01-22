namespace ObjectRationalMapper.Attributes;

public class FieldAttribute : Attribute
{
    public string Name { get; set; }
    public Type? Type { get; set; }
    public FieldAttribute(string name, Type? type = null)
    {
        Name = name;
        Type = type;
    }
}