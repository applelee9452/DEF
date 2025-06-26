using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DEF;

public class DbClientMongo
{
    public const string DOC_DEFAULT_ID_NAME = "_id";// 默认唯一标识

    public IMongoDatabase Database { get; private set; }

    public DbClientMongo(string database_name, string connection_string)
    {
        MongoUrl url = new(connection_string);
        MongoClientSettings settings = MongoClientSettings.FromUrl(url);
        settings.MinConnectionPoolSize = 100;
        settings.MaxConnectionPoolSize = 100;
        settings.MaxConnectionIdleTime = TimeSpan.FromDays(365);
        settings.MaxConnectionLifeTime = TimeSpan.FromDays(365);
        settings.MaxConnecting = 5;

        MongoClient client = new(settings);
        Database = client.GetDatabase(database_name);
    }

    public Task<TDocument> ReadAsync<TDocument>(
        Expression<Func<TDocument, bool>> filter,
        string collection_name) where TDocument : class
    {
        var collection = GetCollection<TDocument>(collection_name);

        // e => e.Id == key
        return collection.Find(filter, null).FirstOrDefaultAsync();
    }

    public Task<List<TDocument>> ReadListAsync<TDocument>(string collection_name) where TDocument : class
    {
        var collection = GetCollection<TDocument>(collection_name);

        // e => e.Id == key
        return collection.Find(e => true, null).ToListAsync();
    }

    public Task<List<TDocument>> ReadListAsync<TDocument>(
        Expression<Func<TDocument, bool>> filter,
        string collection_name, int pageIndex = 0, int pageItems = 0) where TDocument : class
    {
        var collection = GetCollection<TDocument>(collection_name);

        // e => e.Id == key
        return collection.Find(filter, null).ToListAsync();
    }

    public async Task<(List<TDocument> Items, long Count)> PageAsync<TDocument>(
        FilterDefinition<TDocument> filter,
        string collection_name, int pageIndex, int pageItems) where TDocument : class
    {
        var collection = GetCollection<TDocument>(collection_name);

        var count = await collection.CountDocumentsAsync(filter);
        // e => e.Id == key
        var items = await collection.Find(filter, null).Skip(pageIndex * pageItems).Limit(pageItems).ToListAsync();
        return (items, count);
    }

    public Task<List<TDocument>> ReadListAsync<TDocument>(
        Expression<Func<TDocument, bool>> filter,
        string collection_name,
        int limit) where TDocument : class
    {
        var collection = GetCollection<TDocument>(collection_name);

        // e => e.Id == key
        return collection.Find(filter, null).Limit(limit).ToListAsync();
    }

    public Task UpdateOneAsync<TDocument>(FilterDefinition<TDocument> filter,
        string collection_name, UpdateDefinition<TDocument> update) where TDocument : class
    {
        var collection = GetCollection<TDocument>(collection_name);

        UpdateOptions update_options = new() { IsUpsert = true };
        return collection.UpdateOneAsync(filter, update, update_options);
    }

    public Task UpsertAsync<TDocument>(Expression<Func<TDocument, bool>> filter,
        string collection_name, TDocument doc) where TDocument : class
    {
        var collection = GetCollection<TDocument>(collection_name);

        // e => e.Id == doc.Id
        ReplaceOptions update_options = new() { IsUpsert = true };
        return collection.ReplaceOneAsync(filter, doc, update_options);
    }

    public Task InsertAsync<TDocument>(string collection_name, TDocument doc) where TDocument : class
    {
        var collection = GetCollection<TDocument>(collection_name);

        return collection.InsertOneAsync(doc);
    }

    public Task InsertManyAsync<TDocument>(string collection_name,
        IEnumerable<TDocument> docs) where TDocument : class
    {
        var collection = GetCollection<TDocument>(collection_name);

        return collection.InsertManyAsync(docs);
    }

    public Task<DeleteResult> DeleteOneAsync<TDocument>(string collection_name, string key)
    {
        var collection = GetCollection<TDocument>(collection_name);

        var builder = Builders<TDocument>.Filter.Eq(DOC_DEFAULT_ID_NAME, key);

        return collection.DeleteOneAsync(builder);
    }

    public Task DeleteOneAsync<TDocument>(string collection_name, FilterDefinition<TDocument> filter)
    {
        var collection = GetCollection<TDocument>(collection_name);

        return collection.DeleteOneAsync(filter);
    }

    public Task DeleteManyAsync<TDocument>(string collection_name, FilterDefinition<TDocument> filter)
    {
        var collection = GetCollection<TDocument>(collection_name);

        return collection.DeleteManyAsync(filter);
    }

    public Task InsertOneData<TDocument>(string collection_name, TDocument doc) where TDocument : class
    {
        var collection = GetCollection<TDocument>(collection_name);

        return collection.InsertOneAsync(doc);
    }

    public Task ReplaceOneData<TDocument>(string collection_name, string key, TDocument doc) where TDocument : class
    {
        var collection = GetCollection<TDocument>(collection_name);

        var builder = Builders<TDocument>.Filter.Eq(DOC_DEFAULT_ID_NAME, key);
        ReplaceOptions replace_options = new() { IsUpsert = true };
        return collection.ReplaceOneAsync(builder, doc, replace_options);
    }

    public Task CreateIndex(string name, bool is_ascending, params string[] prop_indextext)
    {
        var collection = GetCollection<BsonDocument>(name);
        var collecion_indexes = collection.Indexes;

        List<CreateIndexModel<BsonDocument>> list_indexs = new();
        foreach (var i in prop_indextext)
        {
            BsonDocument index = new()
            {
                { i, is_ascending ? 1 : -1 }
            };
            CreateIndexModel<BsonDocument> create_index = new(index);
            list_indexs.Add(create_index);
        }

        Task t = collecion_indexes.CreateManyAsync(list_indexs);

        return t;
    }

    public Task<IEnumerable<string>> CreateIndexEx<T>(string name, params IndexKeysDefinition<T>[] index_definition)
    {
        var collection = GetCollection<T>(name);
        var collecion_indexes = collection.Indexes;

        List<CreateIndexModel<T>> list_indexs = [];
        foreach (var i in index_definition)
        {
            CreateIndexModel<T> create_index = new(i);
            list_indexs.Add(create_index);
        }
        return collecion_indexes.CreateManyAsync(list_indexs);
    }

    //public Task<IEnumerable<string>> CreateManyAsync<TDocument>(string collection_name,
    //    IEnumerable<CreateIndexModel<TDocument>> models)
    //{
    //    var collection = GetCollection<TDocument>(collection_name);

    //    return collection.Indexes.CreateManyAsync(models);
    //}

    public IMongoCollection<TDocument> GetCollection<TDocument>(string name)
    {
        return Database.GetCollection<TDocument>(name);
    }

    public void CreateCollection<TDocument>(string name)
    {
        try
        {
            Database.CreateCollection(name);
        }
        catch (Exception)
        {
        }
    }

    public void DropCollection(string name)
    {
        Database.DropCollection(name);
    }

    public Task DropCollectionAsync(string name)
    {
        return Database.DropCollectionAsync(name);
    }
}