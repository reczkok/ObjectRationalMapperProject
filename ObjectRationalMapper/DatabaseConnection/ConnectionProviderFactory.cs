namespace ObjectRationalMapper.DatabaseConnection;

public static class ConnectionProviderFactory
{
    /*
     * The ConnectionProviderFactory class is a factory class responsible for creating instances of ConnectionProvider.
     * It encapsulates the logic for creating and configuring a ConnectionProvider object, which is used to manage 
     * database connections. This approach centralizes the creation logic and allows for easier maintenance and potential 
     * extensions of how ConnectionProvider objects are instantiated and configured.
     */
    public static ConnectionProvider CreateProvider(string host, string database, string user, string password)
    {
        var provider = new ConnectionProvider();
        provider.ConfigureConnection(host, database, user, password);
        return provider;
    }
}