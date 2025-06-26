#if DEF_CLIENT

using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    static CameraController _Instance;
    static public CameraController Instance()
    {
        return _Instance;
    }

    CinemachineVirtualCamera CinemachineVirtualCamera;
    CinemachineConfiner CinemachineConfiner;
    private Camera Camera;

    public BoxCollider LeftConfiner;
    public BoxCollider RightConfiner;
    private int Layer;

    void Awake()
    {
#if UNITY_2022_1_OR_NEWER
        CinemachineVirtualCamera = UnityEngine.GameObject.FindFirstObjectByType<CinemachineVirtualCamera>();
        CinemachineConfiner = UnityEngine.GameObject.FindFirstObjectByType<CinemachineConfiner>();
#else
        CinemachineVirtualCamera = UnityEngine.GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        CinemachineConfiner = UnityEngine.GameObject.FindObjectOfType<CinemachineConfiner>();
#endif
        _Instance = this;
        Camera = Camera.main;
        Layer = 1 << LayerMask.NameToLayer("CameraRaycast");
    }


    private Transform _toFollow;
    public void SetFollow(Transform toFollow)
    {
        _toFollow = toFollow;
        CinemachineVirtualCamera.Follow = toFollow;
        CinemachineVirtualCamera.LookAt = toFollow;
        UnityEngine.Debug.Log("toFollow.transform.position.z :" + toFollow.transform.position.z);
        if (toFollow.transform.position.z > 0)
        {
            //left
            CinemachineConfiner.m_BoundingVolume = LeftConfiner;
        }
        else
        {
            CinemachineConfiner.m_BoundingVolume = RightConfiner;
        }
    }

    public void Update()
    {
        if (_toFollow == null)
            return;

        if (_toFollow.transform.position.z < 200)
        {
            //left
            CinemachineConfiner.m_BoundingVolume = LeftConfiner;
        }
        else
        {
            CinemachineConfiner.m_BoundingVolume = RightConfiner;
        }
    }
    public Vector3 GetUserTouch()
    {
        Ray ray;
        if (Input.touchCount > 0)
        {
            Vector3 screen = Input.GetTouch(0).position;
            screen.z = 1f;
            ray = Camera.ScreenPointToRay(screen);
        }
        else
        {
            Vector3 screen = Input.mousePosition;
            screen.z = 1f;
            ray = Camera.ScreenPointToRay(screen);
        }

        if (Physics.Raycast(ray, out var hitInfo, 1000f, Layer))
        {
            return hitInfo.point;
        }
        return Vector3.zero;
    }
}

#endif