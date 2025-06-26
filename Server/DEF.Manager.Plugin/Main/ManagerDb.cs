using System.ComponentModel;
using DEF.UCenter;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DEF.Manager;

public enum RoleType
{
    [Description("游客")]
    Guest = 0,// 最小的权限

    [Description("代理")]
    Agent,

    [Description("平台")]
    Platform,

    [Description("管理员")]
    Admin
}

public class User
{
    public string Id { get; set; }

    public string UserName { get; set; }// 唯一的，不可以重复

    public string Password { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public RoleType Role { get; set; }

    public ulong AgentId { get; set; }

    public List<ulong> AgentParents { get; set; } = [];

    public DateTime CreateDateTime { get; set; }

    public bool IsDelete { get; set; }// 是否标记为删除
}

public class Role
{
    public string Id { get; set; }// 角色Id，不重复的可读的字符串

    public string Desc { get; set; }// 角色描述
}

//public class Res
//{
//    public string Id { get; set; }

//    public string ResType { get; set; }

//    public string ResName { get; set; }

//    public string Desc { get; set; }// 资源描述
//}

//public class RoleRes
//{
//    [BsonId]
//    [BsonRepresentation(BsonType.ObjectId)]
//    public string Id { get; set; }

//    public string UserName { get; set; }

//    public string RoleId { get; set; }
//}

public class ManagerDb
{
    public ILogger Logger { get; set; }
    public IOptions<DEFOptions> DEFOptions { get; set; }
    public IOptions<ManagerOptions> ManagerOptions { get; set; }
    public ServiceClient ServiceClient { get; set; }
    public DbClientMongo Db { get; set; }

    public const string CollectionRole = "Role";// 角色表
    public const string CollectionUser = "User";// 用户表
    public const string CollectionRes = "Res";// 资源表
    public const string CollectionRoleRes = "RoleRes";// 角色资源权限表

    public ManagerDb(
        ILogger<ManagerSession> logger,
        IOptions<DEFOptions> def_options,
        IOptions<ManagerOptions> manager_options,
        ServiceClient serviceClient)
    {
        Logger = logger;
        DEFOptions = def_options;
        ManagerOptions = manager_options;
        ServiceClient = serviceClient;

        Db = new DbClientMongo(ManagerOptions.Value.DBName, ManagerOptions.Value.DbConnectionString);
    }

    public async Task<bool> Authenticate(string user_name, string password)
    {
        if (string.IsNullOrEmpty(user_name) || string.IsNullOrEmpty(password))
        {
            return false;
        }

        var user = await GetUserByUserName(user_name, false);

        if (user == null)
        {
            return false;
        }

        if (user.Password != password)
        {
            return false;
        }

        return true;
    }

    //public async Task<List<User>> GetUserList(string user_name, int page_index, int page_items)
    //{
    //    var user = await GetUserByUserName(user_name, false);

    //    // 根据当前用户角色读取可见用户列表
    //    var list = new List<User>();

    //    if (user.Role == RoleType.Admin)
    //    {
    //        list = await Db.ReadListAsync<User>(f => !f.IsDelete, CollectionUser);
    //    }
    //    else if (user.Role == RoleType.Platform)
    //    {
    //        list = await Db.ReadListAsync<User>(f => f.Role < RoleType.Admin && !f.IsDelete, CollectionUser);
    //    }
    //    else
    //    {
    //        list = await Db.ReadListAsync<User>(f => f.Role == RoleType.Agent && !f.IsDelete && f.AgentParents.Contains(user.AgentId), CollectionUser);
    //    }

    //    return list;
    //}

    // 获取指定用户的角色
    public async Task<RoleType> GetRole(string user_name)
    {
        var user = await GetUserByUserName(user_name, true);

        return user.Role;
    }

    // 通过用户名获取指定用户
    public async Task<User> GetUserByUserName(string user_name, bool ignore_isdelete)
    {
        if (ignore_isdelete)
        {
            var user = await Db.ReadAsync<User>(a => a.UserName == user_name, CollectionUser);

            return user;
        }
        else
        {
            var user = await Db.ReadAsync<User>(a => a.UserName == user_name && !a.IsDelete, CollectionUser);

            return user;
        }
    }

