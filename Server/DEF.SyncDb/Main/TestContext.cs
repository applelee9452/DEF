using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DEF.SyncDb;

public class TestContext
{
    public static TestContext Instance { get; private set; }
    public IMongoClient MongoClient { get; private set; }
    public ILogger Logger { get; private set; }
    public Random Rd { get; private set; } = new();
    TestInitDb TestInitDb { get; set; }
    TestEntity TestEntity { get; set; }

    public TestContext(ILogger<TestContext> logger, TestEntity test_entity)
    {
        Instance = this;
        Logger = logger;
        TestEntity = test_entity;

        var connection_str = "mongodb://localhost:27017";
        MongoClient = new MongoClient(connection_str);

        var object_serializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || type.FullName.StartsWith("DEF.SyncDb"));
        BsonSerializer.RegisterSerializer(object_serializer);
    }

    public async Task StartAsync(CancellationToken _)
    {
        await Task.Delay(1, _);

        Logger.LogInformation("TestContext启动成功！");

        await TestEntity.Setup();

        //TestInitDb = new();
        //await TestInitDb.Setup();

        //var db = MflixDbContext.Create(mongo_db);

        //db.Database.EnsureCreated();

        //var movie = await db.Movies.FirstOrDefaultAsync(m => m.Title == "Back to the Future");

        //Console.WriteLine(movie.Plot);
    }

    public async Task StopAsync(CancellationToken _)
    {
        await Task.Delay(1, _);

        Logger.LogInformation("TestContext停止成功！");
    }

    //public async Task TestInitDb()
    //{
    //    var mongo_db = MongoClient.GetDatabase("Test");
    //    var collection_data_account = mongo_db.GetCollection<DataAccount>(typeof(DataAccount).Name);

    //    var list = await collection_data_account.Find(FilterDefinition<DataAccount>.Empty).ToListAsync();

    //    foreach (var item in list)
    //    {
    //        Console.WriteLine(item.AccountName);
    //    }
    //}

    public async Task TestAddEntity()
    {
        var mongo_db = MongoClient.GetDatabase("Test");
        var collection_data_account = mongo_db.GetCollection<DataAccount>(typeof(DataAccount).Name);

        var list = await collection_data_account.Find(FilterDefinition<DataAccount>.Empty).ToListAsync();

        foreach (var item in list)
        {
            Console.WriteLine(item.AccountName);
        }
    }

    public async Task TestDeleteEntity()
    {
        await Task.Delay(1);

        var mongo_db = MongoClient.GetDatabase("Test");
        var collection_data_account = mongo_db.GetCollection<DataAccount>(typeof(DataAccount).Name);
        var collection_ev_loginlogout = mongo_db.GetCollection<EvAccountLoginLogout>(typeof(EvAccountLoginLogout).Name);

        var match_filter = Builders<EvAccountLoginLogout>.Filter.Empty;

        var sort_filter = Builders<EvAccountLoginLogout>.Sort.Ascending(x => x.Dt.Day);

        var pipeline = new EmptyPipelineDefinition<EvAccountLoginLogout>()
            .Match(match_filter)
            .Group(r => r.Dt.Day,
                g => new
                {
                    Day = g.Key,
                    Count = g.Count(),
                    //Order = g.Order(a => a.)
                    //Distinct = g.Distinct(e => e.AccountId)
                }
            );
        //.Sort(sort_filter);

        var results = collection_ev_loginlogout.Aggregate(pipeline).ToList();

        Console.WriteLine(results.Count);
        foreach (var result in results)
        {
            Console.WriteLine(result);
        }
    }
}