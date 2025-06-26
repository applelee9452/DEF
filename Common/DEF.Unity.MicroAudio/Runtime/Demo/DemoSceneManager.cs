using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Microlight.MicroAudio {
    public class DemoSceneManager : MonoBehaviour {
        [Header("UI")]
        [SerializeField] AudioClip uiClip1;
        [SerializeField] AudioClip uiClip2;
        [SerializeField] AudioClip uiClipDelayed;
        [SerializeField] Slider uiDelaySlider;

        [Header("SFX")]
        [SerializeField] AudioClip sfxClip1;
        [SerializeField] AudioClip sfxClip2;
        [SerializeField] AudioClip sfxClipDelayed;
        [SerializeField] Slider sfxDelaySlider;

        [Header("Music")]
        [SerializeField] AudioClip musicLoopTrack;
        [SerializeField] MicroSoundGroup musicGroup;
        [SerializeField] Toggle shuffleToggle;
        [SerializeField] Slider crossfadeSlider;
        [SerializeField] Button pauseMusicButton;
        [SerializeField] Button stopMusicButton;
        [SerializeField] Text pauseButtonText;

        [Header("Volume")]
        [SerializeField] Slider masterVolumeSlider;
        [SerializeField] Slider musicVolumeSlider;
        [SerializeField] Slider soundsVolumeSlider;
        [SerializeField] Slider sfxVolumeSlider;
        [SerializeField] Slider uiVolumeSlider;
        [Space]
        [SerializeField] Text masterVolumeText;
        [SerializeField] Text musicVolumeText;
        [SerializeField] Text soundsVolumeText;
        [SerializeField] Text sfxVolumeText;
        [SerializeField] Text uiVolumeText;

        [Header("Music Status")]
        [SerializeField] Text track1Text;
        [SerializeField] Text track2Text;
        [SerializeField] Text track3Text;
        [SerializeField] Slider trackProgressSlider;
        [SerializeField] Slider crossfadeProgressSlider;
        [SerializeField] Text crossfadeFromTrack;
        [SerializeField] Text crossfadeToTrack;
        SoundFade crossfade;

        [Header("Infinity Sounds")]
        [SerializeField] MicroInfinitySoundGroup microInfinitySoundGroup;
        [SerializeField] Button playInfinityButton;
        [SerializeField] Button pauseInfinityButton;
        [SerializeField] Button stopInfinityButton;

        [Header("Misc")]
        [SerializeField] AudioClip checkVolumeClip;
        DelayedSound volumePingTest;   // Tests volume of sound effects when changing value
        const float pingTestDelay = 0.25f;

        private void Start() {
            masterVolumeSlider.value = MicroAudio.MasterVolume;
            musicVolumeSlider.value = MicroAudio.MusicVolume;
            sfxVolumeSlider.value = MicroAudio.SFXVolume;
            uiVolumeSlider.value = MicroAudio.UIVolume;

            MicroAudio.OnNewPlaylist += UpdatePlaylistTrackNames;
            MicroAudio.OnTrackEnd += UpdatePlaylistStatus;
            MicroAudio.OnTrackStart += UpdatePlaylistStatus;
            MicroAudio.OnMusicStopped += UpdatePlaylistStatus;
            MicroAudio.OnMusicPausedChanged += MusicPaused;
            MicroAudio.OnCrossfadeStart += CrossfadeStarted;
            MicroAudio.OnCrossfadeEnd += CrossfadeEnded;

            crossfadeProgressSlider.gameObject.SetActive(false);
            crossfadeFromTrack.gameObject.SetActive(false);
            crossfadeToTrack.gameObject.SetActive(false);

            pauseMusicButton.interactable = false;
            stopMusicButton.interactable = false;

            pauseInfinityButton.interactable = false;
            stopInfinityButton.interactable = false;
            playInfinityButton.interactable = true;
        }

        private void Update() {
            trackProgressSlider.value = MicroAudio.CurrentTrackProgress;

            if(crossfade != null && crossfade.IsPaused) crossfadeProgressSlider.value = crossfade.Progress;
            else if(crossfade != null && !crossfade.IsPaused) CrossfadeEnded(crossfade);
        }

        #region UI Sounds Controls
        public void UIButton1() {
            MicroAudio.PlayUISound(uiClip1);
        }
        public void UIButton2() {
            MicroAudio.PlayUISound(uiClip2);
        }
        public void UIButtonDelay() {
            MicroAudio.PlayUISound(uiClipDelayed, uiDelaySlider.value);
        }
        #endregion

        #region SFX Sounds Controls
        public void SFXButton1() {
            MicroAudio.PlayUISound(sfxClip1);
        }
        public void SFXButton2() {
            MicroAudio.PlayUISound(sfxClip2);
        }
        public void SFXButtonDelay() {
            MicroAudio.PlayUISound(sfxClipDelayed, sfxDelaySlider.value);
        }
        #endregion

        #region Volume Controls
        public void OnMasterVolumeChange() {
            MicroAudio.MasterVolume = masterVolumeSlider.value;
            MicroAudio.SaveSettings();
            masterVolumeText.text = ((int)(masterVolumeSlider.value * 100)).ToString();
        }
        public void OnMusicVolumeChange() {
            MicroAudio.MusicVolume = musicVolumeSlider.value;
            MicroAudio.SaveSettings();
            musicVolumeText.text = ((int)(musicVolumeSlider.value * 100)).ToString();
        }
        public void OnSoundsVolumeChange() {
            MicroAudio.SoundsVolume = soundsVolumeSlider.value;
            MicroAudio.SaveSettings();
            soundsVolumeText.text = ((int)(soundsVolumeSlider.value * 100)).ToString();
            StartVolumeTestPing(1);
        }
        public void OnSFXVolumeChange() {
            MicroAudio.SFXVolume = sfxVolumeSlider.value;
            MicroAudio.SaveSettings();
            sfxVolumeText.text = ((int)(sfxVolumeSlider.value * 100)).ToString();
            StartVolumeTestPing(1);
        }
        public void OnUIVolumeChange() {
            MicroAudio.UIVolume = uiVolumeSlider.value;
            MicroAudio.SaveSettings();
            uiVolumeText.text = ((int)(uiVolumeSlider.value * 100)).ToString();
            StartVolumeTestPing(0);
        }
        void StartVolumeTestPing(int layer) {
            // If already runing, reset
            if(volumePingTest != null && volumePingTest.Progress < 1f) {
                volumePingTest.ResetTimer();
                return;
            }

            if(layer == 0) {
                AudioSource src = MicroAudio.PlayUISound(checkVolumeClip, pingTestDelay);
                volumePingTest = MicroAudio.GetDelayStatusOfSound(src);
            }
            else {
                AudioSource src = MicroAudio.PlayEffectSound(checkVolumeClip, pingTestDelay);
                volumePingTest = MicroAudio.GetDelayStatusOfSound(src);
            }
        }
        #endregion

        #region Music
        public void LoopOne() {
            MicroAudio.PlayOneTrack(musicLoopTrack);
        }
        public void PlayMusicGroup() {
            MicroAudio.PlayMusicGroup(musicGroup, shuffleToggle.isOn, 1f, crossfadeSlider.value);
        }
        public void NextTrack() {
            MicroAudio.NextTrack();
        }
        public void PreviousTrack() {
            MicroAudio.PreviousTrack();
        }
        public void PauseMusic() {
            MicroAudio.ToggleMusicPause();
        }
        public void StopMusic() {
            MicroAudio.StopMusic();
        }
        #endregion

        #region Music Status
        void UpdatePlaylistTrackNames(List<int> playlist, MicroSoundGroup group) {
            track1Text.text = group.ClipList[playlist[0]].name;
            track2Text.text = group.ClipList[playlist[1]].name;
            track3Text.text = group.ClipList[playlist[2]].name;
            UpdatePlaylistStatus();
        }
        void MusicPaused(bool isPaused) {
            UpdatePlaylistStatus();
        }
        void UpdatePlaylistStatus() {
            Debug.Log("Updating playlist status");
            track1Text.color = masterVolumeText.color;
            track2Text.color = masterVolumeText.color;
            track3Text.color = masterVolumeText.color;

            if(MicroAudio.MusicAudioSource.isPlaying) {
                pauseButtonText.text = "Pause";
                pauseMusicButton.interactable = true;
                stopMusicButton.interactable = true;
            }
            else if(MicroAudio.MusicAudioSource.time > 0) {
                if(MicroAudio.IsMusicPaused) {
                    pauseButtonText.text = "Resume";
                }
                else {
                    pauseButtonText.text = "Pause";
                }
                pauseMusicButton.interactable = true;
                stopMusicButton.interactable = true;
            }
            else {
                pauseButtonText.text = "Pause";
                pauseMusicButton.interactable = false;
                stopMusicButton.interactable = false;
            }

            if(MicroAudio.MusicGroup == null) return;
            else if(MicroAudio.MusicPlaylistIndex == 0) track1Text.color = Color.cyan;
            else if(MicroAudio.MusicPlaylistIndex == 1) track2Text.color = Color.cyan;
            else if(MicroAudio.MusicPlaylistIndex == 2) track3Text.color = Color.cyan;
        }
        void CrossfadeStarted(SoundFade fade) {
            // Display crossfade status
            crossfadeProgressSlider.gameObject.SetActive(true);
            crossfadeFromTrack.gameObject.SetActive(true);
            crossfadeToTrack.gameObject.SetActive(true);

            crossfade = fade;

            if(MicroAudio.CrossfadeAudioSource.clip != null) crossfadeFromTrack.text = MicroAudio.CrossfadeAudioSource.clip.name;
            if(MicroAudio.MusicAudioSource.clip != null) crossfadeToTrack.text = MicroAudio.MusicAudioSource.clip.name;
        }
        void CrossfadeEnded(SoundFade fade) {
            // Hide crossfade status
            crossfadeProgressSlider.gameObject.SetActive(false);
            crossfadeFromTrack.gameObject.SetActive(false);
            crossfadeToTrack.gameObject.SetActive(false);

            crossfade = null;
        }
        #endregion

        #region Infinity Sound
        MicroInfinityInstance infinityInstance;
        public void PlayInfinitySound() {
            infinityInstance = MicroAudio.PlayInfinityEffectSound(microInfinitySoundGroup);
            infinityInstance.OnEnd += InfinitySoundEnded;
            pauseInfinityButton.interactable = true;
            stopInfinityButton.interactable = true;
            playInfinityButton.interactable = false;
        }
        void InfinitySoundEnded(MicroInfinityInstance instance) {
            infinityInstance.OnEnd -= InfinitySoundEnded;
            infinityInstance = null;
            pauseInfinityButton.interactable = false;
            stopInfinityButton.interactable = false;
            playInfinityButton.interactable = true;
        }
        public void PauseInfinitySound() {
            if(infinityInstance == null) return;
            else if(infinityInstance.IsPaused) infinityInstance.Resume();
            else infinityInstance.Pause();
        }
        public void StopInfinitySound() {
            if(infinityInstance == null) return;
            infinityInstance.Stop();
        }
        #endregion
    }
}