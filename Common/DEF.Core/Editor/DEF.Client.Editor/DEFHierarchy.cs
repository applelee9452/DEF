using DEF;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class DEFPropScene
{
    public string SceneName;
    public string ContainerType;
    public string ContainerId;
}

public class DEFPropEntity
{
    public long EntityId;
    public string EntityName;
    //[ShowInInspector]
    [ListDrawerSettings(IsReadOnly = true, HideAddButton = true, HideRemoveButton = true, DefaultExpandedState = true, ShowIndexLabels = false)]
    public List<DEFPropComponent> Components;
}

[Serializable]
public class DEFPropComponent
{
    //[ShowInInspector]
    [SerializeField]
    public string Name;

    [HideInInspector]
    [OnInspectorGUI("DrawState", append: false)]
    public IComponentState State;

    bool show = true;

    Dictionary<object, bool> CustomTypeFoldout = new();

    void DrawState()
    {
        if (State != null)
        {
            Type type = State.GetType();

            show = EditorGUILayout.Foldout(show, type.Name);

            if (show)
            {
                //EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUI.indentLevel++;

                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => !p.PropertyType.Name.StartsWith("OnPropChanged"));

                foreach (var property in properties)
                {
                    int depth = 0;

                    if (property.PropertyType == typeof(int) || property.PropertyType == typeof(float) || property.PropertyType == typeof(double)
                        || property.PropertyType == typeof(bool) || property.PropertyType == typeof(string))
                    {
                        DrawBuiltin(State, property);
                    }
                    else if (property.PropertyType.GetInterface(nameof(System.Collections.ICollection)) != null)
                    {


                    }
                    else if (!property.PropertyType.Name.Equals("Component"))
                    {
                        object value = property.GetValue(State);
                        //Debug.LogError(value.GetType().Name + " : " + property.PropertyType.Name);
                        DrawCustom(value, property, depth);
                    }
                }

                EditorGUI.indentLevel--;

                //EditorGUILayout.EndVertical();
            }
            else
            {
                //show = EditorGUILayout.Foldout(show, type.Name);
            }
        }
    }

    void DrawBuiltin(object obj, PropertyInfo propertyInfo)
    {
        if (obj == null)
        {
            return;
        }

        object value = propertyInfo.GetValue(obj);
        if (value != null)
        {
            EditorGUILayout.TextField(propertyInfo.Name, value.ToString());
        }
    }

    void DrawCustom(object obj, PropertyInfo parentProperty, int depth)
    {
        if (obj == null || depth > 10)
        {
            return;
        }
        depth++;

        if (CustomTypeFoldout.TryGetValue(obj, out bool show))
        {
            CustomTypeFoldout[obj] = EditorGUILayout.Foldout(show, parentProperty.Name);
        }
        else
        {
            CustomTypeFoldout.Add(obj, false);
            CustomTypeFoldout[obj] = EditorGUILayout.Foldout(false, parentProperty.Name);
        }

        if (CustomTypeFoldout[obj])
        {
            EditorGUI.indentLevel++;

            var properties = parentProperty.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !p.PropertyType.Name.StartsWith("OnPropChanged"));

            foreach (var property in properties)
            {
                object value = property.GetValue(obj);
                if (value != null)
                {
                    if (property.PropertyType == typeof(int) || property.PropertyType == typeof(float) || property.PropertyType == typeof(double)
                        || property.PropertyType == typeof(bool) || property.PropertyType == typeof(string))
                    {
                        DrawBuiltin(obj, property);
                    }
                    else if (property.PropertyType.GetInterface(nameof(System.Collections.ICollection)) != null)
                    {


                    }
                    else if (!property.PropertyType.Name.Equals("Component"))
                    {
                        if (parentProperty.Name.Equals("Date") && property.Name.Equals("Date"))
                            continue;

                        //Debug.LogError(value.GetType().Name + " : " + property.PropertyType.Name);
                        DrawCustom(value, property, depth);
                        
                    }
                }
            }

            EditorGUI.indentLevel--;
        }
    }
}

// todo，处理运行和停止消息
public class DEFHierarchy : OdinMenuEditorWindow
{
    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree
        {
            DefaultMenuStyle = OdinMenuStyle.TreeViewStyle
        };

        var c = ClientUnity.Instance;
        if (c == null || c.MapScene == null || c.MapScene.Count == 0) return tree;

        foreach (var item in c.MapScene)
        {
            var scene = item.Value;
            var scene_editor = new DEFPropScene()
            {
                SceneName = scene.Name,
                ContainerType = scene.ContainerType,
                ContainerId = scene.ContainerId
            };
            tree.Add(scene.Name, scene_editor, SdfIconType.Folder);

            var entities = scene.RootEntity.GetChildrenRef();
            if (entities != null && entities.Count > 0)
            {
                foreach (var entity in entities)
                {
                    AddEntity(scene.Name, tree, entity);
                }
            }
        }

        tree.Selection.SelectionChanged += (v) =>
        {
            var o = tree.Selection.SelectedValue;
            if (o != null)
            {
                if (o is DEFPropScene)
                {

                }
                else if (o is DEFPropEntity)
                {
                    //DEFPropEntity e = (DEFPropEntity)o;
                }
            }
        };

        return tree;
    }

    void AddEntity(string path, OdinMenuTree tree, Entity entity)
    {
        // 添加自身

        List<DEFPropComponent> list_com = null;
        if (entity.ListComponent != null && entity.ListComponent.Count > 0)
        {
            list_com = new();

            foreach (var component in entity.ListComponent)
            {
                DEFPropComponent com = new()
                {
                    Name = component.Name,
                    State = component.GetState(),
                };

                //AddProperties(com);
                list_com.Add(com);
            }
        }

        var e = new DEFPropEntity
        {
            EntityId = entity.Id,
            EntityName = entity.Name,
            Components = list_com
        };

        string s = "-";
        string s2 = string.IsNullOrEmpty(entity.Name) ? s : entity.Name;
        string path_entity = $"{path}/{s2}";
        tree.Add(path_entity, e, SdfIconType.None);

        // 添加子Entity列表

        var children = entity.GetChildrenRef();
        if (children != null && children.Count > 0)
        {
            foreach (var child in children)
            {
                AddEntity(path_entity, tree, child);
            }
        }
    }

}