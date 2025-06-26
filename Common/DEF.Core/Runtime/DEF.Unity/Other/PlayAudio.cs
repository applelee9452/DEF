#if DEF_CLIENT

using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public AudioClip audioClip;
    public bool playWhenEnabled = true;

    void OnEnable()
    {
        if (playWhenEnabled && audioClip != null)
        {
            Microlight.MicroAudio.MicroAudio.PlayEffectSound(audioClip);
        }
    }

    public void PlayEffectSound()
    {
        if (audioClip != null)
        {
            Microlight.MicroAudio.MicroAudio.PlayEffectSound(audioClip);
        }
    }
}

#endif
