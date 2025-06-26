using System.Collections.Generic;
using UnityEngine;

namespace Microlight.MicroAudio {
    // ****************************************************************************************************
    // Various options to show debug  messages
    // ****************************************************************************************************
    internal static class MicroAudioDebugger {
        #region Manager
        internal static void Initialized() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.ManagerDebug) return;
            //return;

            Debug.Log("MicroAudio: <color=green>Initialized</color>");
        }
        internal static void NotInitialized() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.ManagerDebug) return;
            //return;

            Debug.LogWarning("MicroAudio: <color=red>Manager not present on the scene</color>");
        }
        internal static void SavedSettings() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.ManagerDebug) return;
            //return;

            Debug.Log("MicroAudio: <color=green>Settings saved</color>");
        }
        internal static void LoadedSettings() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.ManagerDebug) return;
            //return;

            Debug.Log("MicroAudio: <color=green>Settings loaded</color>");
        }
        internal static void MasterVolumeChanged(float volume) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.ManagerDebug) return;
            //return;

            Debug.Log($"MicroAudio: Master volume changed to {volume}");
        }
        internal static void MusicVolumeChanged(float volume) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.ManagerDebug) return;
            //return;

            Debug.Log($"MicroAudio: Music volume changed to {volume}");
        }
        internal static void SoundsVolumeChanged(float volume) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.ManagerDebug) return;
            //return;

            Debug.Log($"MicroAudio: Sounds volume changed to {volume}");
        }
        internal static void SFXVolumeChanged(float volume) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.ManagerDebug) return;
            //return;

            Debug.Log($"MicroAudio: SFX volume changed to {volume}");
        }
        internal static void UIVolumeChanged(float volume) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.ManagerDebug) return;
            //return;

            Debug.Log($"MicroAudio: UI volume changed to {volume}");
        }
        #endregion

        #region Music
        // Warnings
        internal static void MusicTrackEmpty() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.MusicDebug) return;
            //return;

            Debug.LogWarning($"MicroAudio: Music track/group reference is null");
        }
        internal static void NoActiveMusicGroup() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.MusicDebug) return;
            //return;

            Debug.LogWarning($"MicroAudio: There is no active playing music sound group");
        }
        internal static void PlaylistOutOfBounds(int index, int playlistCount) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.MusicDebug) return;
            //return;

            Debug.LogWarning($"MicroAudio: Tried selecting music track from playlist that is out of bounds. Index: {index}; Playlist size: {playlistCount}");
        }
        internal static void AudioSourceNotReferenced() {
            if(!MicroAudio.DebugMode) return;
            //return;

            Debug.LogWarning($"MicroAudio: One of the audio sources is not referenced");
        }

        // Logs
        internal static void TrackStarted(AudioSource src) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.MusicDebug) return;
            //return;

            Debug.Log($"MicroAudio: Track started on {src.gameObject.name}; track {src.clip.name}");
        }
        internal static void TrackEnded(AudioSource src) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.MusicDebug) return;
            //return;

            Debug.Log($"MicroAudio: Track ended on {src.gameObject.name}; track {src.clip.name}");
        }
        internal static void NextTrack() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.MusicDebug) return;
            //return;

            Debug.Log("MicroAudio: Next track.");
        }
        internal static void PreviousTrack() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.MusicDebug) return;
            //return;

            Debug.Log("MicroAudio: Previous track.");
        }
        internal static void SpecificTrack() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.MusicDebug) return;
            //return;

            Debug.Log("MicroAudio: Chosen specific track.");
        }
        internal static void MusicPaused() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.MusicDebug) return;
            //return;

            Debug.Log($"MicroAudio: Music has been paused");
        }
        internal static void MusicResumed() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.MusicDebug) return;
            //return;

            Debug.Log($"MicroAudio: Music has been resumed");
        }
        internal static void MusicStopped() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.MusicDebug) return;
            //return;

            Debug.Log($"MicroAudio: Music has been stopped");
        }
        internal static void GeneratedPlaylist(bool shuffle, List<int> playlist) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.MusicDebug) return;
            //return;

            if(shuffle) Debug.Log("MicroAudio: Generated playlist. Shuffled.");
            else Debug.Log("MicroAudio: Generated playlist. Not shuffled.");
            string playlistInString = "";
            foreach(int x in playlist) {
                playlistInString += $"{x},";
            }
            playlistInString = playlistInString.Remove(playlistInString.Length - 1, 1);
            Debug.Log($"MicroAudio: Created playlist:{playlistInString}");
        }
        internal static void ClearedMusicGroup() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.MusicDebug) return;
            //return;

            Debug.Log("MicroAudio: Cleared music group.");
        }
        internal static void SourcesSwapped() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.MusicDebug) return;
            //return;

            Debug.Log("MicroAudio: Sources swapped.");
        }
        #endregion

        #region Crossfade
        internal static void CrossfadeStarted() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.CrossfadeDebug) return;
            //return;

            Debug.Log("MicroAudio: Crossfade started.");
        }
        internal static void CrossfadeEnded() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.CrossfadeDebug) return;
            //return;

            Debug.Log("MicroAudio: Crossfade ended.");
        }
        #endregion

        #region Sounds
        // Warnings
        internal static void SoundClipEmpty() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.SoundsDebug) return;
            //return;

            Debug.LogWarning($"MicroAudio: Sound clip reference is null");
        }
        internal static void SoundMixerEmpty() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.SoundsDebug) return;
            //return;

            Debug.LogWarning($"MicroAudio: Sound mixer reference is null");
        }
        internal static void CantCreateNewSources() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.SoundsDebug) return;
            //return;

            Debug.LogWarning("MicroAudio: Can't create new sources, limit reached.");
        }
        internal static void TooManySameClips() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.SoundsDebug) return;
            //return;

            Debug.LogWarning("MicroAudio: Can't play sound because there are already too many sources with the same clip.");
        }

        // Logs
        internal static void PlaySound(AudioSource src) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.SoundsDebug) return;
            //return;

            Debug.Log($"MicroAudio: Playing sound; {src.gameObject.name}; clip:{src.clip.name}; channel:{src.outputAudioMixerGroup}");
        }
        internal static void SoundSourceCreated(AudioSource src, int soundSources) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.SoundsDebug) return;
            //return;

            Debug.Log($"MicroAudio: Sound source created; {src.gameObject.name} on {src.outputAudioMixerGroup} channel; {soundSources} total created sources");
        }
        internal static void SoundReserved(int reservedSources, int totalSources) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.SoundsDebug) return;
            //return;

            Debug.Log($"MicroAudio: Reserved sound source, {reservedSources} reserved vs {totalSources} total");
        }
        internal static void SoundFreed(int reservedSources, int totalSources) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.SoundsDebug) return;
            //return;

            Debug.Log($"MicroAudio: Freed sound source, {reservedSources} reserved vs {totalSources} total");
        }
        internal static void SoundsPaused() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.SoundsDebug) return;
            //return;

            Debug.Log($"MicroAudio: All sounds have been paused");
        }
        internal static void SoundsResumed() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.SoundsDebug) return;
            //return;

            Debug.Log($"MicroAudio: All sounds are resumed");
        }
        internal static void SoundsStopped() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.SoundsDebug) return;
            //return;

            Debug.Log($"MicroAudio: All sounds have been stopped");
        }
        #endregion

        #region Infinity
        internal static void StartInfinityGroup() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.InfinityDebug) return;
            //return;

            Debug.Log($"MicroAudio: Started infinity group.");
        }
        internal static void FinishInfinityGroup() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.InfinityDebug) return;
            //return;

            Debug.Log($"MicroAudio: Finished infinity group.");
        }
        internal static void StartInfinityLoop(AudioSource src) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.InfinityDebug) return;
            //return;

            Debug.Log($"MicroAudio: Playing infinity looped sound; {src.gameObject.name}; clip:{src.clip.name}");
        }
        internal static void StartInfinitySound(AudioSource src) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.InfinityDebug) return;
            //return;

            Debug.Log($"MicroAudio: Playing start sound for infinity group; {src.clip.name}");
        }
        internal static void EndInfinitySound(AudioSource src) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.InfinityDebug) return;
            //return;

            Debug.Log($"MicroAudio: Playing end sound for infinity group; {src.clip.name}");
        }
        internal static void RandomInfinitySound(AudioSource src) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.InfinityDebug) return;
            //return;

            Debug.Log($"MicroAudio: Playing random sound for infinity group; {src.clip.name}");
        }
        internal static void InfinitySoundPaused() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.InfinityDebug) return;
            //return;

            Debug.Log($"MicroAudio: Paused playing of infinity sound");
        }
        internal static void InfinitySoundResumed() {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.InfinityDebug) return;
            //return;

            Debug.Log($"MicroAudio: Resumed playing of infinity sound");
        }
        #endregion  

        #region Delay
        internal static void StartDelayedSound(DelayedSound delay) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.DelayDebug) return;
            //return;

            Debug.Log($"MicroAudio: Started delay for sound; {delay.Source.gameObject.name}; clip:{delay.Source.clip.name}; delay:{delay.Delay}");
        }
        internal static void DelayEnded(DelayedSound delay) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.DelayDebug) return;
            //return;

            Debug.Log($"MicroAudio: Delay of the sound ended; {delay.Source.gameObject.name}; clip:{delay.Source.clip.name}.");
        }
        #endregion

        #region Fade
        internal static void FadeCreated(SoundFade fade) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.FadeDebug) return;
            //return;

            if(fade.IsPaused) Debug.Log($"MicroAudio: Fade started on {fade.Source.gameObject.name}; {fade.StartVolume} -> {fade.EndVolume} over {fade.OverSeconds} seconds");
            else Debug.Log($"MicroAudio: Fade created but paused on {fade.Source.gameObject.name}; {fade.StartVolume} -> {fade.EndVolume} over {fade.OverSeconds} seconds");
        }
        internal static void FadeEnded(SoundFade fade) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.FadeDebug) return;
            //return;

            Debug.Log($"MicroAudio: Fade ended on {fade.Source.gameObject.name}");
        }
        internal static void FadePlayingChanged(SoundFade fade) {
            if(!MicroAudio.DebugMode) return;
            if(!MicroAudio.FadeDebug) return;
            //return;

            if(fade.IsPaused) Debug.Log($"MicroAudio: Fade resumed on {fade.Source.gameObject.name}");
            else Debug.Log($"MicroAudio: Fade paused on {fade.Source.gameObject.name}");
        }
        #endregion

        #region Forced
        internal static void RandomClipListEmpty() {
            if(!MicroAudio.DebugMode) return;
            //return;

            Debug.LogWarning("MicroAudio: Sound group doesn't have any clips. Can't get random clip.");
        }
        #endregion
    }
}