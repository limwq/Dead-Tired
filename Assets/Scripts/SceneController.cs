using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneController : MonoBehaviour {
    public static SceneController Instance;

    public string lastSceneName;

    private bool isLoading;
    // 🔹 Ordered list of story scenes
    private readonly List<string> sceneOrder = new List<string> {
        "Chap1Trans",
        "Chap1Car",
        "Chap2Trans",
        "Chap2Street",
        "Chap3Trans",
        "Chap3Bath",
    };
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Hook into Unity's sceneLoaded event once
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void SaveLastScene() {
        lastSceneName = SceneManager.GetActiveScene().name;
    }

    // === 🔹 Get next scene based on current/last scene
    public string GetNextScene(string currentScene) {
        int index = sceneOrder.IndexOf(currentScene);
        if (index >= 0 && index < sceneOrder.Count - 1) {
            return sceneOrder[index + 1];
        }
        return null; // no next scene (end of game)
    }

    // === Core Loading ===
    public void LoadScene(string sceneName) {
        if (isLoading) return; // Prevent multiple simultaneous loads

        isLoading = true;

        if (SceneFader.Instance != null) {
            SceneFader.Instance.FadeToScene(sceneName);
            // Reset loading state after a delay to allow fade to complete
            StartCoroutine(ResetLoadingState(SceneFader.Instance.fadeDuration * 2));
        } else {
            Debug.Log("No fader found, loading directly");
            SceneManager.LoadScene(sceneName); // fallback
            isLoading = false;
        }
    }

    private IEnumerator ResetLoadingState(float delay) {
        yield return new WaitForSeconds(delay);
        isLoading = false;
    }

    public void RestartCurrentScene() {
        string current = SceneManager.GetActiveScene().name;
        Debug.Log($"[SceneController] Restarting {current}");
        LoadScene(current);
    }

    public void RestartLastScene() {
        if (!string.IsNullOrEmpty(lastSceneName)) {
            LoadScene(lastSceneName);
        } else {
            Debug.LogWarning("[SceneController] No last scene saved → MainMenu");
            LoadScene("MainMenu");
        }
    }

    // === Generalized Scene Loading ===
    public void LoadSceneByName(string sceneName) {
        if (!string.IsNullOrEmpty(sceneName)) {
            LoadScene(sceneName);
        } else {
            Debug.LogError("[SceneController] Invalid scene name!");
        }
    }

    // === Scene Loaded Callback (BGM control) ===
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Debug.Log($"[SceneController] Scene loaded: {scene.name}");

        if (AudioManager.Instance == null) return;

        switch (scene.name) {
            case "MainMenu":
                AudioManager.Instance.PlayBGM("creepy bgm", true);
                break;

            case "Chap1Car":
                AudioManager.Instance.PlayBGM("inside-car-noise-while-driving-29978", true);
                break;

            case "Chap2Street":
                AudioManager.Instance.PlayBGM("ambience", true);
                break;

            case "Chap3Bath":
                //AudioManager.Instance.PlayBGM("Shower", true);
                break;

            case "ChapterChoose":
                AudioManager.Instance.PlayBGM("creepy bgm", true);
                break;

            case "FailScene":
                AudioManager.Instance.PlayBGM("creepy bgm", true);
                break;

            case "NextScene":
                AudioManager.Instance.PlayBGM("creepy bgm", true);
                break;

            default:
                AudioManager.Instance.StopBGM(); // stop music if scene has no bgm
                break;
        }
    }

    // === Common Shortcuts ===
    public void LoadMainMenu() => LoadScene("MainMenu");
    public void LoadVictoryScene() { SaveLastScene(); LoadScene("VictoryScene"); }
    public void LoadEndScene() { SaveLastScene(); LoadScene("EndScene"); }
    public void LoadFailScene() { SaveLastScene(); LoadScene("FailScene"); }

    public void LoadNextScene() { SaveLastScene(); LoadScene("NextScene"); }
    public void ChapterChoose() { SaveLastScene(); LoadScene("ChapterChoose"); }

    public void Chap1Trans() { SaveLastScene(); LoadScene("Chap1Trans");}
    public void Chap2Trans() { SaveLastScene(); LoadScene("Chap2Trans");}
    public void Chap3Trans() { SaveLastScene(); LoadScene("Chap3Trans");}

    public void Chap1Car() { SaveLastScene(); LoadScene("Chap1Car"); 
    }
    public void Chap2Street() { SaveLastScene(); LoadScene("Chap2Street"); }
    public void Chap3Bath() { SaveLastScene(); LoadScene("Chap3Bath"); }


}
