#if DEF_CLIENT

#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UGuiGo
{
    public string Key;
    public UnityEngine.Object Obj;
}

[AttributeUsage(AttributeTargets.Class)]
public class ViewFactoryAttribute : Attribute
{
    public ViewFactoryAttribute()
    {
    }
}

public class UGuiView : MonoBehaviour
{
    [SerializeField]
    public List<UGuiGo> Objs;

    public T GetObj<T>(string key) where T : UnityEngine.Object
    {
        foreach (var obj in Objs)
        {
            if (obj.Key == key)
            {
                return typeof(T) == typeof(GameObject) ? (T)obj.Obj : ((GameObject)obj.Obj).GetComponent<T>();
            }
        }
        return default;
    }

    public Transform GetObjAsTransform(string key)
    {
        return GetObj<Transform>(key);
    }

    public RectTransform GetObjAsRectTransform(string key)
    {
        return GetObj<RectTransform>(key);
    }

    public TextMeshProUGUI GetObjAsTextMeshProUGUI(string key)
    {
        return GetObj<TextMeshProUGUI>(key);
    }

    public TMP_InputField GetObjAsTmpInputField(string key)
    {
        return GetObj<TMP_InputField>(key);
    }

    public Text GetObjAsText(string key)
    {
        return GetObj<Text>(key);
    }

    public Image GetObjAsImage(string key)
    {
        return GetObj<Image>(key);
    }

    public Button GetObjAsButton(string key)
    {
        return GetObj<Button>(key);
    }

    public TMP_Dropdown GetObjAsDropdownTMP(string key)
    {
        return GetObj<TMP_Dropdown>(key);
    }

    public Toggle GetObjAsToggle(string key)
    {
        return GetObj<Toggle>(key);
    }

    public ScrollRect GetObjAsScrollRect(string key)
    {
        return GetObj<ScrollRect>(key);
    }

    void OnMessage(string msg)
    {
        var wrapper = Client.AssemblesWrapper;
        wrapper?.OnUGuiMsg(gameObject, msg);
    }

#if UNITY_EDITOR
    //[PropertySpace(SpaceBefore = 30, SpaceAfter = 30)]

    [Title("View层级路径")]
    public string ViewLayer = "CanvasLobby/LayerPanel";
    public bool EnableOpenAndCloseAudio = false;

    [Button("生成代码", ButtonSizes.Gigantic), GUIColor(0.1f, 0.9f, 0.1f)]
    void OnGenerateViewCode()
    {
        Debug.Log("点击生成代码按钮");

        // 读取项目配置信息
        ClientCfg4Project cfg_project = null;
        {
            string p1 = Path.Combine(Environment.CurrentDirectory, "./Assets/Settings/");
            var di = new DirectoryInfo(p1);
            string path_settings = di.FullName;

            string full_filename = Path.Combine(path_settings, "ClientCfg4Project.json");
            if (File.Exists(full_filename))
            {
                using StreamReader sr = File.OpenText(full_filename);
                string s = sr.ReadToEnd();
                cfg_project = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientCfg4Project>(s);
            }
        }

        if (cfg_project == null)
        {
            Debug.LogError("请先通过DEF菜单生成项目配置！");
            return;
        }

        // 所有按钮组件都自动挂载UGUIComponent并设置OnClick事件
        var arr_button = gameObject.transform.GetComponentsInChildren<Button>(true);
        if (arr_button != null && arr_button.Length > 0)
        {
            foreach (var btn in arr_button)
            {
                var ugui_com = btn.GetComponent<UGuiComponent>();
                if (ugui_com == null)
                {
                    ugui_com = btn.gameObject.AddComponent<UGuiComponent>();
                }

                //var m_OnClick = btn.GetType().GetField("m_OnClick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var btnEvent = UnityEngine.Events.UnityAction.CreateDelegate(typeof(UnityAction), ugui_com, "OnButtonClick") as UnityAction;

                int count = btn.onClick.GetPersistentEventCount();
                if (count == 0)
                {
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, btnEvent);
                }
            }
        }

        GenViewGenCs(cfg_project);

