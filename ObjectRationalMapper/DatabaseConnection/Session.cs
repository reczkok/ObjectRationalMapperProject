using MySql.Data.MySqlClient;

namespace ObjectRationalMapper.DatabaseConnection;

public sealed class Session
{
    private static Session instance;
    private IConnectionProvider? _connectionProvider;
    
    private Session() {}
    
    public static Session GetInstance()
    {
        return instance ??= new Session();
    }
    
    public MySqlConnection? GetConnection()
    {
        if (_connectionProvider == null)
        {
            throw new InvalidOperationException("Connection must be configured before use");
        }
        return _connectionProvider?.GetConnection();
    }
    
    public void Configure(string host, string database, string user, string password)
    {
        _connectionProvider?.CloseConnection();
        _connectionProvider = ConnectionProviderFactory.CreateProvider(host, database, user, password);
    }
}