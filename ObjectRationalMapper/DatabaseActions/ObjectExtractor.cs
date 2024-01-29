using System.Reflection;
using ObjectRationalMapper.Attributes;
using ObjectRationalMapper.DatabaseConnection;

namespace ObjectRationalMapper.DatabaseActions;

public class ObjectExtractor <T>
{
    public static string ExecuteSelect(string query = "")
    {
        var session = Session.GetInstance();
        var connection = session.GetConnection();
        var command = connection?.CreateCommand();
        if (command == null) return "";
        command.CommandText = query;
        var reader = command.ExecuteReader();
        var result = "";
        for (var i = 0; i < reader.FieldCount; i++)
        {
            result += reader.GetName(i) + " | ";
        }
        result += "\n";
        while (reader.Read())
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                result += reader[i] + " | ";
            }
            result += "\n";
        }
        reader.Close();
        return result;
    }
    
    public static T[] ExtractObjects(string query = "")
    {
        var session = Session.GetInstance();
        var connection = session.GetConnection();
        var command = connection?.CreateCommand();
        if (command == null) return new T[0];
        command.CommandText = query;
        var reader = command.ExecuteReader();
        var result = new List<T>();
        while (reader.Read())
        {
            var obj = Activator.CreateInstance<T>();
            var type = typeof(T);
            var properties = type.GetProperties();
            var fields = properties.Select(property => property.GetCustomAttribute<FieldAttribute>()).ToArray();
            
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var value = reader[i];
                var field = fields.FirstOrDefault(field => field?.Name == reader.GetName(i));
                if (field == null) continue;
                var property = properties.FirstOrDefault(property => property.GetCustomAttribute<FieldAttribute>()?.Name == field.Name);
                if (property == null) continue;
                if (value is decimal)
                {
                    value = Convert.ToDouble(value);
                }
                property.SetValue(obj, value);
            }
            result.Add(obj);
        }
        reader.Close();
        return result.ToArray();
    }
}