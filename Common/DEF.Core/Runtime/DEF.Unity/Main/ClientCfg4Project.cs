#if DEF_CLIENT

using System.Collections.Generic;

public class ClientCfg4Project
{
    public List<string> ListCodeDir { get; set; }// 热更新代码目录列表
    public List<string> ListExcludeReferences { get; set; }// 编译热更新代码时，需要排除的引用Dll
    public string GenViewCodeDir { get; set; }// 生成View代码目录
    public string ClientNameSpace { get; set; }// 客户端命名空间
}

#endif