using System.Reflection;
using ObjectRationalMapper.Attributes;
using ObjectRationalMapper.DatabaseConnection;

namespace ObjectRationalMapper.DatabaseActions;

public static class CommandExecutor
{
    /*
     * This is a CommandExecutor class, which is a static class.
     * It is used to execute created commands on the database.
     */
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
    
    public static void ExecuteInsert(string query = "")
    {
        var session = Session.GetInstance();
        var connection = session.GetConnection();
        var command = connection?.CreateCommand();
        if (command == null) return;
        command.CommandText = query;
        command.ExecuteNonQuery();
    }

    public static void ExecuteDelete(string query = "")
    {
        var session = Session.GetInstance();
        var connection = session.GetConnection();
        var command = connection?.CreateCommand();
        if (command == null) return;
        command.CommandText = query;
        command.ExecuteNonQuery();
    }
    
    public static void ExecuteUpdate(string query = "")
    {
        var session = Session.GetInstance();
        var connection = session.GetConnection();
        var command = connection?.CreateCommand();
        if (command == null) return;
        command.CommandText = query;
        command.ExecuteNonQuery();
    }
    
    public static bool ExecuteTableExists(string query = "")
    {
        var session = Session.GetInstance();
        var connection = session.GetConnection();
        var command = connection?.CreateCommand();
        if (command == null) return false;
        command.CommandText = query;
        var reader = command.ExecuteReader();
        var result = reader.HasRows;
        reader.Close();
        return result;
    }
    
    public static void ExecuteCreateTable(string query = "")
    {
        var session = Session.GetInstance();
        var connection = session.GetConnection();
        var command = connection?.CreateCommand();
        if (command == null) return;
        command.CommandText = query;
        command.ExecuteNonQuery();
    }

    public static void ExecuteDropTable<T>() 
    {
        var session = Session.GetInstance();
        var connection = session.GetConnection();
        var command = connection?.CreateCommand();
        if (command == null) return;
        var tableName = typeof(T).GetCustomAttribute<TablenameAttribute>()!.Name;
        command.CommandText = $"DROP TABLE IF EXISTS {tableName};";
        command.ExecuteNonQuery();    
    }
}