        GenViewCs(cfg_project);
    }

    void GenViewGenCs(ClientCfg4Project cfg_project)
    {
        // 获取待生成View代码的目录
        string p = Path.Combine(Environment.CurrentDirectory, cfg_project.GenViewCodeDir);
        p = Path.GetFullPath(p);
        string cs_filename = $"View{gameObject.name}.Gen.cs";
        string cs_filename_fullpath = Path.Combine(p, cs_filename);
        cs_filename_fullpath = Path.GetFullPath(cs_filename_fullpath);
        Debug.Log(cs_filename_fullpath);

        if (File.Exists(cs_filename_fullpath))
        {
            // todo
            // 检查View类的一致性，如果不一致，则重新生成View类。所有变量类型相同，所有变量名相同，变量个数相同
            // 检查ViewFactory类，不存在则生成，存在则不执行任何操作

            //var s = File.ReadAllText(cs_filename_fullpath);

            //var parseOptions = new CSharpParseOptions().WithLanguageVersion(LanguageVersion.CSharp11);
            //var compilationOptions = new CSharpCompilationOptions(OutputKind.NetModule);
            ////.WithNullableContextOptions(NullableContextOptions.Enable);

            //var tree = CSharpSyntaxTree.ParseText(cs_filename_fullpath, parseOptions);
            //var root = tree.GetCompilationUnitRoot();
            //var compilation = CSharpCompilation.Create(string.Empty, options: compilationOptions)
            //    //.AddReferences(MetadataReference.CreateFromFile(typeof(string).Assembly.Location))
            //    .AddSyntaxTrees(tree);
            //var semanticModel = compilation.GetSemanticModel(tree);
            //// 检查是否存在语义错误
            //var diagnostics = semanticModel.GetDiagnostics();
            //var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
            //if (errors.Count > 0)
            //{
            //    foreach (var i in errors)
            //    {
            //        Debug.LogError(i.ToString());
            //    }
            //}

            // 创建SyntaxTree
            //var parse_options = new CSharpParseOptions().WithLanguageVersion(LanguageVersion.CSharp11);
            //SyntaxTree tree = CSharpSyntaxTree.ParseText(s, parse_options);

            // 获取根节点
            //CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            //var semanticModel = compilation.GetSemanticModel(syntaxTree);

            //var class_declaration_syntaxes = from node in tree.GetRoot().DescendantNodes()
            //                                 where node.IsKind(SyntaxKind.ClassDeclaration)
            //                                 select (ClassDeclarationSyntax)node;

            //foreach (var class_declaration_syntax in class_declaration_syntaxes)
            //{
            //    //if (semanticModel.GetDeclaredSymbol(class_declaration_syntax) is { } classDeclarationSymbol)
            //    //{
            //    //    // 在这里使用你的类型定义语义符号。

            //    //    int aa = 0;
            //    //}
            //}

            //var root = (CompilationUnitSyntax)tree.GetRoot();

            // 遍历语法树
            //foreach (var node in root.DescendantNodes())
            //{
            //    // 在这里可以对语法树节点进行操作
            //    Debug.Log(node.Kind() + "  " + node.ToFullString());

            //    node.IsKind(SyntaxKind.CompilationUnit);
            //}
        }
        else
        {
        }

        // 生成View代码

        StringBuilder sb = new();

        sb.AppendLine(
            $"#if DEF_CLIENT\n" +
            $"\n" +
            $"using DEF.Client;\n" +
            $"using System;\n" +
            $"using System.Collections.Generic;\n" +
            $"using System.Threading.Tasks;\n" +
            $"using TMPro;\n" +
            $"using UnityEngine;\n" +
            $"using UnityEngine.UI;\n" +
            $"\n" +
            $"namespace {cfg_project.ClientNameSpace}\n" +
            $"{{");

        sb.AppendLine($"    public partial class View{gameObject.name} : View");
        sb.AppendLine($"    {{");

        for (int i = 0; i < Objs.Count; i++)
        {
            var obj = Objs[i];
            var name = obj.Key;
            var type = GetObjType(name);
            if (type == null)
            {
                Debug.LogError($"未能生成Key={name}对应的类型，请检查Key的前缀命名");
                continue;
            }
            sb.AppendLine($"        {type.Namespace}.{type.Name} {name} {{ get; set; }}");
        }

        if (Objs.Count > 0)
        {
            sb.AppendLine();
        }

        sb.AppendLine($"        void Setup()");
        sb.AppendLine($"        {{");
        for (int i = 0; i < Objs.Count; i++)
        {
            var obj = Objs[i];
            var name = obj.Key;
            var type = GetObjType(name);
            if (type == null)
            {
                continue;
            }
            sb.AppendLine($"            {name} = UGuiView.GetObj<{type.Namespace}.{type.Name}>(\"{name}\");");
        }
        sb.AppendLine($"        }}");

        //sb.AppendLine();

        //sb.AppendLine($"    void UnInitUGUIView()");
        //sb.AppendLine($"    {{");

        //for (int i = 0; i < Objs.Count; i++)
        //{
        //    var obj = Objs[i];
        //    var name = obj.Key;
        //    var type = GetObjType(name);

        //    if (type == typeof(Button))
        //    {
        //        var func = $"On{name}Click";
        //        sb.AppendLine($"        {name}.onClick.RemoveListener({func});");
        //    }
        //}
        //sb.AppendLine($"    }}");

        sb.AppendLine($"    }}");

        sb.AppendLine();

        string disable_audio = EnableOpenAndCloseAudio ? "false" : "true";
        sb.AppendLine(
                $"    [ViewFactory]\n" +
                $"    public class ViewFactory{gameObject.name} : ViewFactory\n" +
                $"    {{\n" +
                $"        public override string GetName()\n" +
                $"        {{\n" +
                $"            return \"View{gameObject.name}\";\n" +
                $"        }}\n" +
                $"    \n" +
                $"        public override bool DisableOpenAndCloseAudio()\n" +
                $"        {{\n" +
                $"            return {disable_audio};\n" +
                $"        }}\n" +
                $"    \n" +
                $"        public override string GetComponentName()\n" +
                $"        {{\n" +
                $"            return \"{gameObject.name}\";\n" +
                $"        }}\n" +
                $"    \n" +
                $"        public override string GetParentName()\n" +
                $"        {{\n" +
                $"            return \"{ViewLayer}\";\n" +
                $"        }}\n" +
                $"    \n" +
                $"        public override View CreateView()\n" +
                $"        {{\n" +
                $"            return new View{gameObject.name}();\n" +
                $"        }}\n" +
                $"    }}");

        sb.AppendLine($"}}");

        sb.AppendLine();

        sb.AppendLine($"#endif");

        //for(int i = 0;i<funcs.Count;i++)
        //{
        //    sb.Append($"private void {funcs[i]}()\n");
        //    sb.Append("{\n\n");
        //    sb.Append("}\n");
        //}
        //Clipboard.Copy(sb.ToString());

        File.WriteAllText(cs_filename_fullpath, sb.ToString());
    }

    void GenViewCs(ClientCfg4Project cfg_project)
    {
        // 获取待生成View代码的目录
        string p = Path.Combine(Environment.CurrentDirectory, cfg_project.GenViewCodeDir);
        p = Path.GetFullPath(p);
        string cs_filename = $"View{gameObject.name}.cs";
        string cs_filename_fullpath = Path.Combine(p, cs_filename);
        cs_filename_fullpath = Path.GetFullPath(cs_filename_fullpath);
        Debug.Log(cs_filename_fullpath);

        if (File.Exists(cs_filename_fullpath))
        {
            // todo
            // 检查View类的一致性，如果不一致，则重新生成View类。所有变量类型相同，所有变量名相同，变量个数相同
            // 检查ViewFactory类，不存在则生成，存在则不执行任何操作

            //var s = File.ReadAllText(cs_filename_fullpath);

            //var parseOptions = new CSharpParseOptions().WithLanguageVersion(LanguageVersion.CSharp11);
            //var compilationOptions = new CSharpCompilationOptions(OutputKind.NetModule);
            ////.WithNullableContextOptions(NullableContextOptions.Enable);

            //var tree = CSharpSyntaxTree.ParseText(cs_filename_fullpath, parseOptions);
            //var root = tree.GetCompilationUnitRoot();
            //var compilation = CSharpCompilation.Create(string.Empty, options: compilationOptions)
            //    //.AddReferences(MetadataReference.CreateFromFile(typeof(string).Assembly.Location))
            //    .AddSyntaxTrees(tree);
            //var semanticModel = compilation.GetSemanticModel(tree);
            //// 检查是否存在语义错误
            //var diagnostics = semanticModel.GetDiagnostics();
            //var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
            //if (errors.Count > 0)
            //{
            //    foreach (var i in errors)
            //    {
            //        Debug.LogError(i.ToString());
            //    }
            //}

            // 创建SyntaxTree
            //var parse_options = new CSharpParseOptions().WithLanguageVersion(LanguageVersion.CSharp11);
            //SyntaxTree tree = CSharpSyntaxTree.ParseText(s, parse_options);

            // 获取根节点
            //CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            //var semanticModel = compilation.GetSemanticModel(syntaxTree);

            //var class_declaration_syntaxes = from node in tree.GetRoot().DescendantNodes()
            //                                 where node.IsKind(SyntaxKind.ClassDeclaration)
            //                                 select (ClassDeclarationSyntax)node;

            //foreach (var class_declaration_syntax in class_declaration_syntaxes)
            //{
            //    //if (semanticModel.GetDeclaredSymbol(class_declaration_syntax) is { } classDeclarationSymbol)
            //    //{
            //    //    // 在这里使用你的类型定义语义符号。

            //    //    int aa = 0;
            //    //}
            //}

            //var root = (CompilationUnitSyntax)tree.GetRoot();

            // 遍历语法树
            //foreach (var node in root.DescendantNodes())
            //{
            //    // 在这里可以对语法树节点进行操作
            //    Debug.Log(node.Kind() + "  " + node.ToFullString());

            //    node.IsKind(SyntaxKind.CompilationUnit);
            //}

            return;
        }
        else
        {
        }

        // 生成View代码

        StringBuilder sb = new();

        sb.AppendLine(
            $"#if DEF_CLIENT\n" +
            $"\n" +
            $"using DEF.Client;\n" +
            $"using System;\n" +
            $"using System.Collections.Generic;\n" +
            $"using System.Threading.Tasks;\n" +
            $"using TMPro;\n" +
            $"using UnityEngine;\n" +
            $"using UnityEngine.UI;\n" +
            $"\n" +
            $"namespace {cfg_project.ClientNameSpace}\n" +
            $"{{");

        sb.AppendLine($"    public partial class View{gameObject.name}");
        sb.AppendLine($"    {{");

        sb.AppendLine($"        public override void OnCreate(object obj, Dictionary<string, string> create_params)");
        sb.AppendLine($"        {{");
        sb.AppendLine($"            Setup();");
        sb.AppendLine($"        }}");
        sb.AppendLine();

        sb.AppendLine($"        public override void OnDestory()");
        sb.AppendLine($"        {{");
        sb.AppendLine($"            UnListenAllEvent();");
        sb.AppendLine($"        }}");
        sb.AppendLine();

        sb.AppendLine($"        public override void OnUGuiButtonClick(string button_name, GameObject go_button)");
        sb.AppendLine($"        {{");
        sb.AppendLine($"        }}");
        sb.AppendLine();

        sb.AppendLine($"        public override void HandleEvent(DEF.Event ev)");
        sb.AppendLine($"        {{");
        sb.AppendLine($"        }}");

        sb.AppendLine($"    }}");
        sb.AppendLine($"}}");

        sb.AppendLine();

        sb.AppendLine($"#endif");

        File.WriteAllText(cs_filename_fullpath, sb.ToString());
    }

    static Dictionary<string, Type> Dic { get; } = new()
    {
        ["Btn"] = typeof(Button),
        ["Button"] = typeof(Button),
        ["Image"] = typeof(Image),
        ["Img"] = typeof(Image),
        ["Slider"] = typeof(Slider),
        ["Progress"] = typeof(Slider),
        ["Scroll"] = typeof(ScrollRect),
        ["Txt"] = typeof(TextMeshProUGUI),
        ["Text"] = typeof(TextMeshProUGUI),
        ["DropDown"] = typeof(TMP_Dropdown),
        ["Input"] = typeof(TMP_InputField),
        ["Toggle"] = typeof(Toggle),

        ["TransRect"] = typeof(RectTransform),
        ["Trans"] = typeof(Transform),
        ["Container"] = typeof(GameObject),
        ["Panel"] = typeof(GameObject),
        ["Tab"] = typeof(GameObject),
    };

    static Type GetObjType(string key)
    {
        foreach (var kv in Dic)
        {
            if (key.StartsWith(kv.Key)) return kv.Value;
        }

        //return typeof(Component);

        return null;
    }
#endif
}

#endif