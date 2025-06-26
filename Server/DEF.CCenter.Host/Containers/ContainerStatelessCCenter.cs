using Microsoft.Extensions.Logging;

namespace DEF.CCenter;

public class ContainerStatelessCCenter : ContainerStateless, IContainerStatelessCCenter
{
    DbClientMongo Db { get; set; }

    public override Task OnCreate()
    {
        Db = CCenterContext.Instance.Mongo;

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    async Task<Dictionary<string, string>> IContainerStatelessCCenter.GetCfg(string list_ns, string bundle_version)
    {
        Logger.LogInformation("ContainerStatelessCCenter.GetCfg() ListNameSpace={list_ns}", list_ns);

        Dictionary<string, string> m = [];

        var arr_ns = list_ns.Split('|');

        string updatebundle_anybundle_key = string.Empty;
        string updatebundle_anybundle_value = string.Empty;
        int updatebundle_anybundle_priority = -10000;

        string updatebundle_bundle_key = string.Empty;
        string updatebundle_bundle_value = string.Empty;
        int updatebundle_bundle_priority = -10000;

        string updatedata_anybundle_key = string.Empty;
        string updatedata_anybundle_value = string.Empty;
        int updatedata_anybundle_priority = -10000;

        string updatedata_bundle_key = string.Empty;
        string updatedata_bundle_value = string.Empty;
        int updatedata_bundle_priority = -10000;

        foreach (var s in arr_ns)
        {
            var ns = await Db.ReadAsync<DataNameSpace>(a => a.NameSpace == s, StringDef.DbCollectionNameSpace);
            if (ns == null)
            {
                continue;
            }

            var cfg = await Db.ReadAsync<DataCfg>(a => a._id == ns._id, StringDef.DbCollectionCfg);
            if (cfg == null)
            {
                continue;
            }

            foreach (var i in cfg.MapCfg)
            {
                if (i.Key.StartsWith("UpdateBundle_anybundle"))
                {
                    updatebundle_anybundle_key = i.Key;
                    updatebundle_anybundle_value = i.Value;

                    var arr = i.Value.Split('_');
                    int priority = int.Parse(arr[0]);
                    updatebundle_anybundle_priority = priority;
                }
                else if (i.Key.StartsWith("UpdateBundle_"))
                {
                    string bundle_current = string.Empty;
                    {
                        var arr = i.Key.Split('_');
                        bundle_current = arr[1];
                    }

                    if (bundle_current != bundle_version)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(updatebundle_bundle_key))
                    {
                        updatebundle_bundle_key = i.Key;
                    }

                    {
                        var arr = i.Value.Split('_');
                        int priority = int.Parse(arr[0]);
                        //string action = arr[1];// keep, disable, update

                        if (priority > updatebundle_bundle_priority)
                        {
                            updatebundle_bundle_value = i.Value;
                            updatebundle_bundle_priority = priority;
                        }
                    }
                }
                else if (i.Key.StartsWith("UpdateData_anybundle"))
                {
                    updatedata_anybundle_key = i.Key;
                    updatedata_anybundle_value = i.Value;

                    var arr = i.Value.Split('_');
                    int priority = int.Parse(arr[0]);
                    updatedata_anybundle_priority = priority;
                }
                else if (i.Key.StartsWith("UpdateData_"))
                {
                    string bundle_current = string.Empty;
                    {
                        var arr = i.Key.Split('_');
                        bundle_current = arr[1];
                    }

                    if (bundle_current != bundle_version)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(updatedata_bundle_key))
                    {
                        updatedata_bundle_key = i.Key;
                    }

                    {
                        var arr = i.Value.Split('_');
                        int priority = int.Parse(arr[0]);
                        //string action = arr[1];// keep, disable, update

                        if (priority > updatedata_bundle_priority)
                        {
                            updatedata_bundle_value = i.Value;
                            updatedata_bundle_priority = priority;
                        }
                    }
                }
                else
                {
                    m[i.Key] = i.Value;
                }
            }
        }

        // 追加UpdateBundle更新信息
        if (!string.IsNullOrEmpty(updatebundle_anybundle_key) && !string.IsNullOrEmpty(updatebundle_bundle_key))
        {
            if (updatebundle_bundle_priority > updatebundle_anybundle_priority)
            {
                m[updatebundle_bundle_key] = updatebundle_bundle_value;
            }
            else
            {
                m[updatebundle_anybundle_key] = updatebundle_anybundle_value;
            }
        }
        else if (!string.IsNullOrEmpty(updatebundle_anybundle_key))
        {
            m[updatebundle_anybundle_key] = updatebundle_anybundle_value;
        }
        else if (!string.IsNullOrEmpty(updatebundle_bundle_key))
        {
            m[updatebundle_bundle_key] = updatebundle_bundle_value;
        }

        // 追加UpdateData更新信息
        if (!string.IsNullOrEmpty(updatedata_anybundle_key) && !string.IsNullOrEmpty(updatedata_bundle_key))
        {
            if (updatedata_bundle_priority > updatedata_anybundle_priority)
            {
                m[updatedata_bundle_key] = updatedata_bundle_value;
            }
            else
            {
                m[updatedata_anybundle_key] = updatedata_anybundle_value;
            }
        }
        else if (!string.IsNullOrEmpty(updatedata_anybundle_key))
        {
            m[updatedata_anybundle_key] = updatedata_anybundle_value;
        }
        else if (!string.IsNullOrEmpty(updatedata_bundle_key))
        {
            m[updatedata_bundle_key] = updatedata_bundle_value;
        }

        return m;
    }
}