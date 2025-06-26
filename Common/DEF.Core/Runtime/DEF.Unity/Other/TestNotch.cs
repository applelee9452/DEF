#if DEF_CLIENT

using UnityEngine;

public class TestNotch : MonoBehaviour
{
    public RectTransform RectTransform = null;
    public int notchWidth = 100;

    void Start()
    {
    }

    void Update()
    {
        //Device Simulator
        //Screen.safeArea

        var anchorMin = RectTransform.anchorMin;
        anchorMin.x = notchWidth / 1080f;
        RectTransform.anchorMin = anchorMin;

        Canvas.ForceUpdateCanvases();
    }
}

#endif