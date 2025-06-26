using System.Collections.Generic;
using System.IO;

namespace DEF
{
    public class DataInfo
    {
        public string data_name;
        public int data_type;
        public int data_value_i;
        public float data_value_f;
        public string data_value_s;
    }

    public class EbSqlite
    {
        //SQLiteDB mSQLiteDB { get; set; }

        // todo，使用System.Data.SQLite替代GameCloud.Unity.Sqlite
        //SQLiteConnection DbConnection { get; set; }
        //SQLiteCommand DbCommand { get; set; }
        //SQLiteDataReader DataReader { get; set; }

        public EbSqlite()
        {
            //mSQLiteDB = new SQLiteDB();
        }

        public bool OpenDb(string db_name, Stream stream)
        {
            //mSQLiteDB.OpenStream(db_name, stream);
            return true;
        }

        public void CloseDb()
        {
            //mSQLiteDB.Close();
        }

        public HashSet<string> GetAllTableName(string sqlite_query)
        {
            HashSet<string> list_tablename = new();

            //SQLiteQuery qr = new SQLiteQuery(mSQLiteDB, sqlite_query);
            //while (qr.Step())
            //{
            //    string s = qr.GetString("name");
            //    if (string.IsNullOrEmpty(s) || s.Contains("sqlite_")) continue;
            //    if (list_tablename.Contains(s))
            //    {
            //        continue;
            //    }
            //    list_tablename.Add(s);
            //}
            //qr.Release();

            return list_tablename;
        }

        public Dictionary<int, List<DataInfo>> GetTableData(string sqlite_query)
        {
            Dictionary<int, List<DataInfo>> map_table_data = new();

            return map_table_data;
        }
    }
}