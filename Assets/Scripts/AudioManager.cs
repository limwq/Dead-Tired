using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;
    public AudioSource monsterSource;
    public AudioSource loopingSFXSource;
    public AudioSource showerloopingSource;

    [Header("Audio Clips")]
    public List<AudioClip> bgmClips;
    public List<AudioClip> sfxClips;
    public List<AudioClip> monsterClips;
    public List<AudioClip> transitionClips;

    private Dictionary<string, AudioClip> clipDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioSource> activeSources = new Dictionary<string, AudioSource>();
    private AudioClip lastPlayedClip;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var clip in bgmClips) if (clip) clipDict[clip.name] = clip;
        foreach (var clip in sfxClips) if (clip) clipDict[clip.name] = clip;
        foreach (var clip in monsterClips) if (clip) clipDict[clip.name] = clip;
        foreach (var clip in transitionClips) if (clip) clipDict[clip.name] = clip;

        // ✅ Subscribe to sceneLoaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // === STOP looping sounds when new scene loads ===
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        StopLoopingSFX();
        StopShowerLooping();
        StopMonsterAudio();
        Debug.Log($"[AudioManager] Stopped looping sounds on scene load: {scene.name}");
    }

    // === PLAYERS ===
    public void PlayBGM(string clipName, bool loop = true) {
        if (clipDict.TryGetValue(clipName, out AudioClip clip)) {
            Debug.Log($"[AudioManager] Playing BGM: {clipName}");
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
            lastPlayedClip = clip;
            activeSources[clipName] = bgmSource;
        } else {
            Debug.LogWarning($"[AudioManager] BGM not found: {clipName}");
        }
    }

    public void StopBGM() {
        if (bgmSource.isPlaying)
            bgmSource.Stop();
        bgmSource.clip = null;
        lastPlayedClip = null;
    }

    public void PlaySFX(string clipName) {
        if (clipDict.TryGetValue(clipName, out AudioClip clip)) {
            Debug.Log($"[AudioManager] Playing SFX: {clipName}");
            sfxSource.PlayOneShot(clip);
            lastPlayedClip = clip;
        }
    }

    public void PlayUI(string clipName) {
        if (clipDict.TryGetValue(clipName, out AudioClip clip)) {
            Debug.Log($"[AudioManager] Playing UI: {clipName}");
            uiSource.PlayOneShot(clip);
            lastPlayedClip = clip;
        }
    }

    public void PlayTransition(string clipName) {
        if (clipDict.TryGetValue(clipName, out AudioClip clip)) {
            Debug.Log($"[AudioManager] Playing Trans: {clipName}");
            sfxSource.PlayOneShot(clip);
            lastPlayedClip = clip;
        }
    }

    public void PlayMonsterAudio(string clipName, Vector3 position) {
        if (!clipDict.TryGetValue(clipName, out AudioClip clip)) {
            Debug.LogWarning($"[AudioManager] Monster SFX '{clipName}' not found!");
            return;
        }

        Debug.Log($"[AudioManager] Playing Monster: {clipName}");
        monsterSource.transform.position = position;
        monsterSource.Stop();
        monsterSource.clip = clip;
        monsterSource.Play();
        activeSources[clipName] = monsterSource;
    }

    public void StopMonsterAudio() {
        monsterSource.Stop();
        monsterSource.clip = null;
    }

    public void PlayLoopingSFX(string clipName) {
        if (clipDict.TryGetValue(clipName, out AudioClip clip)) {
            if (!IsSourcePlaying(clipName)) {
                Debug.Log($"[AudioManager] Playing Loop SFX: {clipName}");
                loopingSFXSource.clip = clip;
                loopingSFXSource.loop = true;
                loopingSFXSource.Play();
                activeSources[clipName] = loopingSFXSource;
            }
        } else {
            Debug.LogWarning($"[AudioManager] Looping SFX not found: {clipName}");
        }
    }

    public void StopLoopingSFX() {
        loopingSFXSource.Stop();
        loopingSFXSource.clip = null;
    }

    public void PlayShowerLooping(string clipName, bool loop = true) {
        if (clipDict.TryGetValue(clipName, out AudioClip clip)) {
            Debug.Log($"[AudioManager] Playing shower: {clipName}");
            showerloopingSource.clip = clip;
            showerloopingSource.loop = loop;
            showerloopingSource.Play();
            lastPlayedClip = clip;
            activeSources[clipName] = showerloopingSource;
        } else {
            Debug.LogWarning($"[AudioManager] Shower clip not found: {clipName}");
        }
    }

    public void StopShowerLooping() {
        if (showerloopingSource.isPlaying)
            showerloopingSource.Stop();
        showerloopingSource.clip = null;
        lastPlayedClip = null;
    }

    public bool IsPlaying() {
        return sfxSource.isPlaying || uiSource.isPlaying;
    }

    public bool IsSourcePlaying(string clipName) {
        if (activeSources.TryGetValue(clipName, out AudioSource source)) {
            return source != null && source.isPlaying;
        }
        return false;
    }
}
