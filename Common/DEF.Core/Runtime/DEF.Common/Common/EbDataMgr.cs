using System;
using System.Collections.Generic;
using System.IO;

namespace DEF
{
    public class EbDataMgr
    {
        static Dictionary<string, EbTableBuffer> MapTable { get; set; } = new();
        static Dictionary<string, Dictionary<int, EbData>> MapData { get; set; } = new();
        static EbSqlite Sqlite { get; set; } = new();

        public static Dictionary<string, byte[]> LoadAllTableFromMemoryDb(string db_name, byte[] db_data)
        {
            Dictionary<string, byte[]> map_table = new();

            using (MemoryStream ms = new(db_data))
            {
                if (!Sqlite.OpenDb(db_name, ms))
                {
                    //EbLog.Note("EbDataMgr.setup() failed! Can not Open Stream!");
                    return map_table;
                }

                // 加载所有Table数据
                HashSet<string> list_tablename = _loadAllTableName();
                foreach (var i in list_tablename)
                {
                    _loadTable(i);
                    map_table[i] = GetTableAsBytes(i);
                }

                Sqlite.CloseDb();
            }

            return map_table;
        }

        public static EbTableBuffer GetTable(string table_name)
        {
            MapTable.TryGetValue(table_name, out EbTableBuffer table);
            if (table == null)
            {
                //EbLog.Error("EbDb.getTable() Error! not exist table,table_name=" + table_name);
            }
            return table;
        }

        public static byte[] GetTableAsBytes(string table_name)
        {
            MapTable.TryGetValue(table_name, out EbTableBuffer table);
            if (table == null)
            {
                //EbLog.Error("EbDb.getTable() Error! not exist table,table_name=" + table_name);
            }
            return table.GetTableData();
        }

        public static void ParseTableAllData<T>(string table_name) where T : EbData, new()
        {
            string key = typeof(T).Name;
            Dictionary<int, EbData> map_data = new();
            MapData[key] = map_data;
            EbTableBuffer table_buffer = GetTable(table_name);
            while (!table_buffer.IsReadEnd())
            {
                T data = new()
                {
                    Id = table_buffer.ReadInt()
                };

                data.Load(table_buffer);

                map_data[data.Id] = data;
            }
        }

        public static T GetData<T>(int id) where T : EbData
        {
            string key = typeof(T).Name;

            MapData.TryGetValue(key, out Dictionary<int, EbData> map_data);
            if (map_data == null)
            {
                throw new Exception();
            }

            map_data.TryGetValue(id, out EbData data);
            if (data == null) return default;
            else return (T)data;
        }

        public static Dictionary<int, EbData> GetMapData<T>() where T : EbData
        {
            string key = typeof(T).Name;

            MapData.TryGetValue(key, out Dictionary<int, EbData> map_data);
            return map_data;
        }

        // 获取Db中所有表名
        static HashSet<string> _loadAllTableName()
        {
            string str_query = string.Format("SELECT * FROM {0};", "sqlite_master");
            HashSet<string> list_tablename = Sqlite.GetAllTableName(str_query);
            return list_tablename;
        }

        static void _loadTable(string table_name)
        {
            string str_query_select = string.Format("SELECT * FROM {0};", table_name);

            Dictionary<int, List<DataInfo>> map_data = Sqlite.GetTableData(str_query_select);
            if (map_data.Count <= 0)
            {
                return;
            }

            EbTableBuffer table = new(table_name);

            foreach (var i in map_data)
            {
                List<DataInfo> list_data_info = i.Value;

                foreach (var data_info in list_data_info)
                {
                    switch (data_info.data_type)
                    {
                        case 1:
                            table.WriteInt(data_info.data_value_i);
                            break;
                        case 2:
                            table.WriteFloat(data_info.data_value_f);
                            break;
                        case 3:
                            table.WriteString(data_info.data_value_s);
                            break;
                        case 5:
                            // 如果字段是T类型，且为null，会运行到此处
                            table.WriteString(string.Empty);
                            break;
                    }
                }
            }

            table.WriteEnd();

            MapTable[table.TableName] = table;
        }
    }
}