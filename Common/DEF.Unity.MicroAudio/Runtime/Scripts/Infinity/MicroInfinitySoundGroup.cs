using System.Collections.Generic;
using UnityEngine;

namespace Microlight.MicroAudio {
    // ****************************************************************************************************
    // ScriptableObject used for storing data about infinite sound group
    // ****************************************************************************************************
    [CreateAssetMenu(fileName = "MicroInfinitySoundGroup", menuName = "Microlight/Micro Audio/Infinity Sound Group")]
    public class MicroInfinitySoundGroup : ScriptableObject {
        [SerializeField] AudioClip _loopClip;
        [SerializeField][Range(0f, 1f)] float _loopClipVolume = 1f;
        [SerializeField][Range(-3f, 3f)] float _loopClipPitch = 1f;
        [SerializeField] AudioClip _startClip;
        [SerializeField][Range(0f, 1f)] float _startClipVolume = 1f;
        [SerializeField][Range(-3f, 3f)] float _startClipPitch = 1f;
        [SerializeField] AudioClip _endClip;
        [SerializeField][Range(0f, 1f)] float _endClipVolume = 1f;
        [SerializeField][Range(-3f, 3f)] float _endClipPitch = 1f;
        [SerializeField] List<AudioClip> _randomClips;
        [SerializeField][Range(0f, 1f)] float _randomClipsVolume = 1f;
        [SerializeField][Range(-3f, 3f)] float _randomClipsPitch = 1f;

        [Header("Settings")]
        [SerializeField] float _minTimeBetweenRandomClips;
        [SerializeField] float _maxTimeBetweenRandomClips;
        [SerializeField] bool _delayFirstRandomClip;

        public AudioClip LoopClip => _loopClip;
        public float LoopClipVolume => _loopClipVolume;
        public float LoopClipPitch => _loopClipPitch;
        public AudioClip StartClip => _startClip;
        public float StartClipVolume => _startClipVolume;
        public float StartClipPitch => _startClipPitch;
        public AudioClip EndClip => _endClip;
        public float EndClipVolume => _endClipVolume;
        public float EndClipPitch => _endClipPitch;
        public List<AudioClip> RandomClips => _randomClips;
        public float RandomClipsVolume => _randomClipsVolume;
        public float RandomClipsPitch => _randomClipsPitch;
        public float[] TimeBetweenClips => new float[2] { _minTimeBetweenRandomClips, _maxTimeBetweenRandomClips };
        public int AmountOfRandomClips => _randomClips.Count;
        public bool DelayFirstRandomClip => _delayFirstRandomClip;
        public AudioClip GetRandomClip {
            get {
                if(_randomClips.Count < 1) {
                    MicroAudioDebugger.RandomClipListEmpty();
                    return null;
                }
                return _randomClips[Random.Range(0, _randomClips.Count - 1)];
            }
        }
    }
}