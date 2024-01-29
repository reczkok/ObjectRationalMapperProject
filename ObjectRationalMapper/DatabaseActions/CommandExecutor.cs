using ObjectRationalMapper.DatabaseConnection;

namespace ObjectRationalMapper.DatabaseActions;

public static class CommandExecutor
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
}