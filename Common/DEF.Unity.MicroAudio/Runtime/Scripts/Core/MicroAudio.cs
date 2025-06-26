using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Microlight.MicroAudio {
    // ****************************************************************************************************
    // Manager for MicroAudio, handles settings and intermediate between modules
    // Should be on first scene
    // ****************************************************************************************************
    public class MicroAudio : MonoBehaviour {
        #region Mixer
        [SerializeField] AudioMixer _mixer;
        [SerializeField] AudioMixerGroup _masterMixerGroup;
        [SerializeField] AudioMixerGroup _musicMixerGroup;
        [SerializeField] AudioMixerGroup _soundsMixerGroup;
        [SerializeField] AudioMixerGroup _sfxMixerGroup;
        [SerializeField] AudioMixerGroup _uiMixerGroup;
        public static AudioMixer Mixer => Instance._mixer;
        public static AudioMixerGroup MasterMixerGroup => Instance._masterMixerGroup;
        public static AudioMixerGroup MusicMixerGroup => Instance._musicMixerGroup;
        public static AudioMixerGroup SoundsMixerGroup => Instance._soundsMixerGroup;
        public static AudioMixerGroup SFXMixerGroup => Instance._sfxMixerGroup;
        public static AudioMixerGroup UIMixerGroup => Instance._uiMixerGroup;
        #endregion

        #region Music
        [SerializeField] AudioSource musicAudioSource1;
        [SerializeField] AudioSource musicAudioSource2;   // Used for crossfade of songs
        [SerializeField] float crossfadeTime;
        #endregion

        #region Sound
        [SerializeField] int maxSoundsSources = 40;
        [SerializeField] int maxInstancesOfSameClip = 8;   // Maximum number of same sounds playing at the same time
        [SerializeField] Transform soundsContainer;
        #endregion

        #region Debug
        [SerializeField] bool _debugMode;
        internal static bool DebugMode {
            get {
                if(Instance == null) return false;   // Prevents NullReference exception for all debugs
                return Instance._debugMode;
            }
        }
        [SerializeField] bool _managerDebug;
        internal static bool ManagerDebug => Instance._managerDebug;
        [SerializeField] bool _musicDebug;
        internal static bool MusicDebug => Instance._musicDebug;
        [SerializeField] bool _crossfadeDebug;
        internal static bool CrossfadeDebug => Instance._crossfadeDebug;
        [SerializeField] bool _soundsDebug;
        internal static bool SoundsDebug => Instance._soundsDebug;
        [SerializeField] bool _infinityDebug;
        internal static bool InfinityDebug => Instance._infinityDebug;
        [SerializeField] bool _delayDebug;
        internal static bool DelayDebug => Instance._delayDebug;
        [SerializeField] bool _fadeDebug;
        internal static bool FadeDebug => Instance._fadeDebug;
        #endregion

        #region Settings
        // Strings for referencing mixer group
        const string MIXER_MASTER = "MasterVolume";
        const string MIXER_MUSIC = "MusicVolume";
        const string MIXER_SOUNDS = "SoundsVolume";
        const string MIXER_SFX = "SFXVolume";
        const string MIXER_UI = "UIVolume";

        // Keys for PlayerPref for saving and loading of volume
        const string MASTER_KEY = "microAudioMasterVolume";
        const string MUSIC_KEY = "microAudioMusicVolume";
        const string SOUNDS_KEY = "microAudioSoundsVolume";
        const string SFX_KEY = "microAudioSFXVolume";
        const string UI_KEY = "microAudioUIVolume";
        #endregion        

        #region Volume
        static float _masterVolume; 
        public static float MasterVolume {
            get => _masterVolume;
            set {
                if(Instance == null) {
                    MicroAudioDebugger.NotInitialized();
                    return;
                }
                value = Mathf.Clamp(value, 0.0001f, 1f);
                _masterVolume = value;
                Mixer.SetFloat(MIXER_MASTER, Mathf.Log10(value) * 20);
                MicroAudioDebugger.MasterVolumeChanged(_masterVolume);
            } 
        }
        static float _musicVolume;
        public static float MusicVolume {
            get => _musicVolume;
            set {
                if(Instance == null) {
                    MicroAudioDebugger.NotInitialized();
                    return;
                }
                value = Mathf.Clamp(value, 0.0001f, 1f);
                _musicVolume = value;
                Mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20);
                MicroAudioDebugger.MusicVolumeChanged(_musicVolume);
            }
        }
        static float _soundsVolume;
        public static float SoundsVolume {
            get => _soundsVolume;
            set {
                if(Instance == null) {
                    MicroAudioDebugger.NotInitialized();
                    return;
                }
                value = Mathf.Clamp(value, 0.0001f, 1f);
                _soundsVolume = value;
                Mixer.SetFloat(MIXER_SOUNDS, Mathf.Log10(value) * 20);
                MicroAudioDebugger.SoundsVolumeChanged(_soundsVolume);
            }
        }
        static float _sfxVolume;
        public static float SFXVolume {
            get => _sfxVolume;
            set {
                if(Instance == null) {
                    MicroAudioDebugger.NotInitialized();
                    return;
                }
                value = Mathf.Clamp(value, 0.0001f, 1f);
                _sfxVolume = value;
                Mixer.SetFloat(MIXER_SFX, Mathf.Log10(value) * 20);
                MicroAudioDebugger.SFXVolumeChanged(_sfxVolume);
            }
        }
        static float _uiVolume;
        public static float UIVolume {
            get => _uiVolume;
            set {
                if(Instance == null) {
                    MicroAudioDebugger.NotInitialized();
                    return;
                }
                value = Mathf.Clamp(value, 0.0001f, 1f);
                _uiVolume = value;
                Mixer.SetFloat(MIXER_UI, Mathf.Log10(value) * 20);
                MicroAudioDebugger.UIVolumeChanged(_uiVolume);
            }
        }
        #endregion

        #region Modules
        MicroMusic microMusic;
        MicroSounds microSounds;
        MicroInfinitySounds microInfinitySounds;
        #endregion

        // Events for systems
        internal static event Action UpdateEvent;

        #region Initialization
        // Singleton
        static MicroAudio _instance;
        internal static MicroAudio Instance {
            get => _instance;
            private set => _instance = value;
        }
        private void Awake() {
            if(Instance == null) {
                Instance = this;
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
                return;
            }

            microMusic = new MicroMusic(musicAudioSource1, musicAudioSource2, crossfadeTime);
            microSounds = new MicroSounds(soundsContainer, maxSoundsSources, maxInstancesOfSameClip);
            microInfinitySounds = new MicroInfinitySounds();

            CheckReferences();
            LoadSettings();

            MicroAudioDebugger.Initialized();
        }
        private void OnDestroy() {
            if (Instance == this) {
                Instance = null;
                UpdateEvent = null;
            }   
        }
        protected void CheckReferences() {
            if(Mixer == null) {
                Debug.LogWarning("MicroAudio: Mixer is not referenced.");
            }
            if(MasterMixerGroup == null) {
                Debug.LogWarning("MicroAudio: Master mixer group is not referenced.");
            }
            if(MusicMixerGroup == null) {
                Debug.LogWarning("MicroAudio: Music mixer group is not referenced.");
            }
            if(SoundsMixerGroup == null) {
                Debug.LogWarning("MicroAudio: Sounds mixer group is not referenced.");
            }
            if(SFXMixerGroup == null) {
                Debug.LogWarning("MicroAudio: SFX mixer group is not referenced.");
            }
            if(UIMixerGroup == null) {
                Debug.LogWarning("MicroAudio: UI mixer group is not referenced.");
            }
            if(soundsContainer == null) {
                Debug.LogWarning("MicroAudio: Sounds container is not referenced.");
            }
            if(musicAudioSource1 == null) {
                Debug.LogWarning("MicroAudio: Music audio source 1 is not referenced.");
            }
            if(musicAudioSource2 == null) {
                Debug.LogWarning("MicroAudio: Music audio source 2 is not referenced.");
            }
        }
        #endregion

        private void Update() {
            UpdateEvent?.Invoke();
        }

        // ****************************************************************************************************
        // API
        // ****************************************************************************************************
        #region API
        #region SaveLoad
        /// <summary>
        /// Save volume settings
        /// </summary>
        public static void SaveSettings() {
            PlayerPrefs.SetFloat(MASTER_KEY, MasterVolume);
            PlayerPrefs.SetFloat(MUSIC_KEY, MusicVolume);
            PlayerPrefs.SetFloat(SOUNDS_KEY, SoundsVolume);
            PlayerPrefs.SetFloat(SFX_KEY, SFXVolume);
            PlayerPrefs.SetFloat(UI_KEY, UIVolume);

            MicroAudioDebugger.SavedSettings();
        }
        /// <summary>
        /// Load volume settings
        /// </summary>
        public static void LoadSettings() {
            MasterVolume = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
            MusicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
            SoundsVolume = PlayerPrefs.GetFloat(SOUNDS_KEY, 1f);
            SFXVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);
            UIVolume = PlayerPrefs.GetFloat(UI_KEY, 1f);

            MicroAudioDebugger.LoadedSettings();
        }
        #endregion

        #region Music
        #region Events
        public static event Action OnTrackStart;
        internal static void TrackStarted() => OnTrackStart?.Invoke();
        public static event Action OnTrackEnd;
        internal static void TrackEnded() => OnTrackEnd?.Invoke();
        public static event Action<SoundFade> OnCrossfadeStart;
        internal static void CrossfadeStarted(SoundFade fade) => OnCrossfadeStart?.Invoke(fade);
        public static event Action<SoundFade> OnCrossfadeEnd;
        internal static void CrossfadeEnded(SoundFade fade) => OnCrossfadeEnd?.Invoke(fade);
        public static event Action<List<int>, MicroSoundGroup> OnNewPlaylist;   // Each time playlist is generated
        internal static void GeneratedPlaylist(List<int> playlist, MicroSoundGroup group) => OnNewPlaylist?.Invoke(playlist, group);
        public static event Action<bool> OnMusicPausedChanged;
        internal static void MusicPauseChanged(bool isPaused) => OnMusicPausedChanged?.Invoke(isPaused);
        public static event Action OnMusicStopped;
        internal static void MusicStopped() => OnMusicStopped?.Invoke();
        #endregion

        #region Parameters
        /// <summary>
        /// Audio source for music track
        /// </summary>
        public static AudioSource MusicAudioSource {
            get {
                if(Instance == null || Instance.microMusic == null) {
                    MicroAudioDebugger.NotInitialized();
                    return null;
                }
                return Instance.microMusic.MainAudioSource;
            }
        }
        /// <summary>
        /// Audio source for track that is being crossfaded out
        /// </summary>
        public static AudioSource CrossfadeAudioSource {
            get {
                if(Instance == null || Instance.microMusic == null) {
                    MicroAudioDebugger.NotInitialized();
                    return null;
                }
                return Instance.microMusic.CrossfadeAudioSource;
            }
        }
        /// <summary>
        /// 0-1 progress of MusicAudioSource
        /// </summary>
        public static float CurrentTrackProgress {
            get {
                if(Instance == null || Instance.microMusic == null) {
                    MicroAudioDebugger.NotInitialized();
                    return 0f;
                }
                return Instance.microMusic.CurrentTrackProgress;
            }
        }
        /// <summary>
        /// Currently assigned MicroSoundGroup for playing music tracks
        /// </summary>
        public static MicroSoundGroup MusicGroup {
            get {
                if(Instance == null || Instance.microMusic == null) {
                    MicroAudioDebugger.NotInitialized();
                    return null;
                }
                return Instance.microMusic.ActiveGroup;
            }
        }
        /// <summary>
        /// List of MusicGroup indexes. Defines order of tracks being played
        /// </summary>
        public static List<int> MusicPlaylist {
            get {
                if(Instance == null || Instance.microMusic == null) {
                    MicroAudioDebugger.NotInitialized();
                    return null;
                }
                return Instance.microMusic.Playlist;
            }
        }
        /// <summary>
        /// Current index of MusicPlaylist
        /// </summary>
        public static int MusicPlaylistIndex {
            get {
                if(Instance == null || Instance.microMusic == null) {
                    MicroAudioDebugger.NotInitialized();
                    return 0;
                }
                return Instance.microMusic.PlaylistIndex;
            }
        }
        /// <summary>
        /// Is playlist shuffled or not
        /// </summary>
        public static bool ShufflePlaylist {
            get {
                if(Instance == null || Instance.microMusic == null) {
                    MicroAudioDebugger.NotInitialized();
                    return false;
                }
                return Instance.microMusic.ShufflePlaylist;
            }
        }
        /// <summary>
        /// Is music playback currently paused
        /// </summary>
        public static bool IsMusicPaused { 
            get {
                if(Instance == null || Instance.microMusic == null) {
                    MicroAudioDebugger.NotInitialized();
                    return false;
                }
                return Instance.microMusic.IsPaused;
            }
        }
        #endregion

        #region Controls
        /// <summary>
        /// Plays one single track on music channel
        /// </summary>
        /// <param name="clip">Clip to be played</param>
        /// <param name="loop">Should clip be looped</param>
        /// <param name="volume">Volume of the clip clamped to 0-1, separate from music channel volume. -1 keeps volume from the last played clip</param>
        /// <param name="crossfade">Duration of the fade in, also if there is previously playing music clip it will be faded out over same duration. 
        /// -1 keeps previous crossfade duration</param>
        public static void PlayOneTrack(AudioClip clip, bool loop = true, float volume = -1f, float crossfade = -1f) {
            if(Instance == null || Instance.microMusic == null) {
                MicroAudioDebugger.NotInitialized();
                return;
            }
            Instance.microMusic.PlayOneTrack(clip, loop, volume, crossfade);
        }
        /// <summary>
        /// Plays group of tracks.
        /// </summary>
        /// <param name="group">Group of tracks that will be played</param>
        /// <param name="shuffle">Should the playlist shuffle group tracks order</param>
        /// <param name="volume">Volume of the clip clamped to 0-1, separate from music channel volume. -1 keeps volume from the last played clip</param>
        /// <param name="crossfade">Duration of the fade in of the new track, and previously playing music clip will be faded out over same duration. 
        /// -1 keeps previous crossfade duration</param>
        /// <param name="bypassCrossfade">Should first track be also crossfaded (false) or played instantly(true)</param>
        public static void PlayMusicGroup(MicroSoundGroup group, bool shuffle = false, float volume = -1f, float crossfade = -1f, bool bypassCrossfade = false) {
            if(Instance == null || Instance.microMusic == null) {
                MicroAudioDebugger.NotInitialized();
                return;
            }
            Instance.microMusic.PlayMusicGroup(group, shuffle, volume, crossfade, bypassCrossfade);
        }
        /// <summary>
        /// Play the next track in the playlist
        /// </summary>
        public static void NextTrack() {
            if(Instance == null || Instance.microMusic == null) {
                MicroAudioDebugger.NotInitialized();
                return;
            }
            Instance.microMusic.NextTrack();
        }
        /// <summary>
        /// Play the previous track in the playlist
        /// </summary>
        public static void PreviousTrack() {
            if(Instance == null || Instance.microMusic == null) {
                MicroAudioDebugger.NotInitialized();
                return;
            }
            Instance.microMusic.PreviousTrack();
        }
        /// <summary>
        /// Play the track with specified index in playlist
        /// </summary>
        public static void SelectTrack(int index) {
            if(Instance == null || Instance.microMusic == null) {
                MicroAudioDebugger.NotInitialized();
                return;
            }
            Instance.microMusic.SelectTrack(index);
        }
        /// <summary>
        /// Pauses playback of the music audio sources
        /// </summary>
        public static void PauseMusic() {
            if(Instance == null || Instance.microMusic == null) {
                MicroAudioDebugger.NotInitialized();
                return;
            }
            Instance.microMusic.Pause();
        }
        /// <summary>
        /// Resumes playback of the music audio sources
        /// </summary>
        public static void ResumeMusic() {
            if(Instance == null || Instance.microMusic == null) {
                MicroAudioDebugger.NotInitialized();
                return;
            }
            Instance.microMusic.Resume();
        }
        /// <summary>
        /// Toggles music paused state
        /// </summary>
        public static void ToggleMusicPause() {
            if(Instance == null || Instance.microMusic == null) {
                MicroAudioDebugger.NotInitialized();
                return;
            }
            if(Instance.microMusic.IsPaused) {
                Instance.microMusic.Resume();
            }
            else {
                Instance.microMusic.Pause();
            }
        }
        /// <summary>
        /// Stops music playback
        /// </summary>
        public static void StopMusic() {
            if(Instance == null || Instance.microMusic == null) {
                MicroAudioDebugger.NotInitialized();
                return;
            }
            Instance.microMusic.Stop();
        }
        #endregion
        #endregion

        #region Sounds
        #region Events
        public static event Action<AudioSource> OnSoundFinished;
        internal static void SoundFinished(AudioSource src) => OnSoundFinished?.Invoke(src);
        public static event Action<bool> OnSoundsPausedChange;
        internal static void SoundsPauseChanged(bool isPaused) => OnSoundsPausedChange?.Invoke(isPaused);
        public static event Action OnSoundsStopped;
        internal static void SoundsStopped() => OnSoundsStopped?.Invoke();
        #endregion

        #region Parameters
        public static bool IsSoundsPaused {
            get {
                if(Instance == null || Instance.microSounds == null) {
                    MicroAudioDebugger.NotInitialized();
                    return false;
                }
                return Instance.microSounds.IsPaused;
            }
        }
        #endregion

        #region Controls
        /// <summary>
        /// Play sound on SFX channel with custom audio source
        /// Source settings will be intact
        /// </summary>
        /// <param name="src">Audio source for the clip</param>
        /// <param name="delay">Delay for sound playback</param>
        /// <returns>Audio source on which sound is played</returns>
        public static AudioSource PlayEffectSound(AudioClip clip, AudioSource src, float delay = 0f) {
            if(src == null) return null;
            return PlaySound(clip, SFXMixerGroup, delay, src.volume, src.pitch, src.loop, src);
        }
        /// <summary>
        /// Play sound on SFX channel
        /// </summary>
        /// <param name="delay">Delay for sound playback</param>
        /// <param name="volume">Volume for the audio source (separate from settings volume)</param>
        /// <param name="src">Audio source for the clip. leave 'null' if want to let system choose audio source</param>
        /// <returns>Audio source on which sound is played</returns>
        public static AudioSource PlayEffectSound(AudioClip clip, float delay = 0f, float volume = 1f, float pitch = 1f, bool loop = false, AudioSource src = null) {
            return PlaySound(clip, SFXMixerGroup, delay, volume, pitch, loop, src);
        }
        /// <summary>
        /// Play sound on UI channel with custom audio source
        /// Source settings will be intact
        /// </summary>
        /// <param name="src">Audio source for the clip</param>
        /// <param name="delay">Delay for sound playback</param>
        /// <returns>Audio source on which sound is played</returns>
        public static AudioSource PlayUISound(AudioClip clip, AudioSource src, float delay = 0f) {
            if(src == null) return null;
            return PlaySound(clip, UIMixerGroup, delay, src.volume, src.pitch, src.loop, src);
        }
        /// <summary>
        /// Play sound on UI channel
        /// </summary>
        /// <param name="delay">Delay for sound playback</param>
        /// <param name="volume">Volume for the audio source (separate from settings volume)</param>
        /// <param name="src">Audio source for the clip. leave 'null' if want to let system choose audio source</param>
        /// <returns>Audio source on which sound is played</returns>
        public static AudioSource PlayUISound(AudioClip clip, float delay = 0f, float volume = 1f, float pitch = 1f, bool loop = false, AudioSource src = null) {
            return PlaySound(clip, UIMixerGroup, delay, volume, pitch, loop, src);
        }
        internal static AudioSource PlaySound(AudioClip clip, AudioMixerGroup mixerGroup, float delay, float volume, float pitch, bool loop, AudioSource src) {
            if(Instance == null || Instance.microSounds == null) {
                MicroAudioDebugger.NotInitialized();
                return null;
            }

            // If want to let system handle audio source (global)
            if(src == null) {
                return Instance.microSounds.PlaySound(clip, mixerGroup, src, delay, volume, pitch, loop);
            }
            // Custom audio source
            else {
                return Instance.microSounds.PlaySound(clip, mixerGroup, src, delay, src.volume, src.pitch, src.loop);
            }
        }
        /// <summary>
        /// If you have audio source of the delayed sound, here you can retrieve status of delayed sound
        /// </summary>
        /// <param name="src">Audio source of sound that is delayed</param>
        /// <returns>Data about delay</returns>
        public static DelayedSound GetDelayStatusOfSound(AudioSource src) {
            if(Instance == null || Instance.microSounds == null) {
                MicroAudioDebugger.NotInitialized();
                return null;
            }
            return Instance.microSounds.GetDelayStatusOfSound(src);
        }
        /// <summary>
        /// Pauses all sound effects (includes delayed sounds)
        /// Can't start new sound effects while paused
        /// </summary>
        public static void PauseSounds() {
            if(Instance == null || Instance.microSounds == null) {
                MicroAudioDebugger.NotInitialized();
                return;
            }
            Instance.microSounds.Pause();
        }
        /// <summary>
        /// Resumes playing of all sound effects (includes delayed sounds)
        /// </summary>
        public static void ResumeSounds() {
            if(Instance == null || Instance.microSounds == null) {
                MicroAudioDebugger.NotInitialized();
                return;
            }
            Instance.microSounds.Resume();
        }
        /// <summary>
        /// Stops all sounds from playing (includes delayed sounds)
        /// </summary>
        public static void StopSounds() {
            if(Instance == null || Instance.microSounds == null) {
                MicroAudioDebugger.NotInitialized();
                return;
            }
            Instance.microSounds.Stop();
        }
        #endregion
        #endregion

        #region Infinity Sounds
        /// <summary>
        /// Plays infinity sound on SFX channel
        /// </summary>
        /// <param name="group">Infinity group</param>
        /// <returns>Instance of infinity sound. Allows for control of sound and stopping effect</returns>
        public static MicroInfinityInstance PlayInfinityEffectSound(MicroInfinitySoundGroup group) {
            return PlayInfinitySound(group, SFXMixerGroup);
        }
        /// <summary>
        /// Plays infinity sound on UI channel
        /// </summary>
        /// <param name="group">Infinity group</param>
        /// <returns>Instance of infinity sound. Allows for control of sound and stopping effect</returns>
        public static MicroInfinityInstance PlayInfinityUISound(MicroInfinitySoundGroup group) {
            return PlayInfinitySound(group, UIMixerGroup);
        }
        static MicroInfinityInstance PlayInfinitySound(MicroInfinitySoundGroup group, AudioMixerGroup mixerGroup) {
            if(Instance == null || Instance.microSounds == null || Instance.microInfinitySounds == null) {
                MicroAudioDebugger.NotInitialized();
                return null;
            }
            return Instance.microInfinitySounds.PlayInfinitySound(group, mixerGroup);
        }
        #endregion

        #region Control
        public static void ClearEvents() {
            // Music
            OnTrackStart = null;
            OnTrackEnd = null;
            OnCrossfadeStart = null;
            OnCrossfadeEnd = null;
            OnNewPlaylist = null;
            OnMusicPausedChanged = null;
            OnMusicStopped = null;
    }
        #endregion
        #endregion
    }
}