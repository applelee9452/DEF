using MongoDB.Driver;

namespace DEF.CCenter;

public class ContainerStatelessManager : ContainerStateless, IContainerManager
{
    DbClientMongo Db;

    public override Task OnCreate()
    {
        Db = CCenterContext.Instance.Mongo;

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    async Task<List<DataNameSpace>> IContainerManager.GetAllNameSpace()
    {
        var list_namespace = await Db.ReadListAsync<DataNameSpace>(
            e => !string.IsNullOrEmpty(e._id),
            StringDef.DbCollectionNameSpace);

        return list_namespace;
    }

    async Task<DataNameSpace> IContainerManager.AddNameSpace(string name_space, string desc)
    {
        var ns = await Db.ReadAsync<DataNameSpace>(
            a => a.NameSpace == name_space,
            StringDef.DbCollectionNameSpace);
        if (ns != null)
        {
            return ns;
        }

        ns = new DataNameSpace()
        {
            _id = Guid.NewGuid().ToString(),
            NameSpace = name_space,
            Desc = desc
        };

        await Db.InsertAsync(StringDef.DbCollectionNameSpace, ns);

        var cfg = new DataCfg()
        {
            _id = ns._id,
            MapCfg = []
        };

        await Db.InsertAsync(StringDef.DbCollectionCfg, cfg);

        return ns;
    }

    async Task<bool> IContainerManager.RemoveNameSpace(string _id)
    {
        await Db.DeleteOneAsync<DataNameSpace>(StringDef.DbCollectionNameSpace, _id);

        await Db.DeleteOneAsync<DataCfg>(StringDef.DbCollectionCfg, _id);

        return true;
    }

    async Task<DataNameSpace> IContainerManager.UpdateNameSpace(string _id, string name_space_new, string desc_new)
    {
        var ns = await Db.ReadAsync<DataNameSpace>(
            a => a._id == _id,
            StringDef.DbCollectionNameSpace);
        if (ns == null)
        {
            return null;
        }

        ns.NameSpace = name_space_new;
        ns.Desc = desc_new;

        var filter = Builders<DataNameSpace>.Filter
                    .Where(x => x._id == _id);
        var update = Builders<DataNameSpace>.Update
            .Set(x => x.NameSpace, name_space_new)
            .Set(x => x.Desc, desc_new);
        await Db.UpdateOneAsync(filter, StringDef.DbCollectionNameSpace, update);

        return ns;
    }

    async Task<List<DataCfg>> IContainerManager.GetAllCfg()
    {
        var list_cfg = await Db.ReadListAsync<DataCfg>(
            e => !string.IsNullOrEmpty(e._id),
            StringDef.DbCollectionCfg);

        return list_cfg;
    }

    async Task<DataCfg> IContainerManager.AddCfgItem(string _id, string key, string value)
    {
        var cfg = await Db.ReadAsync<DataCfg>(
            a => a._id == _id,
            StringDef.DbCollectionCfg);
        if (cfg == null)
        {
            return null;
        }

        cfg.MapCfg[key] = value;

        var filter = Builders<DataCfg>.Filter
                    .Where(x => x._id == _id);
        var update = Builders<DataCfg>.Update
            .Set(x => x.MapCfg, cfg.MapCfg);
        await Db.UpdateOneAsync(filter, StringDef.DbCollectionCfg, update);

        return cfg;
    }

    async Task<DataCfg> IContainerManager.RemoveCfgItem(string _id, string key)
    {
        var cfg = await Db.ReadAsync<DataCfg>(
            a => a._id == _id,
            StringDef.DbCollectionCfg);
        if (cfg == null)
        {
            return null;
        }

        cfg.MapCfg.Remove(key);

        var filter = Builders<DataCfg>.Filter
                    .Where(x => x._id == _id);
        var update = Builders<DataCfg>.Update
            .Set(x => x.MapCfg, cfg.MapCfg);
        await Db.UpdateOneAsync(filter, StringDef.DbCollectionCfg, update);

        return cfg;
    }

    async Task<DataCfg> IContainerManager.UpdateCfgItem(string _id, string key, string value)
    {
        var cfg = await Db.ReadAsync<DataCfg>(
            a => a._id == _id,
            StringDef.DbCollectionCfg);
        if (cfg == null)
        {
            return null;
        }

        // 如果Key相同，覆盖，如果Key不同，添加。
        cfg.MapCfg[key] = value;

        var filter = Builders<DataCfg>.Filter
                    .Where(x => x._id == _id);
        var update = Builders<DataCfg>.Update
            .Set(x => x.MapCfg, cfg.MapCfg);
        await Db.UpdateOneAsync(filter, StringDef.DbCollectionCfg, update);

        return cfg;
    }

    async Task<ImportExportCfgs> IContainerManager.ExportAllCfg()
    {
        ImportExportCfgs cfgs = new()
        {
            List = [],
        };

        var list_cfg = await Db.ReadListAsync<DataCfg>(
            e => !string.IsNullOrEmpty(e._id),
            StringDef.DbCollectionCfg);

        var list_namespace = await Db.ReadListAsync<DataNameSpace>(
            e => !string.IsNullOrEmpty(e._id),
            StringDef.DbCollectionNameSpace);
        if (list_namespace != null && list_namespace.Count > 0)
        {
            foreach (var i in list_namespace)
            {
                ImportExportCfg cfg = new()
                {
                    NameSpace = i.NameSpace,
                    Desc = i.Desc,
                };

                if (list_cfg != null && list_cfg.Count > 0)
                {
                    foreach (var j in list_cfg)
                    {
                        if (i._id == j._id)
                        {
                            cfg.MapCfg = j.MapCfg;
                            break;
                        }
                    }
                }

                cfgs.List.Add(cfg);
            }
        }

        return cfgs;
    }

    async Task IContainerManager.ImportAllCfg(ImportExportCfgs cfgs)
    {
        if (cfgs == null || cfgs.List == null || cfgs.List.Count == 0) return;

        foreach (var i in cfgs.List)
        {
            var ns = await ((IContainerManager)(this)).AddNameSpace(i.NameSpace, i.Desc);

            var cfg = await Db.ReadAsync<DataCfg>(
                a => a._id == ns._id,
                StringDef.DbCollectionCfg);

            cfg ??= new()
            {
                _id = ns._id,
            };

            cfg.MapCfg = i.MapCfg;

            var filter = Builders<DataCfg>.Filter
                        .Where(x => x._id == ns._id);
            var update = Builders<DataCfg>.Update
                .Set(x => x.MapCfg, cfg.MapCfg);
            await Db.UpdateOneAsync(filter, StringDef.DbCollectionCfg, update);
        }
    }
}