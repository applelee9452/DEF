#if !DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace DEF.IM;

// 分区管理，单实例，分区不可以被删除，只能失活
public class ContainerStatefulIMRegionMgr : ContainerStateful, IContainerStatefulIMRegionMgr
{
    List<Region> ListRegion { get; set; } = [];
    int DefaultRegionId { get; set; }// 默认分区Id
    Stopwatch StopwatchUpdate { get; set; }
    IDisposable TimerHandleUpdate { get; set; }
    int MaxRegionId { get; set; } = 10000;

    public override async Task OnCreate()
    {
        Logger.LogDebug("ContainerStatefulIMRegionMgr.OnCreate()");

        var list_data_region = await IMContext.Instance.Mongo.ReadListAsync<DataRegion>(StringDef.DbCollectionDataRegion);

        if (list_data_region != null && list_data_region.Count > 0)
        {
            foreach (var i in list_data_region)
            {
                Region region = new()
                {
                    RegionGuid = i._id,
                    RegionId = i.RegionId,
                    RegionName = i.RegionName,
                    PlayerNum = i.PlayerNum,
                    Dt = i.Dt,
                    IsActive = i.IsActive,
                    Merge2RegionId = i.Merge2RegionId,
                };

                if (i.RegionId > MaxRegionId)
                {
                    MaxRegionId = i.RegionId;
                }

                ListRegion.Add(region);
            }
        }

        if (ListRegion.Count == 0)
        {
            // 没有任何分区，则新建默认分区

            await ((IContainerStatefulIMRegionMgr)this).CreateRegion("default");
        }

        string name_region_default = typeof(DataRegionDefault).Name;
        var data_region_default = await IMContext.Instance.Mongo.ReadAsync<DataRegionDefault>(i => i._id == name_region_default, name_region_default);

        if (data_region_default == null)
        {
            DefaultRegionId = ListRegion[0].RegionId;

            data_region_default = new()
            {
                _id = name_region_default,
                RegionId = DefaultRegionId,
            };

            await IMContext.Instance.Mongo.ReplaceOneData(name_region_default, name_region_default, data_region_default);
        }
        else
        {
            DefaultRegionId = data_region_default.RegionId;
        }

        StopwatchUpdate = new Stopwatch();
        StopwatchUpdate.Start();
        TimerHandleUpdate = RegisterTimer((_) => TimerUpdate(),
            null, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(300));
    }

    public override Task OnDestroy()
    {
        if (TimerHandleUpdate != null)
        {
            TimerHandleUpdate.Dispose();
            TimerHandleUpdate = null;
        }

        if (StopwatchUpdate != null)
        {
            StopwatchUpdate.Stop();
            StopwatchUpdate = null;
        }

        Logger.LogDebug("ContainerStatefulIMRegionMgr.OnDestroy()");

        return Task.CompletedTask;
    }

    // 保活
    Task IContainerStatefulIMRegionMgr.Touch()
    {
        return Task.CompletedTask;
    }

    // 获取所有分区Id列表
    Task<List<string>> IContainerStatefulIMRegionMgr.GetRegionIdList()
    {
        List<string> list_region = [];

        foreach (var region in ListRegion)
        {
            list_region.Add(region.RegionGuid);
        }

        return Task.FromResult(list_region);
    }

    // 获取所有分区列表
    Task<List<Region>> IContainerStatefulIMRegionMgr.GetRegionList()
    {
        return Task.FromResult(ListRegion);
    }

    // 获取指定分区信息
    Task<Region> IContainerStatefulIMRegionMgr.GetRegionInfo(int region_id)
    {
        Region region = null;

        foreach (var i in ListRegion)
        {
            if (i.RegionId == region_id)
            {
                region = i;
                break;
            }
        }

        return Task.FromResult(region);
    }

    // 新建分区
    async Task<Region> IContainerStatefulIMRegionMgr.CreateRegion(string region_name)
    {
        Region region = new()
        {
            RegionGuid = Guid.NewGuid().ToString(),
            RegionId = ++MaxRegionId,
            RegionName = region_name,
            PlayerNum = 0,
            Dt = DateTime.UtcNow,
            IsActive = true,
            Merge2RegionId = 0,
        };

        ListRegion.Add(region);

        DataRegion data_region = new()
        {
            _id = region.RegionGuid,
            RegionId = region.RegionId,
            RegionName = region_name,
            PlayerNum = region.PlayerNum,
            Dt = region.Dt,
            IsActive = region.IsActive,
            Merge2RegionId = region.Merge2RegionId,
        };

        await IMContext.Instance.Mongo.InsertAsync(StringDef.DbCollectionDataRegion, data_region);

        return region;
    }

