using MongoDB.Driver;

public class MongoClientFactory
{
    private readonly Dictionary<string, IMongoClient> _clients;

    public MongoClientFactory(Dictionary<string, IMongoClient> clients)
    {
        _clients = clients;
    }

    public IMongoClient GetClient(string key)
    {
        return _clients[key];
    }
}
