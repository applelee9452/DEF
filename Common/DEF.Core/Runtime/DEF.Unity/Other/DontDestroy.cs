#if DEF_CLIENT

using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}

#endif