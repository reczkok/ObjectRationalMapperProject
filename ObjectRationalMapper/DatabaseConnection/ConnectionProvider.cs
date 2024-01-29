using MySql.Data.MySqlClient;

namespace ObjectRationalMapper.DatabaseConnection;

public class ConnectionProvider : IConnectionProvider
{
    private MySqlConnection _connection;

    public MySqlConnection GetConnection()
    {
        return _connection;
    }

    public void ConfigureConnection(string host, string database, string user, string password)
    {
        var connectionString = $"server={host};database={database};uid={user};pwd={password}";
        _connection?.Close();
        _connection = new MySqlConnection(connectionString);
        _connection.Open();
        if (_connection.State != System.Data.ConnectionState.Open)
        {
            throw new InvalidOperationException("Connection could not be opened");
        }
    }

    public void CloseConnection()
    {
        _connection?.Close();
    }
}