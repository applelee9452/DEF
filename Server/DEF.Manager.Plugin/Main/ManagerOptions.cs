using MongoDB.Bson;

namespace DEF.Manager;

public class PluginItem
{
    public string AssemblyPath { get; set; }
    public string AssemblyName { get; set; }
    public string EntryType { get; set; }
}

public class InitDb
{
    public List<Role> ListRole { get; set; }
    public List<User> ListUser { get; set; }
}

public class ManagerOptions
{
    public const string Key = "DEF.Manager";

    public string LogFileName { get; set; } = "Manager";
    public int ListenPort { get; set; } = 5000;
    public string SslFileName { get; set; } = "";
    public string SslPwd { get; set; } = "";
    public string DbConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DBName { get; set; } = "Manager";
    public InitDb InitDb { get; set; } = new()
    {
        ListRole =
        [
            new Role()
            {
                Id = "Guest",
                Desc = "游客"
            },
            new Role()
            {
                Id = "Admin",
                Desc = "管理员"
            },
            new Role()
            {
                Id = "Platform",
                Desc = "平台"
            },
            new Role()
            {
                Id = "Agent",
                Desc = "代理"
            }
        ],

        ListUser =
        [
            new User()
            {
                UserName = "aabb",
                Password = "20242025",
                DisplayName = "aabb",
                Avatar = "avatar001",
                Role = RoleType.Admin,
            },
            new User()
            {
                UserName = "aabb2",
                Password = "20242025",
                DisplayName = "aabb",
                Avatar = "avatar001",
                Role = RoleType.Admin,
            }
        ]
    };
    public List<string> LogServices { get; set; } = [];
    public string PluginDefault { get; set; } = string.Empty;
    public List<PluginItem> Plugins { get; set; } = [];
}