#if DEF_CLIENT

using UnityEngine;

public class DelayDestroySelf : MonoBehaviour
{
    public bool EnableTimeout = false;
    public float TimeoutTm = 1.0f;
    public float MaxLiveTime;

    void Start()
    {
    }

    void Update()
    {
        if (EnableTimeout)
        {
            TimeoutTm -= Time.deltaTime;
            MaxLiveTime -= Time.deltaTime;
            if (TimeoutTm <= 0.0f || MaxLiveTime <= 0.0f)
            {
                DestroySelf();
            }
        }
    }

    public void Initialized(float delay, float max_time = 10)
    {
        EnableTimeout = true;
        TimeoutTm = delay;
        MaxLiveTime = max_time;
    }

    public void DestroySelf()
    {
        GameObject.Destroy(gameObject);
    }
}

#endif