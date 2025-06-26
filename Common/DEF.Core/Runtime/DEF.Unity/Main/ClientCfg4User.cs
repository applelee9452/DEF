#if DEF_CLIENT

public class ClientCfg4User
{
    public string CurrentEnv { get; set; } = "Oss";// 在编辑器中时的指向的当前环境
    public string CurrentChannel { get; set; } = "Default";
    public UpdaterMode UpdaterMode { get; set; } = UpdaterMode.EditorPlayMode;
    public TestMode TestMode { get; set; } = TestMode.None;// 测试模式：无，测试模式1，测试模式2
    public string TestMode1Params { get; set; }// 测试模式1参数
    public string TestMode2Params { get; set; }// 测试模式2参数
}

#endif