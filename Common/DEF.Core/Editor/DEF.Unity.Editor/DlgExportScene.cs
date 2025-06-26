#if DEF_SCENE

using ProtoBuf;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DEF
{
    [ProtoContract]
    public class ColliderInfo
    {
        [ProtoMember(1)]
        public int size_w;
        [ProtoMember(2)]
        public int size_z;
        [ProtoMember(3)]
        public float angle;
    }

    [ProtoContract]
    public class LevelObjectProperty
    {
        public string Key;
        public string Value;
    }

    [ProtoContract]
    public class CfgLevelObj
    {
        [ProtoMember(1)]
        public int x;
        [ProtoMember(2)]
        public int y;
        [ProtoMember(3)]
        public ColliderInfo colliderInfo;
        [ProtoMember(4)]
        public string name;
        [ProtoMember(5)]
        public string levelObjType;
        [ProtoMember(6)]
        public List<LevelObjectProperty> props;
    }

    // 关卡配置
    [ProtoContract]
    public class CfgLevelEx
    {
        public List<CfgLevelObj> ArrLevelObjs;
    }

    public class DlgExportScene : OdinEditorWindow
    {
        [Title("Export Scene")]

        [OnInspectorGUI] private void Space3() { GUILayout.Space(30); }

        [Button("导出为Json文件", ButtonSizes.Gigantic), GUIColor(1, 0.5f, 0)]
        public void ButtonExport3D2JSON()
        {
            var current_scene = EditorSceneManager.GetActiveScene();
            if (!current_scene.name.StartsWith("Main"))
            {
                Debug.Log($"请打开主场景再进行导出");
                return;
            }

            string p = Path.Combine(Environment.CurrentDirectory, "./Assets/Arts/ExampleAssets/");
            var dir_levels = new DirectoryInfo(p);
            p = Path.Combine(Environment.CurrentDirectory, "./Assets/Arts/LevelData/");
            var dir_export = Path.GetFullPath(p);
            if (!Directory.Exists(dir_export))
            {
                Directory.CreateDirectory(dir_export);
            }
            Debug.Log($"ExportInfo");
            Debug.Log($"DirLevels={dir_levels.FullName}");
            Debug.Log($"DirExport={dir_export}");

            var files1 = dir_levels.GetFiles("*.unity");
            foreach (var f in files1)
            {
                var file_name = Path.GetFileNameWithoutExtension(f.FullName);
                if (!file_name.StartsWith("Level")) continue;

                var scene = EditorSceneManager.OpenScene($"Assets/Arts/ExampleAssets/{file_name}.unity", OpenSceneMode.Additive);

                // 重新计算参考点和门的Order
                GameObject rootLeft = null;
                GameObject rootRight = null;
                var arr_go = scene.GetRootGameObjects();
                foreach (var go in arr_go)
                {
                    if (go.name.Equals("rootLeft"))
                    {
                        rootLeft = go;
                    }
                    else if (go.name.Equals("rootRight"))
                    {
                        rootRight = go;
                    }
                }

                // 开始导出
                CfgLevelEx cfg_level = new()
                {
                    ArrLevelObjs = new(),
                };

                var arr_tile_obj = rootLeft.GetComponentsInChildren<Tilemap3D.SceneObj>();
                if (arr_tile_obj != null)
                {
                    foreach (var i in arr_tile_obj)
                    {
                        if (!i.isActiveAndEnabled)
                            continue;

                        CfgLevelObj levelObj = new CfgLevelObj();
                        levelObj.x = (int)i.transform.position.x;
                        levelObj.y = (int)i.transform.position.z;
                        var collider = i.GetComponent<BoxCollider>();
                        if (collider)
                        {
                            levelObj.colliderInfo = new ColliderInfo();
                            levelObj.colliderInfo.size_w = (int)collider.size.x;
                            levelObj.colliderInfo.size_z = (int)collider.size.z;
                            levelObj.colliderInfo.angle = i.transform.localEulerAngles.y * Mathf.Deg2Rad;
                        }
                        levelObj.levelObjType = i.TileType;
                        levelObj.name = "rootLeft_" + i.gameObject.name;
                        levelObj.props = new List<LevelObjectProperty>();
                        foreach (var prop in i.Props)
                        {
                            LevelObjectProperty levelObjectProperty = new LevelObjectProperty();
                            levelObjectProperty.Key = prop.Key;
                            levelObjectProperty.Value = prop.Value;
                            levelObj.props.Add(levelObjectProperty);
                        }
                        cfg_level.ArrLevelObjs.Add(levelObj);
                    }
                }

                arr_tile_obj = rootRight.GetComponentsInChildren<Tilemap3D.SceneObj>();
                if (arr_tile_obj != null)
                {
                    foreach (var i in arr_tile_obj)
                    {
                        if (!i.isActiveAndEnabled)
                            continue;
                        CfgLevelObj levelObj = new();
                        levelObj.x = (int)i.transform.position.x;
                        levelObj.y = (int)i.transform.position.z;
                        var collider = i.GetComponent<BoxCollider>();
                        if (collider)
                        {
                            levelObj.colliderInfo = new ColliderInfo();
                            levelObj.colliderInfo.size_w = (int)collider.size.x;
                            levelObj.colliderInfo.size_z = (int)collider.size.z;
                        }
                        levelObj.levelObjType = i.TileType;
                        levelObj.name = "rootRight_" + i.gameObject.name;
                        levelObj.props = new List<LevelObjectProperty>();
                        foreach (var prop in i.Props)
                        {
                            LevelObjectProperty levelObjectProperty = new LevelObjectProperty();
                            levelObjectProperty.Key = prop.Key;
                            levelObjectProperty.Value = prop.Value;
                            levelObj.props.Add(levelObjectProperty);
                        }
                        cfg_level.ArrLevelObjs.Add(levelObj);
                    }
                }

                var json_str = Newtonsoft.Json.JsonConvert.SerializeObject(cfg_level, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText($"{dir_export}{file_name}.json", json_str);
            }
        }
    }
}

#endif