using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ObjectRationalMapper.DatabaseActions;
using Attributes;

public static class TableGenerator<T>
{
    // Retrieves the base class for the generic type T.
    private static Type _getBaseClass()
    {
        var type = typeof(T);
        // Assumes a custom class mapper is used to identify the base class of T.
        var baseType = CustomClassMapper<T>.GetParentClass(type);
        return baseType;
    }

    // Gets the ID attribute of a given class member, defaulting to "id" if not explicitly defined.
    private static string _getBaseClassId(MemberInfo t)
    {
        var id = t.GetCustomAttribute<IdAttribute>(); // Retrieves the IdAttribute of the member.
        return id?.Name ?? "id"; // Returns the name specified in the IdAttribute or "id" if not specified.
    }

    private static string _getFieldsAsString(FieldAttribute[] fields) {
        var fieldsAsString = string.Join(", ", fields.Select(field => $"{field?.Name} {MapToSqlType(field?.Type ?? typeof(string))}"));
        return fieldsAsString;
    }
    
    // Constructs a string representation of fields for a SQL CREATE TABLE statement.
    private static string _getFieldsAsString(Type t)
    {
        var fields = CustomClassMapper<T>.GetFieldAttributes(t); // Retrieves the fields defined in the class.
        // Joins the fields into a single string, each field is formatted as 'field_name field_type'.
        var fieldsAsString = string.Join(", ", fields.Select(field => $"{field?.Name} {MapToSqlType(field?.Type ?? typeof(string))}"));
        return fieldsAsString;
    }
    
    // Retrieves an array of field attributes for a given type T.
    private static FieldAttribute[]? _getFields(Type t)
    {
        // Utilizes a custom class mapper to extract field attributes from the type.
        var fields = CustomClassMapper<T>.GetFieldAttributes(t);
        return fields;
    }

    // Gets child tables' types based on the TablenameAttribute of a class member.
    private static Type[]? _getChildrenTables(MemberInfo t)
    {
        var children = t.GetCustomAttribute<TablenameAttribute>(); // Retrieves the TablenameAttribute.
        var childrenTables = children?.ChildClasses; // Gets the child classes defined in the attribute.
        return childrenTables;
    }

    // Retrieves all child class types in a recursive manner.
    private static Type[]? _getAllChildrenTables(MemberInfo t)
    {
        var children = _getChildrenTables(t);
        var allChildren = new List<Type>(); // List to hold all discovered child types.
        var queue = new Queue<Type>(); // Queue for breadth-first traversal of child types.

        if (children != null)
        {
            foreach (var child in children)
            {
                queue.Enqueue(child); // Enqueue initial set of children.
            }
        }

        while (queue.Count > 0)
        {
            var child = queue.Dequeue(); // Dequeue a child type.
            allChildren.Add(child); // Add it to the list of all child types.
            var childChildren = _getChildrenTables(child); // Get children of the dequeued child.

            if (childChildren != null)
            {
                foreach (var childChild in childChildren)
                {
                    queue.Enqueue(childChild); // Enqueue the children of the child.
                }
            }
        }

        return allChildren.ToArray(); // Return all discovered child types.
    }

    // Generates a SQL CREATE TABLE query based on the type T and its hierarchy.
    public static string GenerateTableQuery()
    {
        var baseType = _getBaseClass(); // Gets the base class of T.
        var baseTypeName = baseType.GetCustomAttribute<TablenameAttribute>()?.Name ?? baseType.Name; // Gets the table name.
        var baseClassId = _getBaseClassId(baseType); // Gets the ID field name.
        var discriminator = "ORMPROJ_Discriminator VARCHAR(255) NOT NULL"; // Adds a discriminator field for ORM.
        var allChildren = _getAllChildrenTables(baseType); // Gets all child types of the base class.

        var query = new StringBuilder(); // StringBuilder to construct the query.
        // Start of the CREATE TABLE query.
        query.Append($"CREATE TABLE IF NOT EXISTS {baseTypeName} ({baseClassId} INT NOT NULL AUTO_INCREMENT, {discriminator}, ");
        query.Append(_getFieldsAsString(baseType));
        
        var addedFields = new List<string>();
        if (allChildren != null)
        {
            foreach (var child in allChildren)
            {
                var childFields = CustomClassMapper<T>.GetFieldAttributes(child);
                foreach (var field in childFields)
                {
                    if (addedFields.Contains(field!.Name)) {
                        throw new Exception($"Field {field.Name} already exists in base class. Please rename it.");
                    }
                    
                    query.Append($", {field!.Name} {MapToSqlType(field!.Type ?? typeof(string))}");
                    addedFields.Add(field!.Name);
                }
            }
        }

        // Completes the CREATE TABLE query.
        query.Append($", PRIMARY KEY ({baseClassId})");
        query.Append(");");

        return query.ToString(); // Returns the final query string.
    }
    
    // Maps C# types to their corresponding SQL types.
    private static string MapToSqlType(Type propertyType)
    {
        return propertyType switch
        {
            Type t when t == typeof(int) => "INT",
            Type t when t == typeof(string) => "VARCHAR(255)",
            Type t when t == typeof(double) => "DECIMAL(10, 2)",
            Type t when t == typeof(DateTime) => "DATETIME",
            Type t when t == typeof(bool) => "BOOLEAN",
            Type t when t == typeof(byte) => "TINYINT",
            Type t when t == typeof(char) => "CHAR",
            Type t when t.IsEnum => "VARCHAR(255)",
            _ => throw new NotSupportedException($"Type {propertyType.Name} is not supported"),
        };
    }
    
    // Checks if a table for type T exists in the database, and if not, generates one.
    public static void GenerateIfNotExists()
    {
        var tableName = CustomClassMapper<T>.GetHierarchyTableName(); // Gets the hierarchy table name for T.
        var query = $"SHOW TABLES LIKE '{tableName}'"; // Query to check if the table exists.
        var res = CommandExecutor.ExecuteTableExists(query); // Executes the query to check existence.
        
        if (!res)
        {
            var createTableQuery = GenerateTableQuery(); // Generates the CREATE TABLE query if the table doesn't exist.
            CommandExecutor.ExecuteCreateTable(createTableQuery); // Executes the CREATE TABLE query.
        }
    }
}
