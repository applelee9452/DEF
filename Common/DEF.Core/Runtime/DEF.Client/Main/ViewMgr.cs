#if DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using YooAsset;

namespace DEF.Client
{
    public class ViewMgr
    {
        // 界面层级管理策略枚举
        public enum ViewLayerStrategy
        {
            None = 0,//不处理
            Destroy = 1,//打开某个界面时销毁上层界面，配合指定层级列表使用
            Stack = 2,//将上层界面入栈，后续恢复，配合指定层级列表使用
        }

        public struct LoadAssetData
        {
            public string LKey;
            public Task LTask;
            public AssetHandle LHandle;
        }

        static Dictionary<string, ViewFactory> MapViewFactory { get; set; } = new();
        static Dictionary<string, List<View>> MapView { get; set; } = new();
        static Dictionary<string, List<string>> MapViewData { get; set; } = new();// 显示队列中的View
        static Dictionary<string, GameObject> MapGameObject { get; set; } = new();// 保存通过LoadAsset方法异步加载到的需要缓存的资源
        public static int SafteAreaBottomY { get; set; }// 竖屏游戏兼容刘海屏，水平方向没有裁剪；Bottom UI距离屏幕下边沿的高度
        public static int SafteAreaTopY { get; set; }// top UI距离屏幕上边沿的高度
        public static Vector2 GoCanvasSize { get; set; }// 实际Canvas Size
        static EventContext EventContext { get; set; }
        public static Action<View> OnViewCreate { get; set; }
        public static Action<View> OnViewDestroy { get; set; }
        public static Action<string, GameObject, BaseEventData> OnMove { get; set; }
        public static Action<string, string, GameObject> OnButtonClick { get; set; }
        public static ViewLayerStrategy LayerStrategy { get; set; }
        public static List<string> ViewStrategyLayers { get; set; } = new();

        public ViewMgr(EventContext event_context)
        {
            EventContext = event_context;
        }

        // 设置层管理策略
        public void SetLayerStrategy(ViewLayerStrategy strategy, List<string> layers)
        {
            LayerStrategy = strategy;
            ViewStrategyLayers.Clear();
            if (layers != null)
            {
                ViewStrategyLayers.AddRange(layers);
            }
        }

        public void Create()
        {
            // 设置刘海屏安全区域
            //GoCanvasSize = GoCanvas2D.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
            //Rect safeArea = Screen.safeArea;
            //for (int i = 0; i < list_safe_layer.Count; i++)
            //{
            //    Vector2 anchorMin = safeArea.position;
            //    Vector2 anchorMax = safeArea.position + safeArea.size;
            //    SafteAreaBottomY = (int)anchorMin.y;
            //    SafteAreaTopY = (int)(Screen.height - anchorMax.y);

            //    var layer = MapLayer[list_safe_layer[i]];
            //    if (layer != null)
            //    {
            //        anchorMin.x /= Screen.width;
            //        anchorMin.y /= Screen.height;
            //        anchorMax.x /= Screen.width;
            //        anchorMax.y /= Screen.height;

            //        RectTransform rectTransform = layer.GetComponent<RectTransform>();
            //        rectTransform.anchorMin = anchorMin;
            //        rectTransform.anchorMax = anchorMax;
            //    }
            //}
        }

        // 预加载资源
        public static async Task PreLoadAsset(List<string> asset_list)
        {
            var load_list = GetNeedLoadAssets(asset_list);
            if (load_list.Count > 0)
            {
                List<LoadAssetData> handle_list = new();
                List<Task> task_list = new();
                foreach (var ll_name in load_list)
                {
                    var handle = YooAssets.LoadAssetAsync<GameObject>(ll_name);
                    handle_list.Add(new LoadAssetData { LKey = ll_name, LTask = handle.Task, LHandle = handle });
                    task_list.Add(handle.Task);
                }
                await Task.WhenAll(task_list);

                foreach (var d in handle_list)
                {
                    MapGameObject[d.LKey] = (GameObject)d.LHandle.AssetObject;
                }
            }
        }

        // 获取预加载的资源
        public static GameObject GetPreLoadedAsset(string key)
        {
            if (MapGameObject.ContainsKey(key))
                return MapGameObject[key];
            return null;
        }

        // 获取需要加载的资源列表
        public static List<string> GetNeedLoadAssets(List<string> asset_list)
        {
            List<string> itemList = new();
            foreach (var item in asset_list)
            {
                if (!MapGameObject.ContainsKey(item))
                {
                    itemList.Add(item);
                }
            }
            return itemList;
        }

