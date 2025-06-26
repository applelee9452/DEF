#if DEF_TILEMAP3D

using ProtoBuf;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using Tilemap3D;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DEF
{
    // 矿石类型
    public enum OreType
    {
        None = 0,
        Default,
        Gem,
        Potion,
        Gold,// 暂未使用
        Key,
        Medal,
        Chest,
    }

    // 宝箱类型
    public enum ChestType
    {
        None = 0,
        Wood,
        Iron,
        Brass,
        Silver,
        Gold,
        Mys,
        Royal,
        Epic,
        Legendary,
    }

    // 卡牌类型
    public enum CardType
    {
        None = 0,
        AddForgeProfit,//提升锻造炉利润
        AddPitProfit,//提升矿井利润
        AddOreDropGoldRate,//提升矿石掉落金币的比例
        ReduceFreeWorkerTm,//缩短生成免费矿工的时间
        IncreaseForgeAndPitSpeed,//提升锻造炉&矿井的生成速度
        BuyWorkerDiscountWhenOpenDoor,//每次激活关卡后，矿工购买费用缩小倍数
        AddWorkerCritHitRate,//提升矿工暴击几率
        AddBarrelDropGoldRate,//提升随机桶掉落金币的比例
        ReduceFreeWorkerTmWhenOpenDoor,//每次激活关卡后，缩短生成免费矿工的时间
        AddForgeAndPitProfit,//提升锻造炉&矿井利润
        AddMaxWorkerNum,//增加矿工数量上限
        AddForgeAndPitProfitWhenOpenDoor,//每次激活关卡后，提升锻造炉&矿井利润
        AddWorkerCritHurtRate,//提升矿工暴击伤害
        AddWorkerLv,//增加矿工等级
        AddCardPointWhenOpenChest,//增加开宝箱获取的卡牌数量
        AddEventReward,//增加活动结束时获得的奖励
    }

    public class LevelRecord
    {
        public string LevelId { get; set; }
        public string UnitType { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Lv { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        public string Param4 { get; set; }

        public string CardType1 { get; set; }
        public int Value1 { get; set; }
        public string CardType2 { get; set; }
        public int Value2 { get; set; }
        public string CardType3 { get; set; }
        public int Value3 { get; set; }
        public string CardType4 { get; set; }
        public int Value4 { get; set; }
    }

    // 点
    [ProtoContract]
    public class CfgPoint
    {
        [ProtoMember(1)]
        public int x;
        [ProtoMember(2)]
        public int y;
    }

    // 参考点（done）
    [ProtoContract]
    public class CfgPointSpawnForgeAndAirdrop
    {
        [ProtoMember(1)]
        public CfgPoint Pos;
        [ProtoMember(2)]
        public int Order;
    }

    // 门的被动技能
    [ProtoContract]
    public class CfgDoorEffect
    {
        [ProtoMember(1)]
        public CardType CardType;
        [ProtoMember(2)]
        public int Value;
    }

    // 门（done）
    [ProtoContract]
    public class CfgDoor
    {
        [ProtoMember(1)]
        public int Lv;
        [ProtoMember(2)]
        public CfgPoint Pos;
        [ProtoMember(3)]
        public int Order;
        [ProtoMember(4)]
        public string UnlockCards;// 解锁卡牌列表
        [ProtoMember(5)]
        public CfgDoorEffect[] ArrEffect;// 被动技能列表
    }
    public enum CardColor
    {
        Common = 0,// 普通，只用于普通关卡&活动关卡矿井类卡牌
        Uncommon,// 非凡，只用于普通关卡矿井类卡牌
        Rare,// 稀有
        Epic,// 史诗
        Legendary,// 传奇
    }
    // 矿石（done）
    [ProtoContract]
    public class CfgOre
    {
        [ProtoMember(1)]
        public CfgPoint Pos;
        [ProtoMember(2)]
        public int Lv;// 等级
        [ProtoMember(3)]
        public OreType OreType;// 矿石类型
        [ProtoMember(4)]
        public ChestType ChestType;// 宝箱类型
        [ProtoMember(5)]
        public string VFX;// 矿石上绑定的特效
    }

    // 矿井（done）
    [ProtoContract]
    public class CfgPit
    {
        [ProtoMember(1)]
        public CfgPoint Pos;
        [ProtoMember(2)]
        public CfgPoint[] ArrGrid;
        [ProtoMember(3)]
        public int OreLv;// 未挖开时的矿石等级
        [ProtoMember(4)]
        public int StoneId;// 自动化所需的卡牌Id
        [ProtoMember(5)]
        public string CardAutoColor;// 自动化所需的卡牌Id
        [ProtoMember(6)]
        public int CardAutoLv;// 自动化所需的卡牌等级
        [ProtoMember(7)]
        public string EffectCards;// 所有关联卡牌，逗号分隔
        [ProtoMember(8)]
        public string UnlockCards;// 解锁卡牌列表，逗号分隔
    }

    // 关卡配置
    // 锻造炉和空投炮紧挨一起，会动态改变位置
    // 如果一个物体占多格，XY表示该物体左下角的1格
    [ProtoContract]
    public class CfgLevel
    {
        [ProtoMember(1)]
        public int Lv;
        [ProtoMember(2)]
        public int Height;// 高，格子数
        [ProtoMember(3)]
        public int HeightOffset;// 向上偏移的格子数
        [ProtoMember(4)]
        public int MaxWorkerNum;// 矿工人口上限
        [ProtoMember(5)]
        public List<CfgPoint> ArrObstacle;// 关卡中所有的障碍物
        [ProtoMember(6)]
        public List<CfgOre> ArrOre;// 关卡中所有的矿石
        [ProtoMember(7)]
        public List<CfgDoor> ArrDoor;// 关卡中所有的门
        [ProtoMember(8)]
        public List<CfgPit> ArrPit;// 关卡中所有的矿井
        [ProtoMember(9)]
        public List<CfgPointSpawnForgeAndAirdrop> ArrSpawnPoint;// 关卡中所有的出生点
    }

    public class DlgExportTilemaps : OdinEditorWindow
    {
        [Title("Export Tilemaps")]

        [OnInspectorGUI] private void Space3() { GUILayout.Space(30); }

        [Button("检查当前关卡场景", ButtonSizes.Gigantic), GUIColor(1, 0.5f, 0)]
        public void ButtonCheckLevelCurrent()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (scene.name.StartsWith("Main"))
            {
                Debug.LogError($"请打开关卡场景");
                return;
            }

            var file_name = scene.name;

            Tilemap3D.Tilemap tile_map = null;
            var arr_go = scene.GetRootGameObjects();
            foreach (var go in arr_go)
            {
                var c = go.GetComponent<Tilemap3D.Tilemap>();
                if (c != null)
                {
                    tile_map = c;
                    break;
                }
            }

            if (tile_map == null)
            {
                Debug.LogError($"缺少Tilemap组件");
                return;
            }

            int y_min, y_max;
            bool oddor_even = TileUtils.OddorEven(tile_map.TilemapHeight);
            if (oddor_even)
            {
                y_min = -(tile_map.TilemapHeight - 1) / 2;
                y_max = (tile_map.TilemapHeight - 1) / 2;
            }
            else
            {
                y_min = -tile_map.TilemapHeight / 2;
                y_max = (tile_map.TilemapHeight - 2) / 2;
            }
            y_min += tile_map.TilemapHeightOffset;
            y_max += tile_map.TilemapHeightOffset;

            string level_id_s = file_name.Replace("Level", "");
            int.TryParse(level_id_s, out var level_id);

            var arr_tile = tile_map.GetComponentsInChildren<Tilemap3D.Tile>();

            if (arr_tile.Length == 0)
            {
                Debug.LogError($"{file_name}中Tile数量==0！");
                return;
            }

            foreach (var i in arr_tile)
            {
                var tile = i;

                var tile_layer = tile.transform.parent.GetComponent<TileLayer>();

                if (tile_layer != tile.Layer)
                {
                    Debug.LogError($"{file_name}中Tile有错误，Name={tile.name}，{tile.GridCellPosition.ToString()}！");
                }

                var tile2 = tile_map.GetTile(tile.GridCellPosition, tile.Layer);
                if (tile2 == null)
                {
                    Debug.LogError($"{file_name}中Tile有错误，Name={tile.name}，{tile.GridCellPosition.ToString()}！");
                }
            }

            var tile_airdrop = tile_map.transform.Find("Airdrop/AirdropAndForge");
            if (tile_airdrop == null)
            {
                Debug.LogError($"{file_name}中Tile有错误，Airdrop层下面需要刷AirdropAndForge 1！");
                return;
            }

            var tile_airdrop2 = tile_airdrop.GetComponent<Tile>();
            if (tile_airdrop2 == null)
            {
                Debug.LogError($"{file_name}中Tile有错误，Airdrop层下面需要刷AirdropAndForge 2！");
                Debug.LogError($"{file_name}中Tile有错误，Airdrop层下面需要刷AirdropAndForge！");
            }

            if (tile_airdrop2.Layer.name != "Airdrop")
            {
                Debug.LogError($"{file_name}中Tile有错误，Airdrop层下面需要刷AirdropAndForge 4！");
                Debug.LogError($"{file_name}中Tile有错误，Airdrop层下面需要刷AirdropAndForge！");
            }

            foreach (var i in tile_map)
            {
                var tile = i.Value;
                var tile_obj = tile.GetComponentInChildren<Tilemap3D.TileObj>();

                switch (tile_obj.TileType)
                {
                    case "Obstacle":
                        {
                            var pos_cell = tile.GridCellPosition;
                            var grid_x = pos_cell.x;
                            var grid_y = pos_cell.z;

                            if (grid_y < y_min || grid_y > y_max)
                            {
                                Debug.LogError($"Name={tile.name}，障碍物超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
                                continue;
                            }
                        }
                        break;
                    case "Ore":
                        {
                            var pos_cell = tile.GridCellPosition;
                            var grid_x = pos_cell.x;
                            var grid_y = pos_cell.z;

                            if (grid_y < y_min || grid_y > y_max)
                            {
                                Debug.LogError($"Name={tile.name}，矿石超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
                                return;
                            }
                        }
                        break;
                    case "Door":
                        {
                            // 格子数，5x1，原点位于格子正中间

                            var pos_cell = tile.GridCellPosition;
                            var grid_x = pos_cell.x;
                            var grid_y = pos_cell.z;

                            if (grid_y < y_min || grid_y > y_max)
                            {
                                Debug.LogError($"Name={tile.name}，小门超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
                                continue;
                            }
                        }
                        break;
                    case "DoorEnd":
                        {
                            var pos_cell = tile.GridCellPosition;
                            var grid_x = pos_cell.x;
                            var grid_y = pos_cell.z;

                            if (grid_y < y_min || grid_y > y_max)
                            {
                                Debug.LogError($"Name={tile.name}，大门超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
                                continue;
                            }
                        }
                        break;
                    case "Pit":
                        {
                            // 格子数，3x3，原点位于格子正中间

                            var pos_cell = tile.GridCellPosition;
                            var grid_x = pos_cell.x;
                            var grid_y = pos_cell.z;

                            if (grid_y < y_min || grid_y > y_max)
                            {
                                Debug.LogError($"Name={tile.name}，矿井超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
                                continue;
                            }
                        }
                        break;
                }
            }

            Debug.Log($"检查完毕！");

            Close();
        }

        [Button("校正当前关卡场景", ButtonSizes.Gigantic), GUIColor(1, 0.5f, 0)]
        public void ButtonAdjustTileGrid2TransformLevelCurrent()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (scene.name.StartsWith("Main"))
            {
                Debug.LogError($"请打开关卡场景");
                return;
            }

            var file_name = scene.name;


            Tilemap3D.Tilemap tile_map = null;
            var arr_go = scene.GetRootGameObjects();
            foreach (var go in arr_go)
            {
                var c = go.GetComponent<Tilemap3D.Tilemap>();
                if (c != null)
                {
                    tile_map = c;
                    break;
                }
            }

            if (tile_map == null)
            {
                Debug.LogError($"{file_name}上缺少Tilemap组件");
                return;
            }

            var go_floor = GameObject.Find("Floor/Floor");
            if (go_floor != null)
            {
                go_floor.transform.localScale = new Vector3(100, 1, 100);
                EditorUtility.SetDirty(go_floor);
            }
            else
            {
                Debug.LogError($"{file_name}上没找到地板");
            }

            tile_map.AdjustTileGrid2Transform((tile) => { EditorUtility.SetDirty(tile); });

            EditorUtility.SetDirty(tile_map);

            Close();
        }

        [Button("检查所有关卡场景", ButtonSizes.Gigantic), GUIColor(1, 0.5f, 0)]
        public void ButtonCheckLevels()
        {
            string p = Path.Combine(Environment.CurrentDirectory, "./Assets/Arts/LevelScenes/");
            var dir_levels = new DirectoryInfo(p);
            Debug.Log($"DirLevels={dir_levels.FullName}");

            // 遍历所有关卡
            var files1 = dir_levels.GetFiles("*.unity");
            foreach (var f in files1)
            {
                var file_name = Path.GetFileNameWithoutExtension(f.FullName);
                if (!file_name.StartsWith("Level")) continue;

                Debug.Log(file_name);

                string full_file_name = $"Assets/Arts/LevelScenes/{file_name}.unity";
                var scene = EditorSceneManager.OpenScene(full_file_name, OpenSceneMode.Single);

                Tilemap3D.Tilemap tile_map = null;
                var arr_go = scene.GetRootGameObjects();
                foreach (var go in arr_go)
                {
                    var c = go.GetComponent<Tilemap3D.Tilemap>();
                    if (c != null)
                    {
                        tile_map = c;
                        break;
                    }
                }

                if (tile_map == null)
                {
                    Debug.LogError($"{file_name}上缺少Tilemap组件");
                    continue;
                }

                int y_min, y_max;
                bool oddor_even = TileUtils.OddorEven(tile_map.TilemapHeight);
                if (oddor_even)
                {
                    y_min = -(tile_map.TilemapHeight - 1) / 2;
                    y_max = (tile_map.TilemapHeight - 1) / 2;
                }
                else
                {
                    y_min = -tile_map.TilemapHeight / 2;
                    y_max = (tile_map.TilemapHeight - 2) / 2;
                }
                y_min += tile_map.TilemapHeightOffset;
                y_max += tile_map.TilemapHeightOffset;

                string level_id_s = file_name.Replace("Level", "");
                int.TryParse(level_id_s, out var level_id);

                var arr_tile = tile_map.GetComponentsInChildren<Tilemap3D.Tile>();

                if (arr_tile.Length == 0)
                {
                    Debug.LogError($"{file_name}中Tile数量==0！");
                    continue;
                }

                foreach (var i in arr_tile)
                {
                    var tile = i;

                    var tile_layer = tile.transform.parent.GetComponent<TileLayer>();

                    if (tile_layer != tile.Layer)
                    {
                        Debug.LogError($"{file_name}中Tile有错误，{tile.GridCellPosition.ToString()}！");
                    }

                    var tile2 = tile_map.GetTile(tile.GridCellPosition, tile.Layer);
                    if (tile2 == null)
                    {
                        Debug.LogError($"{file_name}中Tile有错误，{tile.GridCellPosition.ToString()}！");
                    }
                }

                var tile_airdrop = tile_map.transform.Find("Airdrop/AirdropAndForge");
                if (tile_airdrop == null)
                {
                    Debug.LogError($"{file_name}中Tile有错误，Airdrop层下面需要刷AirdropAndForge 1！");
                    continue;
                }

                var tile_airdrop2 = tile_airdrop.GetComponent<Tile>();
                if (tile_airdrop2 == null)
                {
                    Debug.LogError($"{file_name}中Tile有错误，Airdrop层下面需要刷AirdropAndForge 2！");
                    Debug.LogError($"{file_name}中Tile有错误，Airdrop层下面需要刷AirdropAndForge！");
                }

                if (tile_airdrop2.Layer.name != "Airdrop")
                {
                    Debug.LogError($"{file_name}中Tile有错误，Airdrop层下面需要刷AirdropAndForge 4！");
                    Debug.LogError($"{file_name}中Tile有错误，Airdrop层下面需要刷AirdropAndForge！");
                }

                foreach (var i in tile_map)
                {
                    var tile = i.Value;
                    var tile_obj = tile.GetComponentInChildren<Tilemap3D.TileObj>();

                    switch (tile_obj.TileType)
                    {
                        case "Obstacle":
                            {
                                var pos_cell = tile.GridCellPosition;
                                var grid_x = pos_cell.x;
                                var grid_y = pos_cell.z;

                                if (grid_y < y_min || grid_y > y_max)
                                {
                                    Debug.LogError($"障碍物超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
                                    continue;
                                }
                            }
                            break;
                        case "Ore":
                            {
                                var pos_cell = tile.GridCellPosition;
                                var grid_x = pos_cell.x;
                                var grid_y = pos_cell.z;

                                if (grid_y < y_min || grid_y > y_max)
                                {
                                    Debug.LogError($"矿石超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
                                    continue;
                                }

                                //LevelRecord r = null;
                                //foreach (var kk in list_record)
                                //{
                                //    if (grid_x == kk.PosX && grid_y == kk.PosY && kk.UnitType == "Ore")
                                //    {
                                //        r = kk;
                                //        break;
                                //    }
                                //}

                                //if (r != null)
                                //{
                                //    tile_obj.SetPropertyAsInt("Lv", r.Lv);

                                //    EditorUtility.SetDirty(tile_obj);
                                //}
                            }
                            break;
                        case "Door":
                            {
                                // 格子数，5x1，原点位于格子正中间

                                var pos_cell = tile.GridCellPosition;
                                var grid_x = pos_cell.x;
                                var grid_y = pos_cell.z;

                                if (grid_y < y_min || grid_y > y_max)
                                {
                                    Debug.LogError($"小门超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
                                    continue;
                                }

                                //LevelRecord r = null;
                                //foreach (var kk in list_record)
                                //{
                                //    if (grid_x == kk.PosX && grid_y == kk.PosY && kk.UnitType == "Door")
                                //    {
                                //        r = kk;
                                //        break;
                                //    }
                                //}

                                //if (r != null)
                                //{
                                //    tile_obj.SetPropertyAsInt("Lv", r.Lv);
                                //    tile_obj.SetPropertyAsString("Order", r.Param1);
                                //    tile_obj.SetPropertyAsString("UnlockCards", r.Param3);

                                //    tile_obj.SetPropertyAsString("DoorEffect1", r.CardType1);
                                //    tile_obj.SetPropertyAsInt("Operator1", r.Operator1);
                                //    tile_obj.SetPropertyAsInt("PercentOrNumber1", r.PercentOrNumber1);
                                //    tile_obj.SetPropertyAsInt("Value1", r.Value1);

                                //    tile_obj.SetPropertyAsString("DoorEffect2", r.CardType2);
                                //    tile_obj.SetPropertyAsInt("Operator2", r.Operator2);
                                //    tile_obj.SetPropertyAsInt("PercentOrNumber21", r.PercentOrNumber2);
                                //    tile_obj.SetPropertyAsInt("Value2", r.Value2);

                                //    tile_obj.SetPropertyAsString("DoorEffect3", r.CardType3);
                                //    tile_obj.SetPropertyAsInt("Operator3", r.Operator3);
                                //    tile_obj.SetPropertyAsInt("PercentOrNumber3", r.PercentOrNumber3);
                                //    tile_obj.SetPropertyAsInt("Value3", r.Value3);

                                //    tile_obj.SetPropertyAsString("DoorEffect4", r.CardType4);
                                //    tile_obj.SetPropertyAsInt("Operator4", r.Operator4);
                                //    tile_obj.SetPropertyAsInt("PercentOrNumber4", r.PercentOrNumber4);
                                //    tile_obj.SetPropertyAsInt("Value4", r.Value4);

                                //    EditorUtility.SetDirty(tile_obj);
                                //}
                            }
                            break;
                        case "DoorEnd":
                            {
                                var pos_cell = tile.GridCellPosition;
                                var grid_x = pos_cell.x;
                                var grid_y = pos_cell.z;

                                if (grid_y < y_min || grid_y > y_max)
                                {
                                    Debug.LogError($"大门超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
                                    continue;
                                }

                                //LevelRecord r = null;
                                //foreach (var kk in list_record)
                                //{
                                //    if (grid_x == kk.PosX && grid_y == kk.PosY && kk.UnitType == "DoorEnd")
                                //    {
                                //        r = kk;
                                //        break;
                                //    }
                                //}

                                //if (r != null)
                                //{
                                //    tile_obj.SetPropertyAsInt("Lv", r.Lv);
                                //    tile_obj.SetPropertyAsString("Order", r.Param1);
                                //    tile_obj.SetPropertyAsString("UnlockCards", r.Param3);

                                //    EditorUtility.SetDirty(tile_obj);
                                //}
                            }
                            break;
                        case "Pit":
                            {
                                // 格子数，3x3，原点位于格子正中间

                                var pos_cell = tile.GridCellPosition;
                                var grid_x = pos_cell.x;
                                var grid_y = pos_cell.z;

                                if (grid_y < y_min || grid_y > y_max)
                                {
                                    Debug.LogError($"矿井超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
                                    continue;
                                }

                                //LevelRecord r = null;
                                //foreach (var kk in list_record)
                                //{
                                //    if (grid_x == kk.PosX && grid_y == kk.PosY && kk.UnitType == "Pit")
                                //    {
                                //        r = kk;
                                //        break;
                                //    }
                                //}

                                //if (r != null)
                                //{
                                //    tile_obj.SetPropertyAsInt("OreLv", r.Lv);
                                //    tile_obj.SetPropertyAsString("CardId", r.Param1);
                                //    tile_obj.SetPropertyAsString("CardAutoLv", r.Param2);
                                //    tile_obj.SetPropertyAsString("UnlockCards", r.Param3);

                                //    EditorUtility.SetDirty(tile_obj);
                                //}
                            }
                            break;
                    }
                }
            }

            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);

            Debug.Log($"检查完毕！");

            Close();
        }

        [Button("校正所有关卡场景", ButtonSizes.Gigantic), GUIColor(1, 0.5f, 0)]
        public void ButtonAdjustTileGrid2Transform()
        {
            // 遍历所有关卡
            string p = Path.Combine(Environment.CurrentDirectory, "./Assets/Arts/LevelScenes/");
            var dir_levels = new DirectoryInfo(p);
            var files1 = dir_levels.GetFiles("*.unity");
            foreach (var f in files1)
            {
                var file_name = Path.GetFileNameWithoutExtension(f.FullName);
                if (!file_name.StartsWith("Level")) continue;

                //if (file_name != "Level001") continue;

                Debug.Log(file_name);

                string full_file_name = $"Assets/Arts/LevelScenes/{file_name}.unity";
                var scene = EditorSceneManager.OpenScene(full_file_name, OpenSceneMode.Single);

                Tilemap3D.Tilemap tile_map = null;
                var arr_go = scene.GetRootGameObjects();
                foreach (var go in arr_go)
                {
                    var c = go.GetComponent<Tilemap3D.Tilemap>();
                    if (c != null)
                    {
                        tile_map = c;
                        break;
                    }
                }

                if (tile_map == null)
                {
                    Debug.LogError($"{file_name}上缺少Tilemap组件");
                    continue;
                }

                var go_floor = GameObject.Find("Floor/Floor");
                if (go_floor != null)
                {
                    go_floor.transform.localScale = new Vector3(100, 1, 100);
                    EditorUtility.SetDirty(go_floor);
                }
                else
                {
                    Debug.LogError($"{file_name}上没找到地板");
                }

                tile_map.AdjustTileGrid2Transform((tile) => { EditorUtility.SetDirty(tile); });

                EditorUtility.SetDirty(tile_map);

                EditorSceneManager.SaveScene(scene, full_file_name, false);
            }

            EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);

            Close();
        }

        [Button("所有关卡导出为多个Json文件", ButtonSizes.Gigantic), GUIColor(1, 0.5f, 0)]
        public void ButtonExport3DAll2JSON()
        {
            var current_scene = EditorSceneManager.GetActiveScene();
            if (!current_scene.name.StartsWith("Main"))
            {
                Debug.Log($"请打开主场景再进行导出");
                return;
            }

            string p = Path.Combine(Environment.CurrentDirectory, "./Assets/Arts/LevelScenes/");
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

                Debug.Log(file_name);

                var scene = EditorSceneManager.OpenScene($"Assets/Arts/LevelScenes/{file_name}.unity", OpenSceneMode.Additive);

                Tilemap3D.Tilemap tile_map = null;
                var arr_go = scene.GetRootGameObjects();
                foreach (var go in arr_go)
                {
                    var c = go.GetComponent<Tilemap3D.Tilemap>();
                    if (c != null)
                    {
                        tile_map = c;
                        break;
                    }
                }

                if (tile_map == null)
                {
                    Debug.LogError($"{file_name}上缺少Tilemap组件");
                    EditorSceneManager.CloseScene(scene, true);
                    continue;
                }

                // 重新计算参考点和门的Order
                List<TileObj> list_pointspawn = new();
                List<TileObj> list_door = new();

                var arr_tile_obj = tile_map.GetComponentsInChildren<TileObj>();
                if (arr_tile_obj != null)
                {
                    foreach (var i in arr_tile_obj)
                    {
                        if (i.TileType == "Door")
                        {
                            list_door.Add(i);
                        }
                        else if (i.TileType == "DoorEnd")
                        {
                            list_door.Add(i);
                        }
                        else if (i.TileType == "PointSpawn")
                        {
                            list_pointspawn.Add(i);
                        }
                    }
                }

                list_pointspawn.Sort((a, b) =>
                {
                    var a1 = a.gameObject.GetComponentInParent<Tile>();
                    var b1 = b.gameObject.GetComponentInParent<Tile>();

                    return a1.GridCellPosition.z.CompareTo(b1.GridCellPosition.z);
                });

                int index = 0;
                foreach (var i in list_pointspawn)
                {
                    i.SetProperty("Order", index.ToString());
                    index++;
                }

                list_door.Sort((a, b) =>
                {
                    var a1 = a.gameObject.GetComponentInParent<Tile>();
                    var b1 = b.gameObject.GetComponentInParent<Tile>();

                    return a1.GridCellPosition.z.CompareTo(b1.GridCellPosition.z);
                });

                index = 0;
                foreach (var i in list_door)
                {
                    i.SetProperty("Order", index.ToString());
                    index++;
                }

                int.TryParse(tile_map.UserData1, out var max_workernum);
                if (max_workernum <= 0)
                {
                    Debug.LogError($"{file_name}没有配置矿工人口上限");
                    EditorSceneManager.CloseScene(scene, true);
                    continue;
                }

                // 开始导出
                CfgLevel cfg_level = new()
                {
                    ArrObstacle = new(),
                    ArrOre = new(),
                    ArrDoor = new(),
                    ArrPit = new(),
                    ArrSpawnPoint = new(),
                    Height = tile_map.TilemapHeight,
                    HeightOffset = tile_map.TilemapHeightOffset,
                    MaxWorkerNum = max_workernum
                };

                int y_min, y_max;
                bool oddor_even = TileUtils.OddorEven(tile_map.TilemapHeight);
                if (oddor_even)
                {
                    y_min = -(tile_map.TilemapHeight - 1) / 2;
                    y_max = (tile_map.TilemapHeight - 1) / 2;
                }
                else
                {
                    y_min = -tile_map.TilemapHeight / 2;
                    y_max = (tile_map.TilemapHeight - 2) / 2;
                }
                y_min += tile_map.TilemapHeightOffset;
                y_max += tile_map.TilemapHeightOffset;

                foreach (var i in tile_map)
                {
                    var tile = i.Value;
                    var tile_obj = tile.GetComponentInChildren<Tilemap3D.TileObj>();

                    //Debug.Log(tile_obj.TileType);

                    switch (tile_obj.TileType)
                    {
                        case "Obstacle":
                            {
                                var pos_cell = tile.GridCellPosition;
                                var grid_x = pos_cell.x;
                                var grid_y = pos_cell.z;

                                if (grid_y < y_min || grid_y > y_max)
                                {
                                    Debug.LogError($"障碍物超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
                                    continue;
                                }

                                int w = tile_obj.GetPropertyAsInt("W");
                                int h = tile_obj.GetPropertyAsInt("H");
                                var list_gird = TileUtils.GetGridList(pos_cell, w, h);

                                foreach (var k in list_gird)
                                {
                                    var pos = new CfgPoint()
                                    {
                                        x = k.x,
                                        y = k.z,
                                    };

                                    if (pos.x < -4 || pos.x > 4 || pos.y < y_min || pos.y > y_max)
                                    {
                                        continue;
                                    }

                                    cfg_level.ArrObstacle.Add(pos);
                                }
                            }
                            break;
                        case "Ore":
                            {
                                int lv = tile_obj.GetPropertyAsInt("Lv");
                                string ore_type = tile_obj.GetPropertyAsString("OreType");
                                string chest_type = tile_obj.GetPropertyAsString("ChestType");

                                var pos_cell = tile.GridCellPosition;
                                var grid_x = pos_cell.x;
                                var grid_y = pos_cell.z;

                                if (grid_y < y_min || grid_y > y_max)
                                {
                                    Debug.LogError($"矿石超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
                                    continue;
                                }

                                CfgPoint pos = new()
                                {
                                    x = grid_x,
                                    y = grid_y,
                                };

                                CfgOre cfg_ore = new()
                                {
                                    Pos = pos,
                                    Lv = lv,
                                    OreType = (OreType)Enum.Parse(typeof(OreType), ore_type),
                                    ChestType = (ChestType)Enum.Parse(typeof(ChestType), string.IsNullOrEmpty(chest_type) ? "None" : chest_type),
                                    VFX = string.Empty,
                                };

                                cfg_level.ArrOre.Add(cfg_ore);

                                //Debug.Log($"Ore Lv={lv} OreType={ore_type} ChestType={chest_type} VFX={vfx}");
                            }
                            break;
                        case "Door":
                            {
                                // 格子数，5x1，原点位于格子正中间

                                int lv = tile_obj.GetPropertyAsInt("Lv");
                                int order = tile_obj.GetPropertyAsInt("Order");
                                string unlock_cards = tile_obj.GetPropertyAsString("UnlockCards");

                                List<CfgDoorEffect> list_effect = new();
                                for (int m = 1; m <= 4; m++)
                                {
                                    string card_type = tile_obj.GetPropertyAsString($"DoorEffect{m}");
                                    int op = tile_obj.GetPropertyAsInt($"Operator{m}");
                                    int percent_or_number = tile_obj.GetPropertyAsInt($"PercentOrNumber{m}");
                                    int value = tile_obj.GetPropertyAsInt($"Value{m}");

                                    CfgDoorEffect cfg_effect = new()
                                    {
                                        CardType = (CardType)Enum.Parse(typeof(CardType), card_type),
                                        Value = value,
                                    };
                                    list_effect.Add(cfg_effect);
                                }

                                var pos_cell = tile.GridCellPosition;
                                var grid_x = pos_cell.x;
                                var grid_y = pos_cell.z;

                                if (grid_y < y_min || grid_y > y_max)
                                {
                                    Debug.LogError($"小门超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
                                    continue;
                                }

                                CfgPoint pos = new()
                                {
                                    x = grid_x,
                                    y = grid_y,
                                };

                                CfgDoor cfg_door = new()
                                {
                                    Pos = pos,
                                    Lv = lv,
                                    Order = order,
                                    UnlockCards = unlock_cards,
                                    ArrEffect = list_effect.ToArray(),
                                };

                                cfg_level.ArrDoor.Add(cfg_door);
                            }
                            break;
                        case "DoorEnd":
                            {
                                int lv = tile_obj.GetPropertyAsInt("Lv");
                                int order = tile_obj.GetPropertyAsInt("Order");
                                string unlock_cards = tile_obj.GetPropertyAsString("UnlockCards");

                                var pos_cell = tile.GridCellPosition;
                                var grid_x = pos_cell.x;
                                var grid_y = pos_cell.z;

                                if (grid_y < y_min || grid_y > y_max)
                                {
                                    Debug.LogError($"大门超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
                                    continue;
                                }

                                CfgPoint pos = new()
                                {
                                    x = grid_x,
                                    y = grid_y,
                                };

                                CfgDoor cfg_door = new()
                                {
                                    Pos = pos,
                                    Lv = lv,
                                    Order = order,
                                    UnlockCards = unlock_cards,
                                };

                                cfg_level.ArrDoor.Add(cfg_door);
                            }
                            break;
                        case "Pit":
                            {
                                // 格子数，3x3，原点位于格子正中间

                                int ore_lv = tile_obj.GetPropertyAsInt("OreLv");
                                int stone_id = tile_obj.GetPropertyAsInt("StoneId");
                                int card_auto_lv = tile_obj.GetPropertyAsInt("CardAutoLv");
                                string card_auto_color = tile_obj.GetPropertyAsString("CardAutoColor");
                                string unlock_cards = tile_obj.GetPropertyAsString("UnlockCards");

                                var pos_cell = tile.GridCellPosition;
                                var grid_x = pos_cell.x;
                                var grid_y = pos_cell.z;

                                if (grid_y < y_min || grid_y > y_max)
                                {
                                    Debug.LogError($"矿井超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
                                    continue;
                                }

                                List<CfgPoint> list_grid = new();
                                for (int m = -1; m <= 1; m++)
                                {
                                    for (int n = -1; n <= 1; n++)
                                    {
                                        var pos = new CfgPoint()
                                        {
                                            x = grid_x + m,
                                            y = grid_y + n,
                                        };

                                        if (pos.x < -4 || pos.x > 4 || pos.y < y_min || pos.y > y_max)
                                        {
                                            continue;
                                        }

                                        list_grid.Add(pos);
                                    }
                                }

                                CfgPoint pos_pit = new()
                                {
                                    x = grid_x,
                                    y = grid_y,
                                };

                                CfgPit cfg_pit = new()
                                {
                                    Pos = pos_pit,
                                    ArrGrid = list_grid.ToArray(),
                                    OreLv = ore_lv,
                                    StoneId = stone_id,
                                    CardAutoLv = card_auto_lv,
                                    CardAutoColor = card_auto_color,
                                    UnlockCards = unlock_cards,
                                };

                                cfg_level.ArrPit.Add(cfg_pit);
                            }
                            break;
                        case "PointSpawn":
                            {
                                int order = tile_obj.GetPropertyAsInt("Order");

                                var pos_cell = tile.GridCellPosition;
                                var grid_x = pos_cell.x;
                                var grid_y = pos_cell.z;

                                CfgPoint pos = new()
                                {
                                    x = grid_x,
                                    y = grid_y,
                                };

                                CfgPointSpawnForgeAndAirdrop cfg_spawn = new()
                                {
                                    Order = order,
                                    Pos = pos
                                };

                                cfg_level.ArrSpawnPoint.Add(cfg_spawn);

                                //Debug.Log(obj.m_Type + " Order=" + cfg_spawn.Order + " Pos=" + cfg_spawn.Pos + " GridX=" + grid_x + " GridY=" + grid_y);
                            }
                            break;
                    }
                }

                cfg_level.ArrSpawnPoint.Sort((a, b) =>
                {
                    return a.Order.CompareTo(b.Order);
                });

                cfg_level.ArrDoor.Sort((a, b) =>
                {
                    return a.Order.CompareTo(b.Order);
                });

                var json_str = Newtonsoft.Json.JsonConvert.SerializeObject(cfg_level, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText($"{dir_export}{file_name}.json", json_str);

                EditorSceneManager.CloseScene(scene, true);
            }

            // 将Arts/LevelData下所有关卡配置文件复制到Server/LevelData目录下
            // todo
            string outputServerFolder = Path.Combine(Environment.CurrentDirectory, "../Server/LevelData/");
            UnityEngine.Debug.Log($"目标路径:{outputServerFolder}");

            string[] files = System.IO.Directory.GetFiles(dir_export);
            // 如果目标路径不存在,则创建目标路径
            if (!System.IO.Directory.Exists(outputServerFolder))
            {
                System.IO.Directory.CreateDirectory(outputServerFolder);
            }
            foreach (string file in files)
            {
                string name = System.IO.Path.GetFileName(file);
                var ext = Path.GetExtension(file);
                if (ext == ".json")
                {
                    string dest = System.IO.Path.Combine(outputServerFolder, name);
                    System.IO.File.Copy(file, dest, true); //复制文件
                    //UnityEngine.Debug.Log($"Recast：从{file}复制obj文件到{dest}成功");
                }
            }

            Close();
        }

        //[Button("所有普通关卡导出到单张Csv表", ButtonSizes.Gigantic), GUIColor(1, 0.5f, 0)]
        //public void ButtonExport3DAll2CSV()
        //{
        //    var current_scene = EditorSceneManager.GetActiveScene();
        //    if (!current_scene.name.StartsWith("Main"))
        //    {
        //        Debug.Log($"请打开主场景再进行导出");
        //        return;
        //    }

        //    string p = Path.Combine(Environment.CurrentDirectory, "./Assets/Arts/LevelScenes/");
        //    var dir_levels = new DirectoryInfo(p);
        //    p = Path.Combine(Environment.CurrentDirectory, "./Assets/Arts/LevelData/");
        //    var dir_export = Path.GetFullPath(p);
        //    if (!Directory.Exists(dir_export))
        //    {
        //        Directory.CreateDirectory(dir_export);
        //    }
        //    Debug.Log($"ExportInfo");
        //    Debug.Log($"DirLevels={dir_levels.FullName}");
        //    Debug.Log($"DirExport={dir_export}");

        //    List<LevelRecord> list_record_all = new();

        //    var files1 = dir_levels.GetFiles("*.unity");
        //    foreach (var f in files1)
        //    {
        //        var file_name = Path.GetFileNameWithoutExtension(f.FullName);
        //        if (!file_name.StartsWith("Level")) continue;

        //        Debug.Log(file_name);

        //        var scene = EditorSceneManager.OpenScene($"Assets/Arts/LevelScenes/{file_name}.unity", OpenSceneMode.Additive);

        //        Tilemap3D.Tilemap tile_map = null;
        //        var arr_go = scene.GetRootGameObjects();
        //        foreach (var go in arr_go)
        //        {
        //            var c = go.GetComponent<Tilemap3D.Tilemap>();
        //            if (c != null)
        //            {
        //                tile_map = c;
        //                break;
        //            }
        //        }

        //        if (tile_map == null)
        //        {
        //            Debug.LogError($"{file_name}上缺少Tilemap组件");
        //            EditorSceneManager.CloseScene(scene, true);
        //            continue;
        //        }

        //        int y_min, y_max;
        //        if (tile_map.TilemapHeight % 2 != 0)
        //        {
        //            y_min = -(tile_map.TilemapHeight - 1) / 2;
        //            y_max = (tile_map.TilemapHeight - 1) / 2;
        //        }
        //        else
        //        {
        //            y_min = -tile_map.TilemapHeight / 2;
        //            y_max = (tile_map.TilemapHeight - 2) / 2;
        //        }
        //        y_min += tile_map.TilemapHeightOffset;
        //        y_max += tile_map.TilemapHeightOffset;

        //        List<LevelRecord> list_record = new();

        //        string level_id = file_name;

        //        foreach (var i in tile_map)
        //        {
        //            var tile = i.Value;
        //            var tile_obj = tile.GetComponentInChildren<Tilemap3D.TileObj>();

        //            //Debug.Log(tile_obj.TileType);

        //            switch (tile_obj.TileType)
        //            {
        //                case "Ore":
        //                    {
        //                        int lv = tile_obj.GetPropertyAsInt("Lv");
        //                        string ore_type = tile_obj.GetPropertyAsString("OreType");
        //                        string chest_type = tile_obj.GetPropertyAsString("ChestType");

        //                        var pos_cell = tile.GridCellPosition;
        //                        var grid_x = pos_cell.x;
        //                        var grid_y = pos_cell.z;

        //                        if (grid_y < y_min || grid_y > y_max)
        //                        {
        //                            Debug.LogError($"矿石超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
        //                            continue;
        //                        }

        //                        var r = new LevelRecord()
        //                        {
        //                            LevelId = level_id,
        //                            UnitType = "Ore",
        //                            PosX = grid_x,
        //                            PosY = grid_y,
        //                            Lv = lv,
        //                            Param1 = ore_type,
        //                            Param2 = chest_type,
        //                        };
        //                        list_record.Add(r);
        //                    }
        //                    break;
        //                case "Door":
        //                    {
        //                        // 格子数，5x1，原点位于格子正中间

        //                        int lv = tile_obj.GetPropertyAsInt("Lv");
        //                        int order = tile_obj.GetPropertyAsInt("Order");
        //                        string unlock_cards = tile_obj.GetPropertyAsString("UnlockCards");

        //                        var pos_cell = tile.GridCellPosition;
        //                        var grid_x = pos_cell.x;
        //                        var grid_y = pos_cell.z;

        //                        if (grid_y < y_min || grid_y > y_max)
        //                        {
        //                            Debug.LogError($"小门超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
        //                            continue;
        //                        }

        //                        var r = new LevelRecord()
        //                        {
        //                            LevelId = level_id,
        //                            UnitType = "Door",
        //                            PosX = grid_x,
        //                            PosY = grid_y,
        //                            Lv = lv,
        //                            Param1 = order.ToString(),
        //                            Param4 = unlock_cards,
        //                        };
        //                        list_record.Add(r);

        //                        for (int m = 1; m <= 4; m++)
        //                        {
        //                            string card_type = tile_obj.GetPropertyAsString($"DoorEffect{m}");
        //                            int op = tile_obj.GetPropertyAsInt($"Operator{m}");
        //                            int percent_or_number = tile_obj.GetPropertyAsInt($"PercentOrNumber{m}");
        //                            int value = tile_obj.GetPropertyAsInt($"Value{m}");

        //                            switch (m)
        //                            {
        //                                case 1:
        //                                    r.CardType1 = card_type;

        //                                    r.Value1 = value;
        //                                    break;
        //                                case 2:
        //                                    r.CardType2 = card_type;

        //                                    r.Value2 = value;
        //                                    break;
        //                                case 3:
        //                                    r.CardType3 = card_type;

        //                                    r.Value3 = value;
        //                                    break;
        //                                case 4:
        //                                    r.CardType4 = card_type;
        //                                    r.Value4 = value;
        //                                    break;
        //                            }
        //                        }
        //                    }
        //                    break;
        //                case "DoorEnd":
        //                    {
        //                        int lv = tile_obj.GetPropertyAsInt("Lv");
        //                        int order = tile_obj.GetPropertyAsInt("Order");
        //                        string unlock_cards = tile_obj.GetPropertyAsString("UnlockCards");

        //                        var pos_cell = tile.GridCellPosition;
        //                        var grid_x = pos_cell.x;
        //                        var grid_y = pos_cell.z;

        //                        if (grid_y < y_min || grid_y > y_max)
        //                        {
        //                            Debug.LogError($"大门超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
        //                            continue;
        //                        }

        //                        var r = new LevelRecord()
        //                        {
        //                            LevelId = level_id,
        //                            UnitType = "DoorEnd",
        //                            PosX = grid_x,
        //                            PosY = grid_y,
        //                            Lv = lv,
        //                            Param1 = order.ToString(),
        //                            Param4 = unlock_cards,
        //                        };
        //                        list_record.Add(r);
        //                    }
        //                    break;
        //                case "Pit":
        //                    {
        //                        // 格子数，3x3，原点位于格子正中间

        //                        int ore_lv = tile_obj.GetPropertyAsInt("OreLv");
        //                        int stone_id = tile_obj.GetPropertyAsInt("StoneId");
        //                        int card_auto_lv = tile_obj.GetPropertyAsInt("CardAutoLv");
        //                        string card_auto_color = tile_obj.GetPropertyAsString("CardAutoColor");
        //                        string unlock_cards = tile_obj.GetPropertyAsString("UnlockCards");

        //                        var pos_cell = tile.GridCellPosition;
        //                        var grid_x = pos_cell.x;
        //                        var grid_y = pos_cell.z;

        //                        if (grid_y < y_min || grid_y > y_max)
        //                        {
        //                            Debug.LogError($"矿井超出网格之外，请检查网格范围，确保包含大门和下方空投炮");
        //                            continue;
        //                        }

        //                        var r = new LevelRecord()
        //                        {
        //                            LevelId = level_id,
        //                            UnitType = "Pit",
        //                            PosX = grid_x,
        //                            PosY = grid_y,
        //                            Lv = ore_lv,
        //                            Param1 = stone_id.ToString(),
        //                            Param2 = card_auto_lv.ToString(),
        //                            Param3 = card_auto_color,
        //                            Param4 = unlock_cards,
        //                        };
        //                        list_record.Add(r);
        //                    }
        //                    break;
        //            }
        //        }

        //        list_record.Sort((a, b) =>
        //        {
        //            int x_a = a.PosX + 4;
        //            int y_a = a.PosY + 1000;
        //            int pos_a = y_a * 9 + x_a;

        //            int x_b = b.PosX + 4;
        //            int y_b = b.PosY + 1000;
        //            int pos_b = y_b * 9 + x_b;

        //            return pos_a.CompareTo(pos_b);
        //        });

        //        list_record_all.AddRange(list_record);

        //        EditorSceneManager.CloseScene(scene, true);
        //    }

        //    using (var writer = new StreamWriter($"{dir_export}NormalLevels.csv"))
        //    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //    {
        //        csv.WriteRecords(list_record_all);
        //    }

        //    Close();
        //}

        //[Button("将单张Csv表的数值导入到所有关卡中", ButtonSizes.Gigantic), GUIColor(1, 0.5f, 0)]
        //public void ButtonImportCSV2NormalLevels()
        //{
        //    // 读取Csv
        //    string p = Path.Combine(Environment.CurrentDirectory, "./Assets/Arts/LevelScenes/");
        //    var dir_levels = new DirectoryInfo(p);
        //    p = Path.Combine(Environment.CurrentDirectory, "./Assets/Arts/LevelData/");
        //    var dir_export = Path.GetFullPath(p);
        //    if (!Directory.Exists(dir_export))
        //    {
        //        Directory.CreateDirectory(dir_export);
        //    }
        //    Debug.Log($"ExportInfo");
        //    Debug.Log($"DirLevels={dir_levels.FullName}");
        //    Debug.Log($"DirExport={dir_export}");

        //    Dictionary<string, List<LevelRecord>> map_record_all = new();// Key=LevelId

        //    using TextReader tr = new StreamReader($"{dir_export}NormalLevels.csv", Encoding.UTF8);
        //    using var csv = new CsvHelper.CsvReader(tr, new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)
        //    {
        //        Delimiter = ",",
        //    });

        //    csv.Read();
        //    csv.ReadHeader();
        //    while (csv.Read())
        //    {
        //        var r = new LevelRecord
        //        {
        //            LevelId = csv.GetField<string>("LevelId"),
        //            UnitType = csv.GetField("UnitType"),
        //            PosX = csv.GetField<int>("PosX"),
        //            PosY = csv.GetField<int>("PosY"),
        //            Lv = csv.GetField<int>("Lv"),
        //            Param1 = csv.GetField("Param1"),
        //            Param2 = csv.GetField("Param2"),
        //            Param3 = csv.GetField("Param3"),
        //            Param4 = csv.GetField("Param4"),

        //            CardType1 = csv.GetField("CardType1"),
        //            Value1 = csv.GetField<int>("Value1"),
        //            CardType2 = csv.GetField("CardType2"),
        //            Value2 = csv.GetField<int>("Value2"),
        //            CardType3 = csv.GetField("CardType3"),
        //            Value3 = csv.GetField<int>("Value3"),
        //            CardType4 = csv.GetField("CardType4"),
        //            Value4 = csv.GetField<int>("Value4"),
        //        };

        //        map_record_all.TryGetValue(r.LevelId, out var list_record);
        //        if (list_record == null)
        //        {
        //            list_record = new();
        //            map_record_all[r.LevelId] = list_record;
        //        }
        //        list_record.Add(r);
        //    }

        //    // 遍历所有关卡
        //    var files1 = dir_levels.GetFiles("*.unity");
        //    foreach (var f in files1)
        //    {
        //        var file_name = Path.GetFileNameWithoutExtension(f.FullName);
        //        if (!file_name.StartsWith("Level")) continue;

        //        Debug.Log(file_name);

        //        string full_file_name = $"Assets/Arts/LevelScenes/{file_name}.unity";
        //        var scene = EditorSceneManager.OpenScene(full_file_name, OpenSceneMode.Single);

        //        Tilemap3D.Tilemap tile_map = null;
        //        var arr_go = scene.GetRootGameObjects();
        //        foreach (var go in arr_go)
        //        {
        //            var c = go.GetComponent<Tilemap3D.Tilemap>();
        //            if (c != null)
        //            {
        //                tile_map = c;
        //                break;
        //            }
        //        }

        //        if (tile_map == null)
        //        {
        //            Debug.LogError($"{file_name}上缺少Tilemap组件");
        //            continue;
        //        }

        //        string level_id = file_name;

        //        map_record_all.TryGetValue(level_id, out var list_record);
        //        if (list_record == null)
        //        {
        //            Debug.LogError($"关卡{level_id}缺少对应的Csv数据");
        //            continue;
        //        }

        //        foreach (var i in tile_map)
        //        {
        //            var tile = i.Value;
        //            var tile_obj = tile.GetComponentInChildren<Tilemap3D.TileObj>();
        //            tile_obj.ClearProperty();
        //            //Debug.Log(tile_obj.TileType);

        //            switch (tile_obj.TileType)
        //            {
        //                case "Ore":
        //                    {
        //                        var pos_cell = tile.GridCellPosition;
        //                        var grid_x = pos_cell.x;
        //                        var grid_y = pos_cell.z;

        //                        LevelRecord r = null;
        //                        foreach (var kk in list_record)
        //                        {
        //                            if (grid_x == kk.PosX && grid_y == kk.PosY && kk.UnitType == "Ore")
        //                            {
        //                                r = kk;
        //                                break;
        //                            }
        //                        }

        //                        if (r != null)
        //                        {
        //                            tile_obj.SetProperty("OreType", r.Param1);
        //                            tile_obj.SetProperty("ChestType", r.Param2);
        //                            tile_obj.SetProperty("Lv", r.Lv);

        //                            EditorUtility.SetDirty(tile_obj);
        //                        }
        //                    }
        //                    break;
        //                case "Door":
        //                    {
        //                        // 格子数，5x1，原点位于格子正中间

        //                        var pos_cell = tile.GridCellPosition;
        //                        var grid_x = pos_cell.x;
        //                        var grid_y = pos_cell.z;

        //                        LevelRecord r = null;
        //                        foreach (var kk in list_record)
        //                        {
        //                            if (grid_x == kk.PosX && grid_y == kk.PosY && kk.UnitType == "Door")
        //                            {
        //                                r = kk;
        //                                break;
        //                            }
        //                        }

        //                        if (r != null)
        //                        {
        //                            tile_obj.SetProperty("Lv", r.Lv);
        //                            tile_obj.SetProperty("Order", r.Param1);
        //                            tile_obj.SetProperty("UnlockCards", r.Param4);

        //                            tile_obj.SetProperty("DoorEffect1", r.CardType1);

        //                            tile_obj.SetProperty("Value1", r.Value1);

        //                            tile_obj.SetProperty("DoorEffect2", r.CardType2);

        //                            tile_obj.SetProperty("Value2", r.Value2);

        //                            tile_obj.SetProperty("DoorEffect3", r.CardType3);

        //                            tile_obj.SetProperty("Value3", r.Value3);

        //                            tile_obj.SetProperty("DoorEffect4", r.CardType4);
        //                            tile_obj.SetProperty("Value4", r.Value4);

        //                            EditorUtility.SetDirty(tile_obj);
        //                        }
        //                    }
        //                    break;
        //                case "DoorEnd":
        //                    {
        //                        var pos_cell = tile.GridCellPosition;
        //                        var grid_x = pos_cell.x;
        //                        var grid_y = pos_cell.z;

        //                        LevelRecord r = null;
        //                        foreach (var kk in list_record)
        //                        {
        //                            if (grid_x == kk.PosX && grid_y == kk.PosY && kk.UnitType == "DoorEnd")
        //                            {
        //                                r = kk;
        //                                break;
        //                            }
        //                        }

        //                        if (r != null)
        //                        {
        //                            tile_obj.SetProperty("Lv", r.Lv);
        //                            tile_obj.SetProperty("Order", r.Param1);
        //                            tile_obj.SetProperty("UnlockCards", r.Param4);

        //                            EditorUtility.SetDirty(tile_obj);
        //                        }
        //                    }
        //                    break;
        //                case "Pit":
        //                    {
        //                        // 格子数，3x3，原点位于格子正中间

        //                        var pos_cell = tile.GridCellPosition;
        //                        var grid_x = pos_cell.x;
        //                        var grid_y = pos_cell.z;

        //                        LevelRecord r = null;
        //                        foreach (var kk in list_record)
        //                        {
        //                            if (grid_x == kk.PosX && grid_y == kk.PosY && kk.UnitType == "Pit")
        //                            {
        //                                r = kk;
        //                                break;
        //                            }
        //                        }

        //                        if (r != null)
        //                        {
        //                            tile_obj.SetProperty("OreLv", r.Lv);
        //                            tile_obj.SetProperty("StoneId", r.Param1);
        //                            tile_obj.SetProperty("CardAutoLv", r.Param2);
        //                            tile_obj.SetProperty("CardAutoColor", r.Param3);
        //                            tile_obj.SetProperty("UnlockCards", r.Param4);

        //                            EditorUtility.SetDirty(tile_obj);
        //                        }
        //                    }
        //                    break;
        //            }
        //        }

        //        EditorSceneManager.SaveScene(scene, full_file_name, false);
        //    }

        //    EditorSceneManager.OpenScene("Assets/Scenes/Main.unity", OpenSceneMode.Single);

        //    Close();
        //}

        //[Button("JsonToCsv", ButtonSizes.Gigantic), GUIColor(1, 0.5f, 0)]
        //public void ButtonJsonToCSV()
        //{
        //    string p = Path.Combine(Environment.CurrentDirectory, "./Assets/Arts/LevelData/");
        //    var dir_jsons = new DirectoryInfo(p);
        //    p = Path.Combine(Environment.CurrentDirectory, "./Assets/Arts/LevelData/");
        //    var dir_export = Path.GetFullPath(p);
        //    if (!Directory.Exists(dir_export))
        //    {
        //        Directory.CreateDirectory(dir_export);
        //    }
        //    Debug.Log($"ExportInfo");
        //    Debug.Log($"DirLevels={dir_jsons.FullName}");
        //    Debug.Log($"DirExport={dir_export}");

        //    List<LevelRecord> list_record_all = new();

        //    var files1 = dir_jsons.GetFiles("*.json");
        //    foreach (var f in files1)
        //    {
        //        string s = File.ReadAllText(f.FullName);
        //        var cfg_level = Newtonsoft.Json.JsonConvert.DeserializeObject<CfgLevel>(s);
        //        int y_min, y_max;
        //        int height = cfg_level.Height;
        //        if (height % 2 != 0)
        //        {
        //            y_min = -(height - 1) / 2;
        //            y_max = (height - 1) / 2;
        //        }
        //        else
        //        {
        //            y_min = -height / 2;
        //            y_max = (height - 2) / 2;
        //        }
        //        y_min += cfg_level.HeightOffset;
        //        y_max += cfg_level.HeightOffset;



        //        List<LevelRecord> list_record = new();

        //        string level_id = Path.GetFileNameWithoutExtension(f.FullName);
        //        foreach (var i in cfg_level.ArrOre)
        //        {
        //            var r = new LevelRecord()
        //            {
        //                LevelId = level_id,
        //                UnitType = "Ore",
        //                PosX = i.Pos.x,
        //                PosY = i.Pos.y,
        //                Lv = i.Lv,
        //                Param1 = i.OreType.ToString(),
        //                Param2 = i.ChestType.ToString(),
        //            };
        //            list_record.Add(r);
        //        }
        //        foreach (var i in cfg_level.ArrPit)
        //        {
        //            var r = new LevelRecord()
        //            {
        //                LevelId = level_id,
        //                UnitType = "Pit",
        //                PosX = i.Pos.x,
        //                PosY = i.Pos.y,
        //                Lv = i.OreLv,
        //                Param1 = i.StoneId.ToString(),
        //                Param2 = i.CardAutoLv.ToString(),
        //                Param3 = i.CardAutoColor.ToString(),
        //                Param4 = i.UnlockCards.ToString(),
        //            };
        //            list_record.Add(r);
        //        }
        //        foreach (var i in cfg_level.ArrDoor)
        //        {
        //            if (i.ArrEffect == null || i.ArrEffect.Length == 0)
        //            {
        //                var r = new LevelRecord()
        //                {
        //                    LevelId = level_id,
        //                    UnitType = "DoorEnd",
        //                    PosX = i.Pos.x,
        //                    PosY = i.Pos.y,
        //                    Lv = i.Lv,
        //                    Param1 = i.Order.ToString(),
        //                    Param4 = i.UnlockCards,
        //                };
        //                list_record.Add(r);
        //            }
        //            else
        //            {
        //                var r = new LevelRecord()
        //                {
        //                    LevelId = level_id,
        //                    UnitType = "Door",
        //                    PosX = i.Pos.x,
        //                    PosY = i.Pos.y,
        //                    Lv = i.Lv,
        //                    Param1 = i.Order.ToString(),
        //                    Param4 = i.UnlockCards,
        //                };
        //                list_record.Add(r);

        //                for (int m = 1; m <= 4; m++)
        //                {
        //                    var effect = i.ArrEffect[m - 1];
        //                    string card_type = effect.CardType.ToString();

        //                    int value = effect.Value;

        //                    switch (m)
        //                    {
        //                        case 1:
        //                            r.CardType1 = card_type;
        //                            r.Value1 = value;
        //                            break;
        //                        case 2:
        //                            r.CardType2 = card_type;
        //                            r.Value2 = value;
        //                            break;
        //                        case 3:
        //                            r.CardType3 = card_type;

        //                            r.Value3 = value;
        //                            break;
        //                        case 4:
        //                            r.CardType4 = card_type;
        //                            r.Value4 = value;
        //                            break;
        //                    }
        //                }
        //            }
        //        }

        //        list_record.Sort((a, b) =>
        //        {
        //            int x_a = a.PosX + 4;
        //            int y_a = a.PosY + 1000;
        //            int pos_a = y_a * 9 + x_a;

        //            int x_b = b.PosX + 4;
        //            int y_b = b.PosY + 1000;
        //            int pos_b = y_b * 9 + x_b;

        //            return pos_a.CompareTo(pos_b);
        //        });

        //        list_record_all.AddRange(list_record);
        //    }

        //    using (var writer = new StreamWriter($"{dir_export}NormalLevels.csv"))
        //    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //    {
        //        csv.WriteRecords(list_record_all);
        //    }

        //    Close();
        //}

        //[Button("同步Json", ButtonSizes.Gigantic), GUIColor(1, 0.5f, 0)]
        //public void ButtonJson()
        //{
        //    ButtonJsonToCSV();

        //    ButtonImportCSV2NormalLevels();
        //}

        //[Button("同步CSV", ButtonSizes.Gigantic), GUIColor(1, 0.5f, 0)]
        //public void ButtonCSV()
        //{
        //    ButtonImportCSV2NormalLevels();

        //    ButtonExport3DAll2JSON();
        //}

        //[Button("同步Level", ButtonSizes.Gigantic), GUIColor(1, 0.5f, 0)]
        //public void ButtonLevel()
        //{
        //    ButtonExport3DAll2CSV();

        //    ButtonExport3DAll2JSON();
        //}
    }
}

#endif