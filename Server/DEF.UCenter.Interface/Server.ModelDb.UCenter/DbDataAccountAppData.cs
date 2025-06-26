namespace DEF.UCenter;

// 账号与App的关联文档，可以同时关联一个数据
public class DataAccountAppData : DataBase
{
    public string AppId { get; set; }
    public string AccountId { get; set; }
    public string Data { get; set; }
    public Dictionary<string, string> DataDictionary { get; set; }
    public Dictionary<string, string> Data4Auth { get; set; }// 网关认证通过后将该Dic返回给网关
}