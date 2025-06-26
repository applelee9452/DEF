using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Microlight.MicroAudio {
    // ****************************************************************************************************
    // Instance of MicroInfinity sound effect
    // Lets user control the flow of the infinity sound effect
    // ****************************************************************************************************
    public class MicroInfinityInstance {
        // Variables
        MicroInfinitySoundGroup infinityGroup;
        AudioMixerGroup mixerGroup;

        // Timer
        float timer;
        float nextRandomSound;

        // Play variables
        AudioSource loopAudioSource;
        AudioSource startAudioSource;
        AudioSource endAudioSource;
        List<AudioSource> randomsAudioSource = new List<AudioSource>();

        internal MicroInfinityInstance(MicroInfinitySoundGroup infiniteGroup, AudioMixerGroup mixerGroup) {
            infinityGroup = infiniteGroup;
            this.mixerGroup = mixerGroup;

            if(infinityGroup.AmountOfRandomClips < 1) timer = -1f;

            Start();

            MicroAudio.OnSoundFinished += StopSource;
            MicroAudio.OnSoundsStopped += SoundsStopped;
            MicroAudio.OnSoundsPausedChange += SoundsPaused;
        }

        #region Properties
        bool _isPaused = false;
        public bool IsPaused {
            get => _isPaused;
            private set {
                if(_isPaused == value) return;
                _isPaused = value;

                if(_isPaused) {
                    if(startAudioSource != null) startAudioSource.Pause();
                    if(endAudioSource != null) endAudioSource.Pause();
                    if(loopAudioSource != null) loopAudioSource.Pause();
                    foreach(AudioSource x in randomsAudioSource) {
                        if(x != null) x.Pause();
                    }
                    MicroAudioDebugger.InfinitySoundPaused();
                }
                else {
                    if(startAudioSource != null) startAudioSource.UnPause();
                    if(endAudioSource != null) endAudioSource.UnPause();
                    if(loopAudioSource != null) loopAudioSource.UnPause();
                    foreach(AudioSource x in randomsAudioSource) {
                        if(x != null) x.UnPause();
                    }
                    MicroAudioDebugger.InfinitySoundResumed();
                }

                OnPausedChanged?.Invoke(this);
            }
        }
        #endregion

        #region Events
        public event Action<MicroInfinityInstance> OnEnd;
        public event Action<MicroInfinityInstance> OnPausedChanged;
        #endregion

        void Destroy() {
            OnEnd = null;
            MicroAudio.OnSoundFinished -= StopSource;
            MicroAudio.OnSoundsStopped -= SoundsStopped;
            MicroAudio.OnSoundsPausedChange -= SoundsPaused;
        }

        #region API
        /// <summary>
        /// Pauses infinity sound effect
        /// </summary>
        public void Pause() {
            if(MicroAudio.IsSoundsPaused) return;
            IsPaused = true;
        }
        /// <summary>
        /// Resumes playing infinity sound effect
        /// </summary>
        public void Resume() {
            if(MicroAudio.IsSoundsPaused) return;
            IsPaused = false;
        }
        /// <summary>
        /// Stops playing infinity sound and play end clip if defined in infinity group
        /// </summary>
        public void Stop(bool forceNoEndSound = false) {
            if(MicroAudio.IsSoundsPaused) return;
            StopAllSounds();
            if(!forceNoEndSound) PlayEndSound();
            OnEnd?.Invoke(this);
            MicroAudioDebugger.FinishInfinityGroup();
            Destroy();
        }
        #endregion

        #region Control
        void Start() {
            MicroAudioDebugger.StartInfinityGroup();
            
            PlayStartSound();
            PlayLoopSound();

            // Determine how will random sound play
            if(startAudioSource == null) {
                if(infinityGroup.DelayFirstRandomClip) {
                    SetNewRandomClipTime(0f);
                }
                else {
                    nextRandomSound = 0f;
                }
            }
        }
        void StopAllSounds() {
            StopSource(loopAudioSource);
            StopSource(startAudioSource);
            StopSource(endAudioSource);
            while(randomsAudioSource.Count > 0) {
                if(randomsAudioSource[0] == null) {
                    randomsAudioSource.RemoveAt(0);
                    continue;
                }
                StopSource(randomsAudioSource[0]);
            }
        }
        void StopSource(AudioSource src) {
            if(src == null) return;
            if(src == loopAudioSource) {
                loopAudioSource.Stop();
                loopAudioSource = null;
            }
            else if(src == startAudioSource) {
                startAudioSource.Stop();
                startAudioSource = null;
            }
            else if(src == endAudioSource) {
                endAudioSource.Stop();
                endAudioSource = null;
            }
            else if(randomsAudioSource.Contains(src)) {
                src.Stop();
                randomsAudioSource.Remove(src);
            }
        }
        void SoundsPaused(bool isPaused) {
            if(IsPaused && !isPaused) {
                Pause();   // If sounds is unpausing but infinity sound is paused, we need to pause sounds again
            }
        }
        void SoundsStopped() {
            Stop(true);
        }
        #endregion

        #region Sounds
        void PlayLoopSound() {
            if(infinityGroup.LoopClip == null) return;

            loopAudioSource = MicroAudio.PlaySound(infinityGroup.LoopClip, mixerGroup, 0f, infinityGroup.LoopClipVolume, infinityGroup.LoopClipPitch, true, null);
            MicroAudioDebugger.StartInfinityLoop(loopAudioSource);
        }
        void PlayRandomSound() {
            AudioSource source = MicroAudio.PlaySound(infinityGroup.GetRandomClip, mixerGroup, 0f, infinityGroup.RandomClipsVolume, infinityGroup.RandomClipsPitch, false, null);
            if(source == null) {
                SetNewRandomClipTime(1f);
                return;
            }

            randomsAudioSource.Add(source);
            SetNewRandomClipTime(GetSourceLength(source));
            MicroAudioDebugger.RandomInfinitySound(source);
        }
        void PlayStartSound() {
            if(infinityGroup.StartClip == null) return;

            startAudioSource = MicroAudio.PlaySound(infinityGroup.StartClip, mixerGroup, 0f, infinityGroup.StartClipVolume, infinityGroup.StartClipPitch, false, null);
            SetNewRandomClipTime(GetSourceLength(startAudioSource));
            MicroAudioDebugger.StartInfinitySound(startAudioSource);
        }
        void PlayEndSound() {
            if(infinityGroup.EndClip == null) return;

            endAudioSource = MicroAudio.PlaySound(infinityGroup.EndClip, mixerGroup, 0f, infinityGroup.EndClipVolume, infinityGroup.EndClipPitch, false, null);
            MicroAudioDebugger.EndInfinitySound(endAudioSource);
        }
        #endregion

        #region Lifetime
        internal void Update() {
            if(timer == -1) return;
            if(IsPaused) return;
            if(MicroAudio.IsSoundsPaused) return;
            timer += Time.deltaTime;

            if(timer >= nextRandomSound) {
                PlayRandomSound();
            }
        }
        void SetNewRandomClipTime(float previousClipLength) {
            nextRandomSound = 
                timer +
                previousClipLength +
                UnityEngine.Random.Range(infinityGroup.TimeBetweenClips[0], infinityGroup.TimeBetweenClips[1]);
        }
        float GetSourceLength(AudioSource src) {
            if(src == null || src.clip == null) return 0f;
            return src.clip.length / Mathf.Abs(src.pitch);
        }
        #endregion
    }
}