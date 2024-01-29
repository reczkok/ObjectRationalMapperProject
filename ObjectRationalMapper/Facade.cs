using System.Linq.Expressions;
using ObjectRationalMapper.DatabaseQuery;

namespace ObjectRationalMapper;

public class Facade
{
    // Configures the MySQL database connection using the provided credentials.
    public void ConfigureMySql(string host, string database, string user, string password)
    {
        // Retrieves a singleton instance of the database session.
        var session = DatabaseConnection.Session.GetInstance();
        // Configures the session with the provided connection details.
        session.Configure(host, database, user, password);
    }
    
    // Executes a SELECT SQL query and returns the result as a string.
    public string ExecuteSelect(string query = "")
    {
        // Executes the SELECT query using the CommandExecutor in DatabaseActions.
        return DatabaseActions.CommandExecutor.ExecuteSelect(query);
    }
    
    // Executes an INSERT SQL query.
    public void ExecuteInsert(string query = "")
    {
        // Executes the INSERT query using the CommandExecutor in DatabaseActions.
        DatabaseActions.CommandExecutor.ExecuteInsert(query);
    }
    
    // Executes a DELETE SQL query.
    public void ExecuteDelete(string query = "")
    {
        // Executes the DELETE query using the CommandExecutor in DatabaseActions.
        DatabaseActions.CommandExecutor.ExecuteDelete(query);
    }
    
    // Executes an UPDATE SQL query.
    public void ExecuteUpdate(string query = "")
    {
        // Executes the UPDATE query using the CommandExecutor in DatabaseActions.
        DatabaseActions.CommandExecutor.ExecuteUpdate(query);
    }
    
    // Extracts and returns an array of objects of type T from the result of the provided SQL query.
    public T[] ExtractObjects<T>(string query = "")
    {
        // Extracts objects of type T from the SQL query using the ObjectExtractor in DatabaseActions.
        return DatabaseActions.ObjectExtractor<T>.ExtractObjects(query);
    }
    
    // Returns an instance of a query builder for constructing SELECT statements for type T.
    public IQueryBuilder<T> GetSelectBuilder<T>()
    {
        // Creates and returns a new instance of QueryBuilder for type T.
        return new DatabaseQuery.QueryBuilder<T>();
    }
    
    // Returns an instance of a query builder for constructing INSERT statements for type T.
    public IInsertBuilder<T> GetInsertBuilder<T>()
    {
        // Creates and returns a new instance of InsertBuilder for type T.
        return new DatabaseQuery.InsertBuilder<T>();
    }
    
    // Returns an instance of a query builder for constructing DELETE statements for type T.
    public IDeleteBuilder<T> GetDeleteBuilder<T>()
    {
        // Creates and returns a new instance of DeleteBuilder for type T.
        return new DatabaseQuery.DeleteBuilder<T>();
    }
    
    // Returns an instance of a query builder for constructing UPDATE statements for type T.
    public IUpdateBuilder<T> GetUpdateBuilder<T>()
    {
        // Creates and returns a new instance of UpdateBuilder for type T.
        return new DatabaseQuery.UpdateBuilder<T>();
    }
}