    // 向数据库添加一组角色
    public async Task CreateRoles(List<Role> list_role)
    {
        foreach (var role in list_role)
        {
            var role2 = await Db.ReadAsync<Role>(
                a => a.Id == role.Id,
                CollectionRole);

            if (role2 != null)
            {
                await Db.UpsertAsync(a => a.Id == role.Id, CollectionRole, role);
            }
            else
            {
                await Db.InsertAsync(CollectionRole, role);
            }
        }
    }

    // 向数据库添加管理员用户
    public async Task CreateAdmin(List<User> list_user)
    {
        // 创建用户
        foreach (var user in list_user)
        {
            var user2 = await Db.ReadAsync<User>(
                a => a.UserName == user.UserName,
                CollectionUser);

            if (user2 != null)
            {
                continue;
            }

            user.Id = Guid.NewGuid().ToString();
            user.Avatar = "avatar001";
            user.CreateDateTime = DateTime.UtcNow;

            await Db.InsertAsync(CollectionUser, user);
        }
    }

    // 向数据库添加或更新一组用户
    public async Task CreateUsers(User current_user, List<User> list_user)
    {
        // 检测当前登录用户的api权限

        if (current_user.Role <= RoleType.Guest)
        {
            return;
        }
        else if (current_user.Role == RoleType.Agent)
        {
            if (current_user.AgentParents != null && current_user.AgentParents.Count() >= 2)
            {
                return;
            }
        }

        // 校验参数
        if (list_user == null) return;

        // 创建用户
        foreach (var user in list_user)
        {
            var user2 = await Db.ReadAsync<User>(
                a => a.UserName == user.UserName,
                CollectionUser);

            if (user2 != null)
            {
                continue;
            }

            if (user.Role == RoleType.Agent)
            {
                var c = ServiceClient.GetContainerRpc<IContainerStatelessUCenter>();
                var agent_id = await c.RequestCreateAgent(user.UserName, current_user.AgentId);

                user.AgentId = agent_id;
            }

            user.Id = Guid.NewGuid().ToString();
            user.Avatar = "avatar001";
            user.CreateDateTime = DateTime.UtcNow;

            await Db.InsertAsync(CollectionUser, user);
        }
    }

    // 更新已存在的用戶
    public async Task UpdateUser(User current_user, User user)
    {
        // 检测当前登录用户的api权限

        if (current_user.Role <= RoleType.Guest)
        {
            return;
        }
        else if (current_user.Role == RoleType.Agent)
        {
            if (current_user.AgentParents != null && current_user.AgentParents.Count() >= 2)
            {
                return;
            }
        }

        // 校验参数
        if (user == null) return;

        // 更新用户
        var user2 = await Db.ReadAsync<User>(
            a => a.UserName == user.UserName,
            CollectionUser);

        if (user2 == null)
        {
            return;
        }

        user2.Password = user.Password;
        user2.DisplayName = user.DisplayName;
        user2.Role = user.Role;

        await Db.UpsertAsync(a => a.Id == user2.Id, CollectionUser, user2);
    }

    // 从数据库中删除一组用户
    public async Task DeleteUsers(User current_user, List<User> list_user)
    {
        // 检测当前登录用户的api权限

        if (current_user.Role <= RoleType.Guest)
        {
            return;
        }
        else if (current_user.Role == RoleType.Agent)
        {
            if (current_user.AgentParents != null && current_user.AgentParents.Count() >= 2)
            {
                return;
            }
        }

        // 校验参数
        if (list_user == null) return;

        // 删除用户
        list_user.RemoveAll(i => i.Role == RoleType.Admin);

        if (list_user.Count == 0) return;

        List<Task> tasks = [];
        foreach (var user in list_user)
        {
            var c = ServiceClient.GetContainerRpc<IContainerStatelessUCenter>();
            var t = c.RequestDeleteAgent(user.AgentId);
            tasks.Add(t);
        }
        await Task.WhenAll(tasks);

        var filter = Builders<User>.Filter.Or(list_user.Select(i => Builders<User>.Filter.Eq(user => user.UserName, i.UserName)));

        var update = Builders<User>.Update.Set(x => x.IsDelete, true);

        UpdateOptions update_options = new() { IsUpsert = true };

        var collection = Db.GetCollection<User>(CollectionUser);
        await collection.UpdateManyAsync(filter, update, update_options);
    }
}