#if DEF_CLIENT

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiLaunch : MonoBehaviour
{
    public RawImage Bg;
    public TextMeshProUGUI TextVersion;
    public TextMeshProUGUI TextDesc;
    public TextMeshProUGUI TextInfo;
    public Transform Age;
    public Image ImgWaiting;
    public Slider Progress;
    TweenerCore<Quaternion, Vector3, QuaternionOptions> Tweener { get; set; }

    void Start()
    {
        SetTexInfoVisible(false);

        SetWaiting(true);
    }

    void OnDestroy()
    {
        if (Tweener != null)
        {
            Tweener.Kill();
            Tweener = null;
        }
    }

    // 刷新版本信息
    public void RefreshVersionInfo()
    {
        string cfg_version = PlayerPrefs.GetString("CfgVersion");

        if (YooAssetWrapper.AssetsPackage == null)
        {
            TextVersion.text = $"版本号 {Client.GetBundleVersion()}, 配置 {cfg_version}";
        }
        else
        {
            //TextVersion.text = $"版本号 {Client.GetBundleVersion()}, 资源版本 {YooAssetWrapper.AssetsPackage.GetPackageVersion()}, 配置 {cfg_version}";
            TextVersion.text = $"版本号 {Client.GetBundleVersion()}";
        }

        if (Client.Updater.TryGetCfgCenter("Age", out var age))
        {
            var w = Age?.Find($"{age}")?.gameObject;
            w?.SetActive(true);
        }
    }

    // 更新进度条上方提示文字
    public void UpdateDesc(string desc)
    {
        TextDesc.text = desc;
    }

    // 更新进度条
    public void UpdateLoadingProgress(int value, int max)
    {
        Progress.value = (float)value / max;
    }

    public void SetLoadingVisible(bool value)
    {
        Progress.gameObject.SetActive(value);
    }

    public string GetLanguageString(string key)
    {
        return key;
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void SetTexInfoVisible(bool visible)
    {
        TextInfo?.gameObject.SetActive(visible);
    }

    public void SetProgressVisable(bool visable)
    {
        Progress.gameObject.SetActive(visable);
        TextDesc.gameObject.SetActive(visable);
    }

    public void SetWaiting(bool visable)
    {
        if (ImgWaiting == null) return;

        ImgWaiting?.gameObject.SetActive(visable);

        if (visable)
        {
            if (Tweener == null)
            {
                Tweener = ImgWaiting.transform
                    .DOLocalRotate(new Vector3(0, 0, -360), 2.0f, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Restart);
            }
        }
        else
        {
            if (Tweener != null)
            {
                Tweener.Kill();
                Tweener = null;
            }
        }
    }
}

#endif