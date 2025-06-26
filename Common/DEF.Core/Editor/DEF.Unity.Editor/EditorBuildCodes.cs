using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

public static class EditorBuildCodes
{
    const string BuildOutputDir = "./ScriptDll";

    public static void BuildCodeDebug()
    {
        var list_codedir = EditorContext.Instance.EditorCfg.ClientCfg4Project.ListCodeDir;

        if (list_codedir == null || list_codedir.Count == 0)
        {
            Debug.LogWarning("跳过编译，没有配置项目目录列表");
            return;
        }

        List<string> code_dirs = new();
        if (list_codedir != null)
        {
            code_dirs.AddRange(list_codedir);
        }

        bool result = BuildMuteAssembly("Script", code_dirs.ToArray());

        if (result)
        {
            AfterCompiling();

            AssetDatabase.Refresh();
        }
    }

    static void GetFiles(string dir, List<string> list_file, List<string> excludes)
    {
        DirectoryInfo di = new(dir);

        var arr_file = di.GetFiles("*.cs");
        foreach (var i in arr_file)
        {
            list_file.Add(i.FullName);
        }

        var arr_dir = di.GetDirectories();
        foreach (var i in arr_dir)
        {
            bool contain = false;
            foreach (var k in excludes)
            {
                if (i.FullName.Contains(k))
                {
                    contain = true;
                    break;
                }
            }

            if (contain) continue;

            GetFiles(i.FullName, list_file, excludes);
        }
    }

    static bool BuildMuteAssembly(string assembly_name, string[] code_dirs)
    {
        if (!Directory.Exists(BuildOutputDir))
        {
            Directory.CreateDirectory(BuildOutputDir);
        }

        List<string> excludes = new() { "\\obj", "\\bin" };
        List<string> scripts = new();
        for (int i = 0; i < code_dirs.Length; i++)
        {
            GetFiles(code_dirs[i], scripts, excludes);
        }

        string dllPath = Path.Combine(BuildOutputDir, $"{assembly_name}.dll");
        string pdbPath = Path.Combine(BuildOutputDir, $"{assembly_name}.pdb");
        File.Delete(dllPath);
        File.Delete(pdbPath);

        Directory.CreateDirectory(BuildOutputDir);

        //BuildTargetGroup build_target_group = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

        AssemblyBuilder assembly_builder = new(dllPath, scripts.ToArray());
        assembly_builder.compilerOptions.AllowUnsafeCode = true;// 启用UnSafe
        assembly_builder.compilerOptions.CodeOptimization = CodeOptimization.Release;
        //assembly_builder.compilerOptions.LanguageVersion = "10.0";
        //#if UNITY_2021_3_14
        //        assembly_builder.compilerOptions.RoslynAnalyzerDllPaths = new string[] { @"../../DEF/Common/DEF.Core/Libraries/DEF.CodeGenerator.dll" };
        //#endif
        //#if DEF_HYBRIDCLR
        //        assembly_builder.compilerOptions.ApiCompatibilityLevel = ApiCompatibilityLevel.NET_Unity_4_8;
        //#else
        //        assembly_builder.compilerOptions.ApiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(build_target_group);
        //#endif

        //string[] additional_references = new string[]
        //{ 
        //    //"Library\\PackageCache\\com.qq.weixin.minigame@207766f6db\\Runtime\\Plugins\\wx-runtime.dll",
        //    //"Library\\PackageCache\\com.qq.weixin.minigame@207766f6db\\Runtime\\Plugins\\wx-runtime-editor.dll",
        //};
        //assembly_builder.additionalReferences = additional_references;

        //assembly_builder.compilerOptions.AdditionalCompilerArguments = new string[] { "-nowarn:CS1701", "-nowarn:CS0169", "-nowarn:CS0414" };
        assembly_builder.flags = AssemblyBuilderFlags.None;
        assembly_builder.referencesOptions = ReferencesOptions.UseEngineModules;
        //assembly_builder.buildTarget = EditorUserBuildSettings.activeBuildTarget;
        //assembly_builder.buildTargetGroup = build_target_group;

        var list_exclude_references = EditorContext.Instance.EditorCfg.ClientCfg4Project.ListExcludeReferences;
        list_exclude_references ??= new();
        list_exclude_references.Add("Library/ScriptAssemblies/Script.dll");
        list_exclude_references.Add("Library/ScriptAssemblies/Guaji.Interface.dll");
        list_exclude_references.Add("Library/ScriptAssemblies/Guaji.Impl.dll");
        //list_exclude_references.Add("Library\\PackageCache\\com.qq.weixin.minigame@207766f6db\\Runtime\\Plugins\\wx-runtime.dll");
        assembly_builder.excludeReferences = list_exclude_references.ToArray();

        assembly_builder.buildFinished += delegate (string assembly_path, CompilerMessage[] compiler_messages)
        {
            int error_count = compiler_messages.Count(m => m.type == CompilerMessageType.Error);
            int warning_count = compiler_messages.Count(m => m.type == CompilerMessageType.Warning);
            int info_count = compiler_messages.Count(m => m.type == CompilerMessageType.Info);

            Debug.LogFormat("Infos：{0}，Warnings: {1}，Errors: {2}", info_count, warning_count, error_count);

            if (info_count > 0)
            {
                for (int i = 0; i < compiler_messages.Length; i++)
                {
                    if (compiler_messages[i].type == CompilerMessageType.Info)
                    {
                        Debug.Log(compiler_messages[i].message);
                    }
                }
            }

            if (warning_count > 0)
            {
                for (int i = 0; i < compiler_messages.Length; i++)
                {
                    if (compiler_messages[i].type == CompilerMessageType.Warning)
                    {
                        Debug.LogWarning(compiler_messages[i].message);
                    }
                }
            }

            if (error_count > 0)
            {
                for (int i = 0; i < compiler_messages.Length; i++)
                {
                    if (compiler_messages[i].type == CompilerMessageType.Error)
                    {
                        Debug.LogError(compiler_messages[i].message);
                    }
                }
            }
        };

        // 开始构建
        if (!assembly_builder.Build())
        {
            Debug.LogErrorFormat("Build Failed：" + assembly_builder.assemblyPath);
            return false;
        }

        return true;
    }

