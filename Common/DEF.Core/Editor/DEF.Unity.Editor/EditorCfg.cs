using System.IO;

public class EditorCfg
{
    public ClientCfg4Runtime ClientCfg4Runtime { get; set; }
    public ClientCfg4Project ClientCfg4Project { get; private set; }
    public ClientCfg4User ClientCfg4User { get; private set; }

    public EditorCfg()
    {
        // Load or Gen ClientCfg4Runtime.txt
        {
            if (!Directory.Exists(EditorContext.Instance.PathResources))
            {
                Directory.CreateDirectory(EditorContext.Instance.PathResources);
            }

            string full_filename = Path.Combine(EditorContext.Instance.PathResources, EditorStringDef.FileClientCfg4Runtime);
            if (File.Exists(full_filename))
            {
                using StreamReader sr = File.OpenText(full_filename);
                string s = sr.ReadToEnd();
                ClientCfg4Runtime = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientCfg4Runtime>(s);
            }

            if (ClientCfg4Runtime == null)
            {
                ClientCfg4Runtime = new();
                ClientCfg4Runtime.Init();

                SaveClientCfg4Runtime();
            }
        }

        // Load or Gen ClientCfg4Project.json
        {
            if (!Directory.Exists(EditorContext.Instance.PathSettings))
            {
                Directory.CreateDirectory(EditorContext.Instance.PathSettings);
            }

            string full_filename = Path.Combine(EditorContext.Instance.PathSettings, EditorStringDef.FileClientCfg4Project);
            if (File.Exists(full_filename))
            {
                using StreamReader sr = File.OpenText(full_filename);
                string s = sr.ReadToEnd();
                ClientCfg4Project = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientCfg4Project>(s);
            }

            if (ClientCfg4Project == null)
            {
                ClientCfg4Project = new()
                {
                    ListCodeDir = new(),
                    ListExcludeReferences = new(),
                };

                SaveClientCfg4Project();
            }
        }

        // Load or Gen ClientCfg4User.json
        {
            if (!Directory.Exists(EditorContext.Instance.PathSettingsUser))
            {
                Directory.CreateDirectory(EditorContext.Instance.PathSettingsUser);
            }

            string full_filename = Path.Combine(EditorContext.Instance.PathSettingsUser, EditorStringDef.FileClientCfg4User);
            if (File.Exists(full_filename))
            {
                using StreamReader sr = File.OpenText(full_filename);
                string s = sr.ReadToEnd();
                ClientCfg4User = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientCfg4User>(s);
            }

            if (ClientCfg4User == null)
            {
                ClientCfg4User = new()
                {
                    CurrentEnv = "DevLocal",
                    UpdaterMode = UpdaterMode.EditorPlayMode,
                    TestMode = TestMode.None,
                    TestMode1Params = string.Empty,
                    TestMode2Params = string.Empty,
                };

                SaveClientCfg4User();
            }
        }
    }

    // 加载ClientCfg4Runtime.txt
    public void LoadClientCfg4Runtime()
    {
        string full_filename = Path.Combine(EditorContext.Instance.PathResources, EditorStringDef.FileClientCfg4Runtime);
        if (File.Exists(full_filename))
        {
            using StreamReader sr = File.OpenText(full_filename);
            string s = sr.ReadToEnd();
            ClientCfg4Runtime = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientCfg4Runtime>(s);
        }
    }

    // 保存ClientCfg4Runtime.txt
    public void SaveClientCfg4Runtime()
    {
        string full_filename = Path.Combine(EditorContext.Instance.PathResources, EditorStringDef.FileClientCfg4Runtime);

        using StreamWriter sw = File.CreateText(full_filename);
        string s = Newtonsoft.Json.JsonConvert.SerializeObject(ClientCfg4Runtime, Newtonsoft.Json.Formatting.Indented);
        sw.Write(s);
    }

    // 加载ClientCfg4Project.json
    public void LoadClientCfg4Project()
    {
        string full_filename = Path.Combine(EditorContext.Instance.PathSettings, EditorStringDef.FileClientCfg4Project);
        if (File.Exists(full_filename))
        {
            using StreamReader sr = File.OpenText(full_filename);
            string s = sr.ReadToEnd();
            ClientCfg4Project = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientCfg4Project>(s);
        }
    }

    // 保存ClientCfg4Project.json
    public void SaveClientCfg4Project()
    {
        string full_filename = Path.Combine(EditorContext.Instance.PathSettings, EditorStringDef.FileClientCfg4Project);

        using StreamWriter sw = File.CreateText(full_filename);
        string s = Newtonsoft.Json.JsonConvert.SerializeObject(ClientCfg4Project, Newtonsoft.Json.Formatting.Indented);
        sw.Write(s);
    }

    // 保存ClientCfg4User.json
    public void SaveClientCfg4User()
    {
        string full_filename = Path.Combine(EditorContext.Instance.PathSettingsUser, EditorStringDef.FileClientCfg4User);

        using StreamWriter sw = File.CreateText(full_filename);
        string s = Newtonsoft.Json.JsonConvert.SerializeObject(ClientCfg4User, Newtonsoft.Json.Formatting.Indented);
        sw.Write(s);
    }
}