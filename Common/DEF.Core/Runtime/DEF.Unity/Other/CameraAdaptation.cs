#if DEF_CLIENT

using Cinemachine;
using UnityEngine;

public class CameraAdaptation : MonoBehaviour
{
    const float initOrthoSize = 9.6f;
    const float initWidth = 1080;
    const float initHeight = 1920;

    float factWidth;
    float factHeight;

    void Start()
    {
        factWidth = Screen.width;
        factHeight = Screen.height;

        // 实际正交视口 = 初始正交视口 * 初始宽高比 / 实际宽高比
        var c = GetComponent<CinemachineVirtualCamera>();
        c.m_Lens.OrthographicSize = (initOrthoSize * (initWidth / initHeight)) / (factWidth / factHeight);
    }
}

#endif