    // 合并分区
    async Task<Region> IContainerStatefulIMRegionMgr.MergeRegion(int region_id_old, int region_id_new)
    {
        Region src = null;
        Region target = null;

        foreach (var i in ListRegion)
        {
            if (i.RegionId == region_id_old)
            {
                src = i;
            }

            if (i.RegionId == region_id_new)
            {
                target = i;
            }
        }

        if (src == null || target == null)
        {
            return null;
        }

        if (!src.IsActive)
        {
            return null;
        }

        if (src.Merge2RegionId > 0)
        {
            return null;
        }

        if (!target.IsActive)
        {
            return null;
        }

        if (target.Merge2RegionId > 0)
        {
            return null;
        }

        src.IsActive = false;
        src.Merge2RegionId = target.RegionId;

        // 更新分区数据
        var filter = Builders<DataRegion>.Filter
            .Where(x => x._id == src.RegionGuid);
        var update = Builders<DataRegion>.Update
            .Set(x => x.IsActive, src.IsActive)
            .Set(x => x.Merge2RegionId, src.Merge2RegionId);
        await IMContext.Instance.Mongo.UpdateOneAsync(filter, StringDef.DbCollectionDataRegion, update);

        return target;
    }

    // 修改分区名
    async Task<Region> IContainerStatefulIMRegionMgr.ChangeRegionName(int region_id, string region_name_new)
    {
        Region region = null;

        foreach (var i in ListRegion)
        {
            if (i.RegionId == region_id)
            {
                i.RegionName = region_name_new;

                region = i;

                break;
            }
        }

        // 更新分区数据
        var filter = Builders<DataRegion>.Filter
            .Where(x => x._id == region.RegionGuid);
        var update = Builders<DataRegion>.Update
            .Set(x => x.RegionName, region.RegionName);
        await IMContext.Instance.Mongo.UpdateOneAsync(filter, StringDef.DbCollectionDataRegion, update);

        return region;
    }

    // 删除分区
    async Task<bool> IContainerStatefulIMRegionMgr.DeleteRegion(string region_guid)
    {
        var delete_result = await IMContext.Instance.Mongo.DeleteOneAsync<Region>(StringDef.DbCollectionDataRegion, region_guid);

        if (delete_result.DeletedCount > 0)
        {
            ListRegion.RemoveAll(x => x.RegionGuid == region_guid);

            return true;
        }

        return false;
    }

    // 请求分配一个分区
    async Task<Region> IContainerStatefulIMRegionMgr.RequestAssignReion()
    {
        if (ListRegion == null || ListRegion.Count == 0)
        {
            await ((IContainerStatefulIMRegionMgr)this).CreateRegion("default");
        }

        foreach (var region in ListRegion)
        {
            if (region.RegionId == DefaultRegionId)
            {
                return region;
            }
        }

        return ListRegion[0];
    }

    // 获取默认分区Id
    Task<int> IContainerStatefulIMRegionMgr.GetDefaultRegionId()
    {
        return Task.FromResult(DefaultRegionId);
    }

    // 设置默认分区Id，如果该分区Id不存在，则返回false
    async Task<bool> IContainerStatefulIMRegionMgr.SetDefaultRegionId(int region_id)
    {
        bool result = false;
        foreach (var region in ListRegion)
        {
            if (region.RegionId == region_id)
            {
                result = true;
                DefaultRegionId = region.RegionId;
                break;
            }
        }

        if (result)
        {
            string name_region_default = typeof(DataRegionDefault).Name;
            var data_region_default = new DataRegionDefault()
            {
                _id = name_region_default,
                RegionId = DefaultRegionId,
            };

            await IMContext.Instance.Mongo.ReplaceOneData(name_region_default, name_region_default, data_region_default);
        }

        return result;
    }

    // 定时器更新
    Task TimerUpdate()
    {
        float tm = (float)StopwatchUpdate.Elapsed.TotalSeconds;
        StopwatchUpdate.Restart();

        return Task.CompletedTask;
    }
}

#endif