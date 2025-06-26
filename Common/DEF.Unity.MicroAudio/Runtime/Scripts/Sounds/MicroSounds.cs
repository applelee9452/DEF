using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Microlight.MicroAudio {
    // ****************************************************************************************************
    // Sounds module for MicroAudio
    // ****************************************************************************************************
    internal class MicroSounds {
        // Sources
        Transform soundsContainer;
        readonly List<AudioSource> soundAudioSources = new List<AudioSource>();
        readonly List<AudioSource> playingAudioSources = new List<AudioSource>();   // Used for reporting on audio sources which finished playing
        readonly List<AudioSource> reservedSoundAudioSources = new List<AudioSource>();   // List of audio sources which might not be playing right now BUT are needed for other uses
        readonly List<DelayedSound> delayedSounds = new List<DelayedSound>();   // List of delayed sounds

        readonly int MAX_SOUND_SOURCES;
        readonly int MAX_INSTANCES_OF_SAME_CLIP;

        internal MicroSounds(Transform soundsContainer, int maxSoundSources, int maxInstancesOfSameClip) {
            // Sources
            this.soundsContainer = soundsContainer;

            MAX_SOUND_SOURCES = maxSoundSources;
            MAX_INSTANCES_OF_SAME_CLIP = maxInstancesOfSameClip;

            MicroAudio.UpdateEvent += Update;
        }

        bool _isPaused = false;
        internal bool IsPaused {
            get => _isPaused;
            private set {
                if(_isPaused == value) return;

                _isPaused = value;
                if(_isPaused) {
                    foreach(AudioSource x in playingAudioSources) {
                        if(x != null) x.Pause();
                    }
                    MicroAudioDebugger.SoundsPaused();
                }
                else {
                    foreach(AudioSource x in playingAudioSources) {
                        if(x != null) x.UnPause();
                    }
                    MicroAudioDebugger.SoundsResumed();
                }
                MicroAudio.SoundsPauseChanged(_isPaused);
            }
        }

        void Update() {
            for(int i = 0; i < playingAudioSources.Count; i++) {
                AudioSource x = playingAudioSources[i];

                // If somehow audio source is null
                if(x == null) {
                    playingAudioSources.RemoveAt(i);
                    i--;
                    continue;
                }
                // If for some reason clip is null
                else if(x.clip == null) {
                    MicroAudio.SoundFinished(x);
                    playingAudioSources.Remove(x);
                    i--;
                    continue;
                }
                else if(!x.isPlaying && x.time == 0 && !x.loop) {
                    MicroAudio.SoundFinished(x);
                    playingAudioSources.Remove(x);
                    x.clip = null;
                    i--;
                    continue;
                }
            }
        }

        #region API
        internal AudioSource PlaySound(AudioClip clip, AudioMixerGroup mixerGroup, AudioSource src, float delay, float volume, float pitch, bool loop) {
            if(clip == null) {
                MicroAudioDebugger.SoundClipEmpty();
                return null;
            }
            if(mixerGroup == null) {
                MicroAudioDebugger.SoundMixerEmpty();
                return null;
            }
            if(src == null) {
                src = GetFreeSoundSource(clip);
            }
            if(src == null) {
                return null;
            }

            src.clip = clip;
            src.volume = Mathf.Clamp01(volume);
            src.outputAudioMixerGroup = mixerGroup;
            src.loop = loop;
            src.pitch = pitch;

            // Play
            if(delay > 0f) {
                DelaySoundEffect(src, delay);
            }
            else {
                PlaySound(src);
            }

            return src;
        }
        void PlaySound(AudioSource src) {
            src.Play();
            playingAudioSources.Add(src);
            MicroAudioDebugger.PlaySound(src);
        }
        internal DelayedSound GetDelayStatusOfSound(AudioSource src) {
            return delayedSounds.Find(x => x.Source == src);
        }
        internal void Pause() {
            foreach(DelayedSound x in delayedSounds) {
                x.IsPaused = true;
            }
            IsPaused = true;
        }
        internal void Resume() {
            foreach(DelayedSound x in delayedSounds) {
                x.IsPaused = false;
            }
            IsPaused = false;
        }
        internal void Stop() {
            foreach(AudioSource x in soundAudioSources) {
                x.Stop();
                x.clip = null;
            }
            while(delayedSounds.Count > 0) {
                delayedSounds[0].Kill();
            }
        }
        #endregion

        #region Delay
        DelayedSound DelaySoundEffect(AudioSource src, float delay) {
            DelayedSound delayedSound = new DelayedSound(src, delay);
            delayedSounds.Add(delayedSound);
            ReserveSoundSource(src);
            delayedSound.OnDelayEnd += PlayDelayedSound;
            delayedSound.OnDestroy += RemoveDelayedSound;
            return delayedSound;
        }
        void PlayDelayedSound(DelayedSound delayedSound) {
            PlaySound(delayedSound.Source);
        }
        void RemoveDelayedSound(DelayedSound delayedSound) {
            UnreserveSoundSource(delayedSound.Source);
            delayedSounds.Remove(delayedSound);
        }
        #endregion

        #region Reserve
        internal void ReserveSoundSource(AudioSource src) {
            if(src != null) {
                reservedSoundAudioSources.Add(src);
                MicroAudioDebugger.SoundReserved(reservedSoundAudioSources.Count, soundAudioSources.Count);
            }
        }
        internal void UnreserveSoundSource(AudioSource src) {
            if(src != null) {
                reservedSoundAudioSources.Remove(src);
                MicroAudioDebugger.SoundFreed(reservedSoundAudioSources.Count, soundAudioSources.Count);
            }
        }
        bool IsAudioSourceReserved(AudioSource src) {
            foreach(AudioSource x in reservedSoundAudioSources) {
                if(src == x) return true;
            }
            foreach(AudioSource x in playingAudioSources) {
                if(src == x) return true;
            }
            return false;
        }
        #endregion

        #region Audio sources
        AudioSource GetFreeSoundSource(AudioClip clipToPlay) {
            // Check for too many of same clip
            if(MAX_INSTANCES_OF_SAME_CLIP > 0) {
                int sameClipCount = 0;
                foreach(AudioSource x in soundAudioSources) {
                    if(x.clip == clipToPlay && x.isPlaying) sameClipCount++;
                    if(sameClipCount >= MAX_INSTANCES_OF_SAME_CLIP) {
                        MicroAudioDebugger.TooManySameClips();
                        return null;
                    }
                }
            }

            // Free source
            foreach(AudioSource x in soundAudioSources) {
                if(!x.isPlaying && !IsAudioSourceReserved(x)) return x;   // We found free sound source
            }

            // If there is no available sources, try to create new
            if(soundAudioSources.Count >= MAX_SOUND_SOURCES && MAX_SOUND_SOURCES > 0) {
                MicroAudioDebugger.CantCreateNewSources();
                return null;
            }
            return CreateNewSoundSource();
        }
        AudioSource CreateNewSoundSource() {
            GameObject obj = new GameObject($"SoundPlayer {soundAudioSources.Count}", typeof(AudioSource));
            obj.transform.SetParent(soundsContainer);
            AudioSource src = obj.GetComponent<AudioSource>();
            soundAudioSources.Add(src);
            src.playOnAwake = false;

            MicroAudioDebugger.SoundSourceCreated(src, soundAudioSources.Count);
            return src;
        }
        #endregion
    }
}