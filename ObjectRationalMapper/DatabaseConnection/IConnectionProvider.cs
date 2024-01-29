
namespace ObjectRationalMapper.DatabaseConnection;
using MySql.Data.MySqlClient;

public interface IConnectionProvider
{
    /*
     * The IConnectionProvider interface defines methods for managing a connection to a database. 
     * It is intended to be implemented by classes that provide the mechanisms to establish, configure,
     * and close connections to a database.
     */
    public MySqlConnection GetConnection();
    public void ConfigureConnection(string host, string database, string user, string password);
    public void CloseConnection();
}