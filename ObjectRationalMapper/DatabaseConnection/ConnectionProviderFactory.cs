namespace ObjectRationalMapper.DatabaseConnection;

public static class ConnectionProviderFactory
{
    public static ConnectionProvider CreateProvider(string host, string database, string user, string password)
    {
        var provider = new ConnectionProvider();
        provider.ConfigureConnection(host, database, user, password);
        return provider;
    }
}