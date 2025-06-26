#if DEF_CLIENT

using DEF;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DanMu
{
    [ComponentImpl]
    public class ComUi : ComponentLocal
    {
        public override void Awake(Dictionary<string, object> create_params)
        {
            // todo，加载Prefab

            // 获取UGuidComponent

            // 设置Ui层级关系

            Debug.Log("ComUi.Awake()");
        }

        public override void OnStart()
        {
            Debug.Log("ComUi.OnStart()");
        }

        public override void OnDestroy(string reason = null, byte[] user_data = null)
        {
            Debug.Log("ComUi.OnStart()");
        }

        public override void HandleSelfEvent(DEF.SelfEvent ev)
        {
        }

        public override void HandleEvent(DEF.Event ev)
        {
        }

        public void LoadPrefabSync(string prefab_path)
        {
        }

        public Task LoadPrefabAsync(string prefab_path)
        {
            return Task.CompletedTask;
        }
    }
}

#endif