//using Sirenix.OdinInspector;
//using Sirenix.OdinInspector.Editor;
//using Sirenix.OdinInspector.Editor.Examples;
//using Sirenix.Utilities.Editor;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEditor;
//using UnityEngine;

//public class DlgDEFSettings2 : OdinEditorWindow
//{
//    [OnInspectorInit]
//    void Init()
//    {
//    }

//    [OnInspectorDispose]
//    void Destroy()
//    {
//    }

//    [Title("aaaaaaaaaaaa")]

//    [LabelText("1")]
//    public int MyInt1 = 1;

//    [LabelText("2")]
//    [LabelWidth(50)]
//    public int MyInt2 = 12;

//    [LabelText("3")]
//    [LabelWidth(100)]
//    public int MyInt3 = 123;

//    [Title("cccccccccccccc")]

//    [InfoBox("Use $ to refer to a member string.")]
//    [LabelText("$MyInt3")]
//    public string LabelText = "The label is taken from the number 3 above";

//    [InfoBox("Use @ to execute an expression.")]
//    [LabelText("@DateTime.Now.ToString(\"HH:mm:ss\")")]
//    public string DateTimeLabel;

//    [Title("aaaa")]
//    public string Hello1;

//    [InfoBox("The AssetList attribute work on both lists of UnityEngine.Object types and UnityEngine.Object types, but have different behaviour.")]
//    public string Hello;

//    [EnumToggleButtons, BoxGroup("Settings")]
//    public ScaleMode ScaleMode;

//    [FolderPath(RequireExistingPath = true), BoxGroup("Settings")]
//    public string OutputPath;

//    [PropertyOrder(4)]
//    [HorizontalGroup(0.5f)]
//    public List<Texture> InputTextures;

//    [HorizontalGroup, InlineEditor(InlineEditorModes.LargePreview)]
//    public Texture Preview;

//    [Button(ButtonSizes.Gigantic), GUIColor(0, 1, 0)]
//    public void PerformSomeAction()
//    {
//        Debug.Log("PerformSomeAction ScaleMode" + ScaleMode);
//    }

//    [CustomValueDrawer("MyCustomDrawerStatic")]
//    public float CustomDrawerStatic;
//    private static float MyCustomDrawerStatic(float value, GUIContent label)
//    {
//        SirenixEditorGUI.BeginBox();

//        var result = EditorGUILayout.Slider(label, value, 0f, 10f);
//        SirenixEditorGUI.EndBox();

//        return result;
//    }

//    [HorizontalGroup("AAA")]
//    [Button(ButtonSizes.Large)]
//    public void SomeButton1()
//    {
//    }

//    [HorizontalGroup("AAA")]
//    [Button(ButtonSizes.Large)]
//    public void SomeButton2()
//    {
//    }

//    [HorizontalGroup("AAA")]
//    [Button(ButtonSizes.Large)]
//    public void SomeButton3()
//    {
//    }

//    [OnInspectorGUI] private void Space3() { GUILayout.Space(30); }

//    [HorizontalGroup("BBB")]
//    [Button("关闭", ButtonSizes.Gigantic), GUIColor(1, 0.5f, 0)]
//    public void ButtonClose()
//    {
//        Close();
//    }

//    //[Title("项目配置")]
//    //[OnInspectorGUI] private void Space1() { GUILayout.Space(50); }
//}
