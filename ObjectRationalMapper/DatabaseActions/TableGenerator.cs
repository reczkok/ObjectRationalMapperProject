using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ObjectRationalMapper.DatabaseActions;
using Attributes;

public static class TableGenerator<T>
{
    private static Type _getBaseClass()
    {
        var type = typeof(T);
        var baseType = CustomClassMapper<T>.GetParentClass(type);
        return baseType;
    }

    private static string _getBaseClassId(MemberInfo t)
    {
        var id = t.GetCustomAttribute<IdAttribute>();
        return id?.Name ?? "id";
    }

    private static string _getFieldsAsString(FieldAttribute[] fields) {
        var fieldsAsString = string.Join(", ", fields.Select(field => $"{field?.Name} {MapToSqlType(field?.Type ?? typeof(string))}"));
        return fieldsAsString;
    }

    private static string _getFieldsAsString(Type t)
    {
        var fields = _getFields(t);
        var fieldsAsString = _getFieldsAsString(fields!);
        return fieldsAsString;
    }
    
    private static FieldAttribute[]? _getFields(Type t)
    {
        var fields = CustomClassMapper<T>.GetFieldAttributes(t);
        return fields;
    }

    private static Type[]? _getChildrenTables(MemberInfo t)
    {
        var children = t.GetCustomAttribute<TablenameAttribute>();
        var childrenTables = children?.ChildClasses;
        return childrenTables;
    }

    private static Type[]? _getAllChildrenTables(MemberInfo t)
    {
        var children = _getChildrenTables(t);
        var allChildren = new List<Type>();
        var queue = new Queue<Type>();
        if (children != null)
        {
            foreach (var child in children)
            {
                queue.Enqueue(child);
            }
        }
        while (queue.Count > 0)
        {
            var child = queue.Dequeue();
            allChildren.Add(child);
            var childChildren = _getChildrenTables(child);
            if (childChildren != null)
            {
                foreach (var childChild in childChildren)
                {
                    queue.Enqueue(childChild);
                }
            }
        }
        return allChildren.ToArray();
    }

    public static string GenerateTableQuery()
    {
        var baseType = _getBaseClass();
        var baseTypeName = baseType.GetCustomAttribute<TablenameAttribute>()?.Name ?? baseType.Name;
        var baseClassId = _getBaseClassId(baseType);
        var discriminator = "ORMPROJ_Discriminator VARCHAR(255) NOT NULL";
        var allChildren = _getAllChildrenTables(baseType);
        
        // we will use a string builder to build the query
        var query = new StringBuilder();
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
        query.Append($", PRIMARY KEY ({baseClassId})");
        query.Append(");");
        return query.ToString();
    }
    
    private static string MapToSqlType(Type propertyType)
    {
        return propertyType switch
        {
            Type t when t == typeof(int) => "INT",
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
    
    public static void GenerateIfNotExists()
    {
        var tableName = CustomClassMapper<T>.GetHierarchyTableName();
        var query = $"SHOW TABLES LIKE '{tableName}'";
        var res = CommandExecutor.ExecuteTableExists(query);
        if (!res)
        {
            var createTableQuery = GenerateTableQuery();
            CommandExecutor.ExecuteCreateTable(createTableQuery);
        }
    }
}