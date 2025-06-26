using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ReferenceCollector))]
public class ReferenceCollectorEditor : Editor
{
    private string searchKey
    {
        get
        {
            return _searchKey;
        }
        set
        {
            if (_searchKey != value)
            {
                _searchKey = value;
                heroPrefab = referenceCollector.Get<UnityEngine.Object>(searchKey);
            }
        }
    }

    private ReferenceCollector referenceCollector;
    private UnityEngine.Object heroPrefab;
    private string _searchKey = "";

    private void DelNullReference()
    {
        var dataProperty = serializedObject.FindProperty("data");
        for (int i = dataProperty.arraySize - 1; i >= 0; i--)
        {
            var gameObjectProperty = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("gameObject");
            if (gameObjectProperty.objectReferenceValue == null)
            {
                dataProperty.DeleteArrayElementAtIndex(i);
                EditorUtility.SetDirty(referenceCollector);
                serializedObject.ApplyModifiedProperties();
                serializedObject.UpdateIfRequiredOrScript();
            }
        }
    }

    private void OnEnable()
    {
        // 将被选中的gameobject所挂载的ReferenceCollector赋值给编辑器类中的ReferenceCollector，方便操作
        referenceCollector = (ReferenceCollector)target;
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(referenceCollector, "Changed Settings");

        var dataProperty = serializedObject.FindProperty("data");
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("添加引用"))
        {
            AddReference(dataProperty, Guid.NewGuid().GetHashCode().ToString(), null);
        }
        if (GUILayout.Button("全部删除"))
        {
            referenceCollector.Clear();
        }
        if (GUILayout.Button("删除空引用"))
        {
            DelNullReference();
        }
        if (GUILayout.Button("排序"))
        {
            referenceCollector.Sort();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        // 可以在编辑器中对searchKey进行赋值，只要输入对应的Key值，就可以点后面的删除按钮删除相对应的元素
        searchKey = EditorGUILayout.TextField(searchKey);
        // 添加的可以用于选中Object的框，这里的object也是(UnityEngine.Object
        // 第三个参数为是否只能引用scene中的Object
        EditorGUILayout.ObjectField(heroPrefab, typeof(UnityEngine.Object), false);
        if (GUILayout.Button("删除"))
        {
            referenceCollector.Remove(searchKey);
            heroPrefab = null;
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        var delList = new List<int>();
        SerializedProperty property;
        // 遍历ReferenceCollector中data list的所有元素，显示在编辑器中
        for (int i = referenceCollector.data.Count - 1; i >= 0; i--)
        {
            GUILayout.BeginHorizontal();
            property = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("key");
            EditorGUILayout.TextField(property.stringValue, GUILayout.Width(150));
            property = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("gameObject");
            property.objectReferenceValue = EditorGUILayout.ObjectField(property.objectReferenceValue, typeof(UnityEngine.Object), true);
            if (GUILayout.Button("X"))
            {
                delList.Add(i);
            }
            GUILayout.EndHorizontal();
        }
        var eventType = Event.current.type;
        // 在Inspector 窗口上创建区域，向区域拖拽资源对象，获取到拖拽到区域的对象
        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (eventType == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                foreach (var o in DragAndDrop.objectReferences)
                {
                    AddReference(dataProperty, o.name, o);
                }
            }

            Event.current.Use();
        }

        foreach (var i in delList)
        {
            dataProperty.DeleteArrayElementAtIndex(i);
        }
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }

    private void AddReference(SerializedProperty dataProperty, string key, UnityEngine.Object obj)
    {
        int index = dataProperty.arraySize;
        dataProperty.InsertArrayElementAtIndex(index);
        var element = dataProperty.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("key").stringValue = key;
        element.FindPropertyRelative("gameObject").objectReferenceValue = obj;
    }
}