    static void AfterCompiling()
    {
        while (EditorApplication.isCompiling)
        {
            Thread.Sleep(50);
        }

        if (EditorContext.Instance.EditorCfg.ClientCfg4Runtime.RuntimeMode == RuntimeMode.None)
        {
            //string string_dir = "Library/ScriptAssemblies/";
            //Directory.CreateDirectory(string_dir);
            //File.Copy(Path.Combine(BuildOutputDir, "Script.dll"), Path.Combine(string_dir, "Script.dll"), true);
            //File.Copy(Path.Combine(BuildOutputDir, "Script.pdb"), Path.Combine(string_dir, "Script.pdb"), true);

            CopyDll();

            AssetDatabase.Refresh();

            Debug.Log("Copy ScriptTmp.dll to Library/ScriptAssemblies success!");
        }

        if (EditorContext.Instance.EditorCfg.ClientCfg4Runtime.RuntimeMode == RuntimeMode.HybridCLR)
        {
            string string_dir = "Assets/Arts/S/";
            Directory.CreateDirectory(string_dir);

            var cfg = EditorContext.Instance.EditorCfg.ClientCfg4Runtime;

            if (cfg.IsEncrypt
                && !string.IsNullOrEmpty(cfg.EncryptKey)
                && !string.IsNullOrEmpty(cfg.EncryptNonce))
            {
                string dll_src_fullfilename = Path.Combine(BuildOutputDir, "Script.dll");
                string dll_dst_fullfilename = Path.Combine(string_dir, "Script.dll.bytes");

                string pdb_src_fullfilename = Path.Combine(BuildOutputDir, "Script.pdb");
                string pdb_dst_fullfilename = Path.Combine(string_dir, "Script.pdb.bytes");

                var dll_bytes = File.ReadAllBytes(dll_src_fullfilename);
                var pdb_bytes = File.ReadAllBytes(pdb_src_fullfilename);

                byte[] dll_bytes2 = new byte[dll_bytes.Length];
                byte[] pdb_bytes2 = new byte[pdb_bytes.Length];

                byte[] key = System.Convert.FromBase64String(cfg.EncryptKey);
                byte[] nonce = System.Convert.FromBase64String(cfg.EncryptNonce);

                DEF.ChaCha20 forDecrypting = new(key, nonce, 1);
                forDecrypting.EncryptBytes(dll_bytes2, dll_bytes, dll_bytes.Length);
                forDecrypting.EncryptBytes(pdb_bytes2, pdb_bytes, pdb_bytes.Length);

                File.WriteAllBytes(dll_dst_fullfilename, dll_bytes2);
                File.WriteAllBytes(pdb_dst_fullfilename, pdb_bytes2);

                forDecrypting.Dispose();
            }
            else
            {
                File.Copy(Path.Combine(BuildOutputDir, "Script.dll"), Path.Combine(string_dir, "Script.dll.bytes"), true);
                File.Copy(Path.Combine(BuildOutputDir, "Script.pdb"), Path.Combine(string_dir, "Script.pdb.bytes"), true);
            }

            AssetDatabase.Refresh();

            Debug.Log("Copy ScriptTmp.dll to Assets/Arts/S success!");
        }

        Debug.Log("Build Code Success");
    }

    [UnityEditor.InitializeOnLoadMethod]
    [UnityEditor.Callbacks.PostProcessScene]
    static void CopyDll()
    {
#if !DEF_HYBRIDCLR
        CopyDll("Library/ScriptAssemblies/");
        CopyDll("Library/PlayerScriptAssemblies/");
        CopyDll("Temp/StagingArea/Data/Managed/");
#endif
    }

    private static void CopyDll(string string_dir)
    {
        if (Directory.Exists(string_dir))
        {
            string f1 = Path.Combine(BuildOutputDir, "Script.dll");
            if (File.Exists(f1))
            {
                File.Copy(f1, Path.Combine(string_dir, "Script.dll"), true);
            }

            string f2 = Path.Combine(BuildOutputDir, "Script.pdb");
            if (File.Exists(f2))
            {
                File.Copy(f2, Path.Combine(string_dir, "Script.pdb"), true);
            }
        }
    }
}