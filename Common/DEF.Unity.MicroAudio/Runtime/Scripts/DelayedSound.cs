using System;
using UnityEngine;

namespace Microlight.MicroAudio {
    // ****************************************************************************************************
    // Delays playing of sound
    // ****************************************************************************************************
    public class DelayedSound {
        readonly AudioSource source;
        readonly float delay;
        float timer;

        public DelayedSound(AudioSource src, float delay) {
            source = src;
            this.delay = delay;

            timer = 0;

            MicroAudio.UpdateEvent += Update;
            MicroAudioDebugger.StartDelayedSound(this);
        }

        bool _isPaused;
        public bool IsPaused {
            get => _isPaused;
            set {
                if (_isPaused == value) return;

                _isPaused = value;
                OnPausedChanged?.Invoke(this);
            }
        }

        // Events
        public event Action<DelayedSound> OnDelayEnd;   // Called only when it reaches end, also plays sound effect
        public event Action<DelayedSound> OnPausedChanged;
        public event Action<DelayedSound> OnDestroy;   // Event which is always called when finishing delay

        // Public methods
        public AudioSource Source => source;
        public float Delay => delay;
        public float Progress => Mathf.Clamp01(timer / delay);
        /// <summary>
        /// Stops delay process and doesn't let the sound be played
        /// </summary>
        public void Kill() => FinishDelay(false);
        /// <summary>
        /// Skips delay process and plays the sound
        /// </summary>
        public void SkipToEnd() => FinishDelay(true);
        public void ResetTimer() => timer = 0;

        // Private methods
        void Update() {
            if(timer >= delay) return;
            if(IsPaused) return;

            timer += Time.deltaTime;
            if(timer >= delay) {
                FinishDelay(true);
            }
        }
        void FinishDelay(bool playSound) {
            timer = delay;
            if(playSound) {
                MicroAudioDebugger.DelayEnded(this);
                OnDelayEnd?.Invoke(this);
            }

            OnDestroy?.Invoke(this);
            MicroAudio.UpdateEvent -= Update;
            OnDelayEnd = null;
            OnDestroy = null;
        } 
    }
}