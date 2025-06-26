#if !DEF_CLIENT

namespace DEF.IM;

[ContainerRpc("DEF.IM", "IMRegionMgr", ContainerStateType.Stateful)]
public interface IContainerStatefulIMRegionMgr : IContainerRpc
{
    // 保活
    Task Touch();

    // 获取所有分区Id列表
    Task<List<string>> GetRegionIdList();

    // 获取所有分区列表
    Task<List<Region>> GetRegionList();

    // 获取指定分区信息
    Task<Region> GetRegionInfo(int region_id);

    // 新建分区
    Task<Region> CreateRegion(string region_name);

    // 合并分区
    Task<Region> MergeRegion(int region_id_old, int region_id_new);

    // 修改分区名
    Task<Region> ChangeRegionName(int region_id, string region_name_new);

    // 删除分区
    Task<bool> DeleteRegion(string region_id);

    // 请求分配一个分区
    Task<Region> RequestAssignReion();

    // 获取默认分区Id
    Task<int> GetDefaultRegionId();

    // 设置默认分区Id，如果该分区Id不存在，则返回false
    Task<bool> SetDefaultRegionId(int region_id);
}

#endif