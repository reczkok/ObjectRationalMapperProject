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
    
    public void ExecuteDelete(string query = "")
    {
        DatabaseActions.CommandExecutor.ExecuteDelete(query);
    }
    
    public void ExecuteUpdate(string query = "")
    {
        DatabaseActions.CommandExecutor.ExecuteUpdate(query);
    }
    
    public T[] ExtractObjects<T>(string query = "")
    {
        return DatabaseActions.ObjectExtractor<T>.ExtractObjects(query);
    }
}