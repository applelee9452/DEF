namespace DEF.SyncDb;

[Component("Item")]
public partial interface IComponentStateItem : IComponentState
{
    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    int ItemId { get; set; }// 通过ItemId关联该Item的配置数据

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    string ItemObjId { get; set; }// Item实例Id
}