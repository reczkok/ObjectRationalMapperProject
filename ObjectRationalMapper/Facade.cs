namespace ObjectRationalMapper;

public class Facade
{
    public void ConfigureMySql(string host, string database, string user, string password)
    {
        var session = DatabaseConnection.Session.GetInstance();
        session.Configure(host, database, user, password);
    }
    
    public string ExecuteSelect(string query = "")
    {
        return DatabaseActions.CommandExecutor.ExecuteSelect(query);
    }
    
    public void ExecuteInsert(string query = "")
    {
        DatabaseActions.CommandExecutor.ExecuteInsert(query);
    }
}