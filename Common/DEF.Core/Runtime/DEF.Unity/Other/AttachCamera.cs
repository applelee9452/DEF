#if DEF_CLIENT

using UnityEngine;

public class AttachCamera : MonoBehaviour
{
    void Awake()
    {
        var canvas = gameObject.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }
}

#endif