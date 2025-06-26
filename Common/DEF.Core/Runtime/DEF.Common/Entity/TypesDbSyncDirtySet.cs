using MongoDB.Bson;
using System.Collections.Generic;

namespace DEF
{
    // 辅助函数，Entity获取全路径

    // 更新State属性值
    // 创建Entity
    //      时机：OnAwake之前
    //      添加到MapAddEntity，可能复用EntityId，与MapRemoveEntity中EntityId有重复
    // 删除Entity
    //      时机：OnDestroy之后
    //      看MapAddEntity是否存在Entity引用，存在则从MapAddEntity移除，不存在则添加到MapRemoveEntity
    //      看MapDirtyStates是否存在Entity引用，存在则从MapDirtyStates移除
    public class DbSyncDirtySet
    {
        public List<string> Map1RemoveEntity { get; set; }// 第1步，删除BsonDocument
        public Dictionary<Entity, BsonDocument> Map2AddEntity { get; set; }// 第2步，添加BsonDocument，需要有次序
        public Dictionary<Entity, Dictionary<string, string>> Map3DirtyStates { get; set; }// 第3步，更新State，无需次序
    }
}