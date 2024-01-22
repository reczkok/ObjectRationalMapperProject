
namespace ObjectRationalMapper.DatabaseConnection;
using MySql.Data.MySqlClient;

public interface IConnectionProvider
{
    public MySqlConnection GetConnection();
    public void ConfigureConnection(string host, string database, string user, string password);
    public void CloseConnection();
}