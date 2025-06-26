#if DEF_CLIENT

using UnityEngine;

public class ParticleAutoInactive : MonoBehaviour
{
    ParticleSystem[] ParticleSystems;

    void Start()
    {
        ParticleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        bool all_stopped = true;

        foreach (ParticleSystem ps in ParticleSystems)
        {
            if (!ps.isStopped)
            {
                all_stopped = false;
            }
        }

        if (all_stopped)
        {
            gameObject.SetActive(false);
        }
    }
}

#endif