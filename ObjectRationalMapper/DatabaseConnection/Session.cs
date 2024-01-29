using MySql.Data.MySqlClient;

namespace ObjectRationalMapper.DatabaseConnection;

public sealed class Session
{
    /*
     * The Session class implements the Singleton pattern to provide a single, globally accessible instance 
     * throughout the application. It is responsible for managing the database session, including 
     * obtaining and configuring the database connection through an IConnectionProvider.
     */
    
    // Holds the single instance of Session.
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
    // Configures the database connection using the provided parameters.
    // Parameters:
    //   host: The hostname or IP address of the MySQL server.
    //   database: The name of the database to connect to.
    //   user: The username for the MySQL account.
    //   password: The password for the MySQL account.
    // This method creates and configures a new connection provider using the ConnectionProviderFactory.
    public void Configure(string host, string database, string user, string password)
    {
        _connectionProvider?.CloseConnection();
        _connectionProvider = ConnectionProviderFactory.CreateProvider(host, database, user, password);
    }
}