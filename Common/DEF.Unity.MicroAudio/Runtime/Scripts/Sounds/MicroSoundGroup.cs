using System.Collections.Generic;
using UnityEngine;

namespace Microlight.MicroAudio {
    // ****************************************************************************************************
    // ScriptableObject used for storing sound group
    // ****************************************************************************************************
    [CreateAssetMenu(fileName = "MicroSoundGroup", menuName = "Microlight/Micro Audio/Sound Group")]
    public class MicroSoundGroup : ScriptableObject {
        [SerializeField] List<AudioClip> clipList;
        public List<AudioClip> ClipList => clipList;
        public AudioClip GetRandomClip {
            get {
                if(clipList.Count < 1) {
                    MicroAudioDebugger.RandomClipListEmpty();
                    return null;
                }
                return clipList[Random.Range(0, clipList.Count - 1)];
            }
        }
    }
}