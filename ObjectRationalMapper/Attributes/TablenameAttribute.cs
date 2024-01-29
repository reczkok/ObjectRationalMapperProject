namespace ObjectRationalMapper.Attributes;

public class TablenameAttribute : Attribute
{
    public Type? ParentClass { get; set; }
    public Type[]? ChildClasses { get; set; }
    public string Name { get; set; }

    public TablenameAttribute(string name, Type? parentClass = null, params Type[]? childClasses)
    {
        Name = name;
        ParentClass = parentClass;
        if (childClasses == null) return;
        ChildClasses = new Type[childClasses.Length];
        for (var i = 0; i < childClasses.Length; i++)
        {
            ChildClasses[i] = childClasses[i];
        }
    }
}