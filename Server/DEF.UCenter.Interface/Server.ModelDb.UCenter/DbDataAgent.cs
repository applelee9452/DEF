namespace DEF.UCenter;

// 代理文档
#if !DEF_CLIENT
[GenerateSerializer]
#endif
public class DataAgent : DataBase
{
#if !DEF_CLIENT
    [Id(0)]
#endif
    public AccountStatus AccountStatus { get; set; }// 正常，禁用

#if !DEF_CLIENT
    [Id(1)]
#endif
    public ulong AgentId { get; set; }// 代理唯一Id，不可以为空，长整数

#if !DEF_CLIENT
    [Id(2)]
#endif
    public ulong[] AgentParents { get; set; }// 父代理唯一Id，长整数，所有层级直系父代理，第一个元素的直接父，第二个元素是父父

#if !DEF_CLIENT
    [Id(3)]
#endif
    public string UserName { get; set; }// 代理在管理后台的用户名

#if !DEF_CLIENT
    [Id(8)]
#endif
    public bool IsDelete { get; set; }// 是否标记为已删除
}

public class DataAgentView
{
    public string Id { get; set; }
    public string UserName { get; set; }// 代理在管理后台的用户名
    public AccountStatus AccountStatus { get; set; }// 正常，禁用
    public ulong AgentId { get; set; }// 代理唯一Id，不可以为空，长整数
    public ulong[] AgentParents { get; set; }// 父代理唯一Id，长整数，所有层级直系父代理，第一个元素的直接父，第二个元素是父父
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    public bool IsDelete { get; set; }// 是否标记为已删除
    public string StrAgentParent { get; set; } = string.Empty;
}