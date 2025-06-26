#if DEF_CLIENT

using UnityEngine;

public class UpdateState7LoadDll : SimpleState2
{
    Updater Updater { get; set; }
    UiLaunch UiLaunch { get; set; }

    public UpdateState7LoadDll(Updater updater)
    {
        Updater = updater;
        UiLaunch = Updater.UiLaunch;
    }

    public override string GetName()
    {
        return "LoadDll";
    }

    public override async void Enter()
    {
        Debug.Log("UpdateState7LoadDll.Enter()");

        UiLaunch?.UpdateDesc("准备进入游戏");
        UiLaunch?.RefreshVersionInfo();
        UiLaunch?.SetLoadingVisible(false);

        if (Updater.UpdaterMode == UpdaterMode.None)
        {
            // 更新并加载指定版本的资源清单
            var op_updatepackage_manifest = YooAssetWrapper.AssetsPackage.UpdatePackageManifestAsync("Simulate");
            await op_updatepackage_manifest.Task;
            if (op_updatepackage_manifest.Status != YooAsset.EOperationStatus.Succeed)
            {
                Debug.LogError($"YooAsset.UpdatePackageManifestAsync Error: {op_updatepackage_manifest.Error}");
            }
        }

        await Client.LoadDll();

        Client.RemoveTickableAndDispose(Updater);
    }

    public override void Exit()
    {
        Debug.Log("UpdateState7LoadDll.Exit()");
    }

    public override string Update(float tm)
    {
        return string.Empty;
    }

    public override string OnEvent(string ev_name, string ev_param)
    {
        return string.Empty;
    }

    public override string OnEvent(string ev_name, object ev_param)
    {
        return string.Empty;
    }
}

#endif