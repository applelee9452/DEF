using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microlight.MicroAudio {
    // ****************************************************************************************************
    // Music module for MicroAudio
    // ****************************************************************************************************
    internal class MicroMusic {
        #region Sources
        AudioSource _mainAudioSource;
        internal AudioSource MainAudioSource {
            get => _mainAudioSource;
            private set => _mainAudioSource = value;
        }
        AudioSource _crossfadeAudioSource;
        internal AudioSource CrossfadeAudioSource {
            get => _crossfadeAudioSource;
            private set => _crossfadeAudioSource = value;
        }
        #endregion

        #region Tracks
        bool _shufflePlaylist = false;
        internal bool ShufflePlaylist {
            get => _shufflePlaylist;
            private set => _shufflePlaylist = value;
        }
        internal float CurrentTrackProgress {
            get {
                if(MainAudioSource == null) return 0;
                if(CrossfadeAudioSource == null) return 0;

                if(CrossfadeAudioSource.time > 0) {
                    if(CrossfadeAudioSource.clip == null) return 0;
                    return Mathf.Clamp01(CrossfadeAudioSource.time / CrossfadeAudioSource.clip.length);
                }
                else {
                    if(MainAudioSource.clip == null) return 0;
                    return Mathf.Clamp01(MainAudioSource.time / MainAudioSource.clip.length);
                }
            }
        }
        #endregion

        #region Playlist
        MicroSoundGroup _activeGroup = null;   // Selected group of music tracks
        internal MicroSoundGroup ActiveGroup {
            get => _activeGroup;
            private set => _activeGroup = value;
        }
        List<int> _playlist = new List<int>();   // Playlist that stores index of _activeGroup
        internal List<int> Playlist {
            get => _playlist;
        }
        int _playlistIndex = 0;   // Current index of _playlist
        internal int PlaylistIndex {
            get => _playlistIndex;
            private set {
                if(Playlist == null) return;
                if(_playlistIndex == value) return;

                _playlistIndex = value;
                if(Playlist.Count == 0) {
                    return;
                }
                else if(_playlistIndex >= Playlist.Count) {
                    GeneratePlaylist();
                }
                else if(_playlistIndex < 0) {
                    GeneratePlaylist();
                    _playlistIndex = Playlist.Count - 1;
                }
            }
        }
        #endregion

        #region Playback
        float _musicVolume = 1f;
        float MusicVolume {
            get => _musicVolume;
            set {
                if(_musicVolume == value) return;
                if(value == -1) return;
                _musicVolume = Mathf.Clamp01(value);
            }
        }
        bool _isPaused = false;
        internal bool IsPaused {
            get => _isPaused;
            private set {
                if(_isPaused == value) return;

                _isPaused = value;
                if(_isPaused) {
                    MainAudioSource.Pause();
                    CrossfadeAudioSource.Pause();
                    if(mainFade != null) {
                        mainFade.IsPaused = false;
                    }
                    if(crossfadeFade != null) {
                        crossfadeFade.IsPaused = false;
                    }
                    MicroAudioDebugger.MusicPaused();
                }
                else {
                    MainAudioSource.UnPause();
                    CrossfadeAudioSource.UnPause();
                    if(mainFade != null) {
                        mainFade.IsPaused = true;
                    }
                    if(crossfadeFade != null) {
                        crossfadeFade.IsPaused = true;
                    }
                    MicroAudioDebugger.MusicResumed();
                }
                MicroAudio.MusicPauseChanged(_isPaused);
            }
        }
        #endregion

        #region Crossfade
        float _crossfadeDuration;
        float CrossfadeDuration {
            get => _crossfadeDuration;
            set {
                if(_crossfadeDuration == value) return;
                if(value == -1) return;

                _crossfadeDuration = value;
            }
        }
        SoundFade mainFade;
        SoundFade crossfadeFade;
        #endregion

        internal MicroMusic(AudioSource source1, AudioSource source2, float crossfadeTime) {
            MainAudioSource = source1;
            CrossfadeAudioSource = source2;
            CrossfadeDuration = crossfadeTime;

            MicroAudio.UpdateEvent += Update;
        }

        void Update() {
            if(IsPaused) {
                return;
            }

            // To call the end of the music track before end because crossfade is on
            if(ActiveGroup != null &&
                CrossfadeDuration > 0f &&
                MainAudioSource.clip != null &&
                MainAudioSource.time >= MainAudioSource.clip.length - CrossfadeDuration &&
                CrossfadeAudioSource.clip != null)
            {
                TrackEnded();
            }
            // Music track has ended
            else if(MainAudioSource.clip != null && 
                !MainAudioSource.loop && 
                MainAudioSource.time == 0 && 
                !MainAudioSource.isPlaying)
            {
                TrackEnded();
            }
        }

        #region API methods
        internal void PlayOneTrack(AudioClip clip, bool loop, float volume, float crossfade) {
            if(clip == null) {
                MicroAudioDebugger.MusicTrackEmpty();
            }
            PrepareMusicState(volume, crossfade);

            PlayMusicClip(clip, loop);
            DetermineIfCrossfadeNeeded();
        }
        internal void PlayMusicGroup(MicroSoundGroup group, bool shuffle, float volume, float crossfade, bool bypassCrossfade) {
            if(group == null) {
                MicroAudioDebugger.MusicTrackEmpty();
                return;
            }
            if(group.ClipList.Count < 1) {
                ActiveGroup = null;
                return;
            }
            PrepareMusicState(volume, crossfade);

            ActiveGroup = group;
            ShufflePlaylist = shuffle;
            GeneratePlaylist();
            ChangeTrack(bypassCrossfade);
        }
        internal void NextTrack() {
            if(ActiveGroup == null) {
                MicroAudioDebugger.NoActiveMusicGroup();
                return;
            }
            PlaylistIndex++;
            ChangeTrack();
            MicroAudioDebugger.NextTrack();
        }
        internal void PreviousTrack() {
            if(ActiveGroup == null) {
                MicroAudioDebugger.NoActiveMusicGroup();
                return;
            }
            PlaylistIndex--;
            ChangeTrack();
            MicroAudioDebugger.PreviousTrack();
        }
        internal void SelectTrack(int index) {
            if(ActiveGroup == null) {
                MicroAudioDebugger.NoActiveMusicGroup();
                return;
            }
            if(index < 0 || index >= Playlist.Count) {
                MicroAudioDebugger.PlaylistOutOfBounds(index, Playlist.Count);
                return;
            }
            PlaylistIndex = index;
            ChangeTrack();
            MicroAudioDebugger.SpecificTrack();
        }
        internal void Pause() {
            IsPaused = true;
        }
        internal void Resume() {
            IsPaused = false;
        }
        internal void Stop() {
            ClearAudioSources();
            MicroAudio.MusicStopped();
            MicroAudioDebugger.MusicStopped();
        }
        #endregion

        #region Music control
        void PlayMusicClip(AudioClip clip, bool loop) {
            if(MainAudioSource == null || CrossfadeAudioSource == null) {
                MicroAudioDebugger.AudioSourceNotReferenced();
                return;
            }

            SwapAudioSources();
            MainAudioSource.clip = clip;
            MainAudioSource.loop = loop;
            MainAudioSource.volume = MusicVolume;
            MainAudioSource.Play();
            MicroAudio.TrackStarted();
            MicroAudioDebugger.TrackStarted(MainAudioSource);
        }
        // Need to select PlaylistIndex before calling change track
        void ChangeTrack(bool bypassFade = false) {
            if(ActiveGroup == null) return;
            if(Playlist.Count < 1) return;

            PlayMusicClip(ActiveGroup.ClipList[Playlist[PlaylistIndex]], false);
            DetermineIfCrossfadeNeeded(bypassFade);
        }
        void GeneratePlaylist() {
            if(ActiveGroup == null) return;
            PlaylistIndex = 0;
            Playlist.Clear();

            for(int i = 0; i < ActiveGroup.ClipList.Count; i++) {
                Playlist.Add(i);
            }

            if(ShufflePlaylist) Shuffle();
            MicroAudio.GeneratedPlaylist(Playlist, ActiveGroup);
            MicroAudioDebugger.GeneratedPlaylist(ShufflePlaylist, Playlist);

            void Shuffle() {
                int n = Playlist.Count;
                int k;
                int swapTmp;
                while(n > 1) {
                    n--;
                    k = UnityEngine.Random.Range(0, n + 1);
                    swapTmp = Playlist[k];
                    Playlist[k] = Playlist[n];
                    Playlist[n] = swapTmp;
                }
            }
        }
        void ClearMusicGroup() {
            ActiveGroup = null;
            Playlist.Clear();
            PlaylistIndex = 0;
            MicroAudioDebugger.ClearedMusicGroup();
        }
        void ClearAudioSources() {
            MainAudioSource.Stop();
            MainAudioSource.clip = null;
            CrossfadeAudioSource.Stop();
            CrossfadeAudioSource.clip = null;
        }
        void SwapAudioSources() {
            AudioSource tmpSrc = MainAudioSource;
            MainAudioSource = CrossfadeAudioSource;
            CrossfadeAudioSource = tmpSrc;
            MicroAudioDebugger.SourcesSwapped();
        }
        void PrepareMusicState(float volume, float crossfade) {
            Resume();
            ClearAudioSources();
            ClearMusicGroup();
            ClearSoundFades();
            MusicVolume = volume;
            CrossfadeDuration = crossfade;
        }
        void ClearCrossfadeTrack() {
            CrossfadeAudioSource.Stop();
            CrossfadeAudioSource.clip = null;
        }
        void DetermineIfCrossfadeNeeded(bool bypass = false) {
            if(!bypass && CrossfadeDuration > 0f) {
                CrossfadeTracks();
            }
            else {
                ClearCrossfadeTrack();
            }
        }
        void TrackEnded() {
            MicroAudioDebugger.TrackEnded(MainAudioSource);
            if(ActiveGroup != null) {
                MicroAudio.TrackEnded();
                NextTrack();
            }
            else {
                MainAudioSource.Stop();
                MainAudioSource.clip = null;
                MicroAudio.TrackEnded();
            }
        }
        #endregion        

        #region Music Fade
        void CrossfadeTracks() {
            ClearSoundFades();
            FadeSound(MainAudioSource, 0f, MusicVolume, CrossfadeDuration);
            FadeSound(CrossfadeAudioSource, MusicVolume, 0f, CrossfadeDuration);
        }
        void FadeSound(AudioSource src, float startVolume, float endVolume, float overSeconds) {
            SoundFade fade = new SoundFade(src, startVolume, endVolume, overSeconds);
            fade.OnFadeEnd += FadeFinished;
            fade.OnDestroy += FadeDestroyed;
            if(src == MainAudioSource) {
                mainFade = fade;
            }
            else {
                crossfadeFade = fade;
                MicroAudio.CrossfadeStarted(fade);
                MicroAudioDebugger.CrossfadeStarted();
            }            
        }
        void FadeFinished(SoundFade fade) {
            if(mainFade != null && mainFade == fade) {
                mainFade = null;
            }
            else if(crossfadeFade != null && crossfadeFade == fade){
                crossfadeFade = null;
                CrossfadeAudioSource.Stop();
                CrossfadeAudioSource.clip = null;
                MicroAudio.CrossfadeEnded(fade);
                MicroAudioDebugger.CrossfadeEnded();
            }
        }
        void FadeDestroyed(SoundFade fade) {
            if(mainFade != null && mainFade == fade) {
                mainFade = null;
            }
            else if(crossfadeFade != null && crossfadeFade == fade) {
                crossfadeFade = null;
                CrossfadeAudioSource.Stop();
                CrossfadeAudioSource.clip = null;
            }
        }
        void ClearSoundFades() {
            if(mainFade != null) {
                mainFade.Kill();
                mainFade = null;
            }
            if(crossfadeFade != null) {
                crossfadeFade.Kill();
                crossfadeFade = null;
            }
        }
        #endregion
    }
}