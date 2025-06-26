//using Microsoft.Extensions.Logging;
//using Rabbit.Zookeeper;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace DEF;

//public class ServiceDiscoverZkWatcher
//{
//    ILogger Logger { get; set; }
//    IZookeeperClient Client { get; set; }
//    ServiceDiscover ServiceDiscover { get; set; }
//    List<string> ServiceDiscoverListenServices { get; set; } = [];
//    List<string> LastChildren { get; set; } = [];

//    public ServiceDiscoverZkWatcher(ServiceDiscover service_discover, ILogger logger, IZookeeperClient zk_client)
//    {
//        ServiceDiscover = service_discover;
//        Logger = logger;
//        Client = zk_client;

//        foreach (var i in ServiceDiscover.ServiceClientOptions.Value.ServiceDiscoverListenServices)
//        {
//            ServiceDiscoverListenServices.Add(i.ServiceName);
//        }
//    }

//    public async Task StartAsync()
//    {
//        if (ServiceDiscoverListenServices.Count == 0) return;

//        for (int k = 0; k < 3; k++)
//        {
//            // 订阅Children变更

//            await Client.SubscribeChildrenChange(ServiceDiscover.ClusterName, NodeChildrenChangeHandler);

//            // 首次启动需要先获取一遍所有关注的ServerNode信息

//            var list_service = await GetAllServiceNode();

//            List<string> list_add = [];
//            foreach (var i in list_service)
//            {
//                if (!ServiceDiscoverListenServices.Contains(i)) continue;

//                if (!LastChildren.Contains(i))
//                {
//                    list_add.Add(i);
//                }
//            }
//            List<string> list_remove = [];
//            foreach (var i in LastChildren)
//            {
//                if (!ServiceDiscoverListenServices.Contains(i)) continue;

//                if (!list_service.Contains(i))
//                {
//                    list_remove.Add(i);
//                }
//            }

//            LastChildren.Clear();
//            LastChildren.AddRange(list_service);

//            foreach (var i in list_add)
//            {
//                await ServiceDiscover.ActionServiceNodeChanged?.Invoke(i, true);
//            }

//            foreach (var i in list_remove)
//            {
//                await ServiceDiscover.ActionServiceNodeChanged?.Invoke(i, false);
//            }
//        }

//    }

//    public Task StopAsync()
//    {
//        if (ServiceDiscoverListenServices.Count == 0) return Task.CompletedTask;

//        for (int k = 0; k < 3; k++)
//        {
//            Client.UnSubscribeChildrenChange(ServiceDiscover.ClusterName, NodeChildrenChangeHandler);
//        }

//        return Task.CompletedTask;
//    }

//    async Task NodeChildrenChangeHandler(IZookeeperClient client, NodeChildrenChangeArgs args)
//    {
//        switch (args.Type)
//        {
//            case org.apache.zookeeper.Watcher.Event.EventType.None:
//                break;
//            case org.apache.zookeeper.Watcher.Event.EventType.NodeDataChanged:
//                break;
//            case org.apache.zookeeper.Watcher.Event.EventType.NodeCreated:
//                break;
//            case org.apache.zookeeper.Watcher.Event.EventType.NodeDeleted:
//                break;
//            case org.apache.zookeeper.Watcher.Event.EventType.NodeChildrenChanged:
//                List<string> list_add = [];
//                foreach (var i in args.CurrentChildrens)
//                {
//                    //Console.WriteLine($"   {i}");

//                    if (!ServiceDiscoverListenServices.Contains(i)) continue;

//                    if (!LastChildren.Contains(i))
//                    {
//                        list_add.Add(i);
//                    }
//                }
//                List<string> list_remove = [];
//                foreach (var i in LastChildren)
//                {
//                    if (!ServiceDiscoverListenServices.Contains(i)) continue;

//                    if (!args.CurrentChildrens.Contains(i))
//                    {
//                        list_remove.Add(i);
//                    }
//                }

//                LastChildren.Clear();
//                LastChildren.AddRange(args.CurrentChildrens);

//                foreach (var i in list_add)
//                {
//                    await ServiceDiscover.ActionServiceNodeChanged?.Invoke(i, true);
//                }

//                foreach (var i in list_remove)
//                {
//                    await ServiceDiscover.ActionServiceNodeChanged?.Invoke(i, false);
//                }

//                break;
//            default:
//                break;
//        }
//    }

//    async Task<List<string>> GetAllServiceNode()
//    {
//        IEnumerable<string> children = null;

//        try
//        {
//            children = await Client.GetChildrenAsync(ServiceDiscover.ClusterName);
//        }
//        catch (Exception)
//        {
//        }

//        List<string> list_service = [];
//        if (children != null && children.Count() > 0)
//        {
//            list_service.AddRange(children);
//        }

//        return list_service;
//    }

//    // 定时同步自身负载（连接数），该函数运行在不同的线程中
//    //async void DoWork(object state)
//    //{
//    //    try
//    //    {
//    //        await Check();
//    //    }
//    //    catch (Exception)
//    //    {
//    //        //Logger.LogError(e, "更新Zk数据失败！");
//    //    }
//    //}

//    //async Task Check()
//    //{
//    //    IEnumerable<string> children = null;

//    //    try
//    //    {
//    //        children = await Client.GetChildrenAsync(string.Format("{0}/{1}", ServiceDiscover.ClusterName, ServiceName));
//    //    }
//    //    catch (Exception)
//    //    {
//    //    }

//    //    if (children != null && children.Count() > 0)
//    //    {
//    //        if (!ExistChildren)
//    //        {
//    //            ExistChildren = true;

//    //            ActionServiceChange?.Invoke(ServiceName, ExistChildren);
//    //        }
//    //    }
//    //    else
//    //    {
//    //        if (ExistChildren)
//    //        {
//    //            ExistChildren = false;

//    //            ActionServiceChange?.Invoke(ServiceName, ExistChildren);
//    //        }
//    //    }
//    //}
//}
