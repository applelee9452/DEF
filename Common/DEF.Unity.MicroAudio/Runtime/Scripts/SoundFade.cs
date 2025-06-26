using System;
using UnityEngine;

namespace Microlight.MicroAudio {
    // ****************************************************************************************************
    // Fades sound over time
    // ****************************************************************************************************
    public class SoundFade {
        readonly AudioSource source;
        readonly float startVolume;
        readonly float endVolume;
        readonly float overSeconds;
        float timer;

        public SoundFade(AudioSource src, float startVolume, float endVolume, float overSeconds, bool isPaused = true) {
            source = src;
            this.startVolume = Mathf.Clamp01(startVolume);
            this.endVolume = Mathf.Clamp01(endVolume);
            this.overSeconds = overSeconds;

            timer = 0;
            IsPaused = isPaused;

            MicroAudio.UpdateEvent += Update;
            MicroAudioDebugger.FadeCreated(this);
        }

        // Properties
        bool _isPaused;
        public bool IsPaused {
            get => _isPaused;
            set {
                if(_isPaused == value) return;
                _isPaused = value;
                OnPausedChange?.Invoke(this);
                MicroAudioDebugger.FadePlayingChanged(this);
            }
        }        

        // Events
        public event Action<SoundFade> OnFadeEnd;   // Invoken when fade reaches end
        public event Action<SoundFade> OnPausedChange;
        public event Action<SoundFade> OnDestroy;   // Invoken when fade finishes (even if killed)

        // Public methods
        public AudioSource Source => source;
        public float StartVolume => startVolume;
        public float EndVolume => endVolume;
        public float OverSeconds => overSeconds;
        public float Progress => Mathf.Clamp01(timer / overSeconds);
        /// <summary>
        /// Stops fade on current progress and doesn't trigger fade end event.
        /// Still triggers OnDestroy event
        /// </summary>
        public void Kill() => FinishFade(false);
        /// <summary>
        /// Skips fade to end and sets source to final volume
        /// </summary>
        public void SkipToEnd() => FinishFade(true);

        // Private methods
        void Update() {
            if(!IsPaused) return;

            timer += Time.deltaTime;
            if(timer >= overSeconds) {
                FinishFade(true);
            }
            else {
                source.volume = Mathf.Clamp01(Mathf.Lerp(startVolume, endVolume, timer/overSeconds));
            }
        }        
        void FinishFade(bool setEndValue) {
            IsPaused = false;
            if(setEndValue) {
                source.volume = endVolume;
                OnFadeEnd?.Invoke(this);
                MicroAudioDebugger.FadeEnded(this);
            }
            
            OnDestroy?.Invoke(this);
            MicroAudio.UpdateEvent -= Update;
            OnFadeEnd = null;
            OnPausedChange = null;
            OnDestroy = null;
        }
    }
}