        public void Destroy()
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.isLoaded)
            {
                foreach (var i in MapView)
                {
                    List<View> list_view = i.Value;
                    if (list_view != null && list_view.Count > 0)
                    {
                        foreach (var j in list_view)
                        {
                            j.IsValid = false;

                            j.Destory();

                            if (j.Go != null)
                            {
                                UnityEngine.Object.DestroyImmediate(j.Go);
                                j.Go = null;
                            }
                        }
                    }
                }
            }

            MapView.Clear();
            MapViewData.Clear();
            MapGameObject.Clear();
        }

        public static void RegViewFactory(ViewFactory factory)
        {
            //Debug.Log($"注册ViewFactory：{factory.GetName()}，Type={factory.GetType()}");

            MapViewFactory[factory.GetName()] = factory;
        }

        //public static Task<T> CreateView<T>(object obj = null, Dictionary<string, string> create_params = null) where T : View
        //{
        //    var name_class = typeof(T).Name;

        //    MapViewFactory.TryGetValue(name_class, out ViewFactory factory);
        //    if (factory == null)
        //    {
        //        string s = string.Format("ViewMgr.CreateView()失败！ 指定ViewName={0}没有注册！", name_class);
        //        Debug.LogError(s);

        //        return null;
        //    }

        //    return CreateView<T>(obj, create_params);
        //}

        public static T CreateView<T>(object obj = null, Dictionary<string, string> create_params = null, string parent = "") where T : View
        {
            var name_class = typeof(T).Name;

            MapViewFactory.TryGetValue(name_class, out ViewFactory factory);
            if (factory == null)
            {
                string s = string.Format("ViewMgr.CreateView()失败！ 指定ViewName={0}没有注册！", name_class);
                Debug.LogError(s);

                return null;
            }

            string name_view = factory.GetComponentName();

            // 单实例View不会重复创建
            bool is_multi_instance = factory.IsMultiInstance();
            MapView.TryGetValue(name_view, out var list_view1);
            if (!is_multi_instance && list_view1 != null && list_view1.Count > 0)
            {
                Debug.LogError($"单实例View不应该重复创建，ViewName={name_class}");
                return null;
            }

            if (!factory.DisableOpenAndCloseAudio())
            {
                //SoundMgr.Play("Assets/Arts/Sound/SfxUiOpen.wav", SoundLayer.LayerReplace);
            }

            //if (!YooAssets.IsInitialized)
            //{
            //    return null;
            //}

            GameObject go;
            // mark，此处开放后，嵌套创建Ui时，前一个为销毁结束即创建第二个时，第一个会把第二个（同一个Go）销毁掉
            //var transform_view = GoCanvas2D.transform.Find(factory.GetComponentName());
            //if (transform_view != null)
            //{
            //    go = transform_view.gameObject;
            //    if (!go.activeSelf)
            //    {
            //        go.SetActive(true);
            //    }
            //}
            //else
            {

                //创建的View数据到字典
                MapViewData.TryGetValue(name_view, out var list_view_data);
                if (list_view_data == null)
                {
                    list_view_data = new List<string>();
                    MapViewData[name_view] = list_view_data;
                }
                list_view_data.Add(name_view);

                // 创建的View显示到字典
                string key = "Assets/Arts/Ui/" + factory.GetComponentName();
                GameObject o1;
                if (MapGameObject.ContainsKey(key))
                {
                    o1 = MapGameObject[key];
                }
                else
                {
                    var handle = YooAssets.LoadAssetSync<GameObject>(key);
                    o1 = (GameObject)handle.AssetObject;
                }
                //var layer_name = factory.GetLayerName();

                //MapLayer.TryGetValue(layer_name, out var trans_layer);

                string parent_name = string.IsNullOrEmpty(parent) ? factory.GetParentName() : parent;
                DealLayerStrategy(parent_name);
                GameObject go_parent = GameObject.Find(parent_name);
                //if (trans_layer == null)
                //{
                //    trans_parent = GoCanvas2D.transform;
                //}
                //else
                //{
                //    trans_parent = trans_layer;
                //}
                go = GameObject.Instantiate(o1, go_parent.transform);
                go.name = name_view;
            }

            var view = factory.CreateView();
            view.Go = go;
            view.UGuiView = go.GetComponent<UGuiView>();
            view.Factory = factory;
            view.DicLanguage = new Dictionary<string, string>();
            view.IsValid = true;
            view.SetEventContext(EventContext);

            MapView.TryGetValue(name_view, out var list_view);
            if (list_view == null)
            {
                list_view = new List<View>();
                MapView[name_view] = list_view;
            }
            list_view.Add(view);

            view.Create(obj, create_params);
            OnViewCreate?.Invoke(view);
            return (T)view;
        }

        public static T CreateViewSync<T>(object obj = null, Dictionary<string, string> create_params = null, string parent = "") where T : View
        {
            var name_class = typeof(T).Name;

            MapViewFactory.TryGetValue(name_class, out ViewFactory factory);
            if (factory == null)
            {
                string s = string.Format("ViewMgr.CreateView()失败！ 指定ViewName={0}没有注册！", name_class);
                Debug.LogError(s);

                return null;
            }

            string name_view = factory.GetComponentName();

            // 单实例View不会重复创建
            bool is_multi_instance = factory.IsMultiInstance();
            MapView.TryGetValue(name_view, out var list_view1);
            if (!is_multi_instance && list_view1 != null && list_view1.Count > 0)
            {
                Debug.LogError($"单实例View不应该重复创建，ViewName={name_class}");
                return null;
            }

            if (!factory.DisableOpenAndCloseAudio())
            {
                //SoundMgr.Play("Assets/Arts/Sound/SfxUiOpen.wav", SoundLayer.LayerReplace);
            }

            //if (!YooAssets.IsInitialized)
            //{
            //    return null;
            //}

            GameObject go;
            // mark，此处开放后，嵌套创建Ui时，前一个为销毁结束即创建第二个时，第一个会把第二个（同一个Go）销毁掉
            //var transform_view = GoCanvas2D.transform.Find(factory.GetComponentName());
            //if (transform_view != null)
            //{
            //    go = transform_view.gameObject;
            //    if (!go.activeSelf)
            //    {
            //        go.SetActive(true);
            //    }
            //}
            //else
            {

                //创建的View数据到字典
                MapViewData.TryGetValue(name_view, out var list_view_data);
                if (list_view_data == null)
                {
                    list_view_data = new List<string>();
                    MapViewData[name_view] = list_view_data;
                }
                list_view_data.Add(name_view);

                // 创建的View显示到字典
                string key = "Assets/Arts/Ui/" + factory.GetComponentName();
                GameObject o1;
                if (MapGameObject.ContainsKey(key))
                {
                    o1 = MapGameObject[key];
                }
                else
                {
                    var handle = YooAssets.LoadAssetSync<GameObject>(key);
                    o1 = (GameObject)handle.AssetObject;
                }
                //var layer_name = factory.GetLayerName();

                //MapLayer.TryGetValue(layer_name, out var trans_layer);

                string parent_name = string.IsNullOrEmpty(parent) ? factory.GetParentName() : parent;
                DealLayerStrategy(parent_name);
                GameObject go_parent = GameObject.Find(parent_name);
                //if (trans_layer == null)
                //{
                //    trans_parent = GoCanvas2D.transform;
                //}
                //else
                //{
                //    trans_parent = trans_layer;
                //}

                go = GameObject.Instantiate(o1, go_parent.transform);
                go.name = name_view;
            }

            var view = factory.CreateView();
            view.Go = go;
            view.UGuiView = go.GetComponent<UGuiView>();
            view.Factory = factory;
            view.DicLanguage = new Dictionary<string, string>();
            view.IsValid = true;
            view.SetEventContext(EventContext);

            MapView.TryGetValue(name_view, out var list_view);
            if (list_view == null)
            {
                list_view = new List<View>();
                MapView[name_view] = list_view;
            }
            list_view.Add(view);

            view.Create(obj, create_params);
            OnViewCreate?.Invoke(view);
            return (T)view;
        }

        // 获取首个视图实例
        public static T GetView<T>() where T : View
        {
            var name_class = typeof(T).Name;

            MapViewFactory.TryGetValue(name_class, out ViewFactory factory);
            if (factory == null)
            {
                return null;
            }

            string name_view = factory.GetComponentName();

            MapView.TryGetValue(name_view, out var list_view);
            if (list_view != null && list_view.Count > 0)
            {
                return (T)list_view[0];
            }

            return null;
        }

        public static T HideView<T>() where T : View
        {
            var name_class = typeof(T).Name;

            MapViewFactory.TryGetValue(name_class, out ViewFactory factory);
            if (factory == null)
            {
                return null;
            }

            string name_view = factory.GetComponentName();

            MapView.TryGetValue(name_view, out var list_view);
            if (list_view != null && list_view.Count > 0)
            {
                var v = (T)list_view[0];
                v.Go.SetActive(false);
                return v;
            }

            return null;
        }

        public static T ShowView<T>() where T : View
        {
            var name_class = typeof(T).Name;

            MapViewFactory.TryGetValue(name_class, out ViewFactory factory);
            if (factory == null)
            {
                return null;
            }

            string name_view = factory.GetComponentName();

            MapView.TryGetValue(name_view, out var list_view);
            if (list_view != null && list_view.Count > 0)
            {
                var v = (T)list_view[0];
                v.Go.SetActive(true);
                return v;
            }

            return null;
        }

        public static T GetOrCreateView<T>(object obj = null, Dictionary<string, string> create_params = null) where T : View
        {
            var view = GetView<T>();

            if (view != null)
            {
                return view;
            }

            return CreateView<T>(obj, create_params);
        }

        //public static T GetOrCreateViewSync<T>(object obj = null, Dictionary<string, string> create_params = null) where T : View
        //{
        //    var view = GetView<T>();

        //    if (view != null)
        //    {
        //        return view;
        //    }

        //    return CreateViewSync<T>(obj, create_params);
        //}

        //public static T CreateNewView<T>(object obj = null, Dictionary<string, string> create_params = null) where T : View
        //{
        //    var view = GetView<T>();
        //    if (view != null)
        //    {
        //        ViewMgr.DestroyView(view);
        //    }

        //    return GetOrCreateView<T>(obj, create_params);
        //}

        //// 创建一个新的试图，如果存在旧的会先关闭
        //public static T CreateNewViewSync<T>(object obj = null, Dictionary<string, string> create_params = null) where T : View
        //{
        //    var view = GetView<T>();
        //    if (view != null)
        //    {
        //        ViewMgr.DestroyView(view);
        //    }

        //    return CreateViewSync<T>(obj, create_params);
        //}

        // 销毁首个视图实例
        public static void DestroyView<T>() where T : View
        {
            var name_class = typeof(T).Name;

            DestoryView(name_class, null);
        }

        // 销毁指定视图实例
        public static void DestroyView(View view)
        {
            var name_class = view.GetType().Name;

            DestoryView(name_class, view);
        }

        // 销毁指定视图实例
        static void DestoryView(string name_class, View instance)
        {
            MapViewFactory.TryGetValue(name_class, out ViewFactory factory);
            if (factory == null)
            {
                return;
            }

            if (!factory.DisableOpenAndCloseAudio())
            {
                //SoundMgr.Play("Assets/Arts/Sound/SfxUiClose.wav", SoundLayer.LayerReplace);
            }

            if (factory.IsMultiInstance() && instance == null)
            {
                Debug.LogError($"多实例View销毁时应提供实例引用，ViewName={name_class}");
            }

            // 删除view数据
            string name_view = factory.GetComponentName();
            MapViewData.TryGetValue(name_view, out List<string> list_view_data);
            if (list_view_data != null && list_view_data.Count > 0)
            {
                list_view_data.RemoveAt(0);
            }

            // 删除view显示
            MapView.TryGetValue(name_view, out List<View> list_view);
            if (list_view == null || list_view.Count == 0)
            {
                return;
            }

            View view = instance;
            if (view == null)
            {
                view = list_view[0];
            }

            if (view != null)
            {
                OnViewDestroy?.Invoke(view);

                view.IsValid = false;
                view.Destory();
                list_view.Remove(view);

                if (view.Go != null)
                {
                    UnityEngine.Object.DestroyImmediate(view.Go);
                    view.Go = null;
                }
            }
        }

        // 销毁所有指定视图类型的实例
        public static void DestroyAllViewByType<T>() where T : View
        {
            var name_class = typeof(T).Name;

            DestoryView(name_class, null);
        }

        public static void DestroyAllView()
        {
            foreach (var i in MapView)
            {
                List<View> list_view = i.Value;
                if (list_view != null && list_view.Count > 0)
                {
                    foreach (var j in list_view)
                    {
                        j.IsValid = false;
                        j.Destory();

                        if (j.Go != null)
                        {
                            UnityEngine.Object.Destroy(j.Go);
                            j.Go = null;
                        }
                    }
                }
            }
            MapView.Clear();
            MapViewData.Clear();
        }

        public static void ListenEvent<T>(DEF.EventListener listener) where T : DEF.Event
        {
            EventContext.ListenEvent<T>(listener);
        }

        public static void UnListenAllEvent(DEF.EventListener listener)
        {
            EventContext.UnListenAllEvent(listener);
        }

        public static T GenEvent<T>() where T : DEF.Event, new()
        {
            return EventContext.GenEvent<T>();
        }

        public static void OnUGuiCreateCom(GameObject view, GameObject obj, string script_com_name)
        {
            MapView.TryGetValue(view.name, out var ui);
            if (ui == null || ui.Count == 0)
            {
                Debug.LogWarningFormat("ViewMgr2.OnUGuiCreateCom View==null ViewName={0} ButtonName={1}", view.name, obj.name);
                return;
            }

            ui[0].OnUGuiCreateCom(view, obj, script_com_name);
        }

        public static void OnUGuiMove(string view_name, GameObject go, BaseEventData data)
        {
            MapView.TryGetValue(view_name, out var view);
            if (view == null || view.Count == 0)
            {
                Debug.LogWarningFormat("ViewMgr2.OnUGuiButtonClick View==null ViewName={0} GoName={1}", view_name, go.name);
                return;
            }

            OnMove?.Invoke(view_name, go, data);

            view[0].OnUGuiMove(go, data);
        }

        public static void OnUGuiButtonClick(string view_name, string button_name, GameObject go_button)
        {
            MapView.TryGetValue(view_name, out var view);
            if (view == null || view.Count == 0)
            {
                Debug.LogWarningFormat("ViewMgr2.OnUGuiButtonClick View==null ViewName={0} ButtonName={1}", view_name, button_name);
                return;
            }

            OnButtonClick?.Invoke(view_name, button_name, go_button);

            view[0].OnUGuiButtonClick(button_name, go_button);
        }

        public static void OnUGuiSliderValueChange(string view_name, string slider_name, float value)
        {
            MapView.TryGetValue(view_name, out var view);
            if (view == null || view.Count == 0)
            {
                Debug.LogWarningFormat("ViewMgr2.OnUGuiSliderValueChange View==null ViewName={0} SliderName={1}", view_name, slider_name);
                return;
            }

            view[0].OnUGuiSliderValueChange(slider_name, value);
        }

        public static void OnUGuiMsg(GameObject go, string msg)
        {
            MapView.TryGetValue(go.name, out var view);
            if (view == null || view.Count == 0)
            {
                Debug.LogWarningFormat("ViewMgr2.OnUGuiMsg View==null ViewName={0}", go.name);
                return;
            }

            view[0].OnUGuiMsg(go, msg);
        }

        public static int GetViewCount()
        {
            int view_count = 0;

            foreach (var i in MapView)
            {
                if (i.Value == null || i.Value.Count == 0) continue;

                view_count++;
            }

            return view_count;
        }

        public static bool IsViewVisible(string view_name)
        {
            return MapView.ContainsKey(view_name);
        }

        static List<View> GetViewsByLayer(string layer)
        {
            List<View> views = new List<View>();
            foreach (var pair in MapView)
            {
                foreach (var view in pair.Value)
                {
                    var name_class = view.GetType().Name;
                    if (MapViewFactory.TryGetValue(name_class, out var factory))
                    {
                        if (factory.GetParentName() == layer)
                        {
                            views.Add(view);
                        }
                    }
                }
            }
            return views;
        }

        // 处理层级管理策略
        static void DealLayerStrategy(string open_layer)
        {
            if (LayerStrategy == ViewLayerStrategy.None)
            {
                return;
            }
            if (LayerStrategy == ViewLayerStrategy.Stack)
            {
                return;
            }
            if (LayerStrategy == ViewLayerStrategy.Destroy)
            {
                int index = ViewStrategyLayers.IndexOf(open_layer);
                if (index == -1)
                {
                    return;
                }
                index++;
                if (index >= ViewStrategyLayers.Count)
                {
                    return;
                }
                for (int i = index; i < ViewStrategyLayers.Count; i++)
                {
                    var views = GetViewsByLayer(ViewStrategyLayers[i]);
                    if (views.Count > 0)
                    {
                        foreach (View view in views)
                        {
                            Debug.Log($"[ViewMgr]层级管理策略关闭界面{view.GetType().Name}");
                            DestroyView(view);
                        }
                    }
                }
            }
        }
    }
}

#endif