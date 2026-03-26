using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Video;

public class PauseManager : MonoBehaviour {
    public static PauseManager Instance;

    [Header("UI")]
    public GameObject pauseUI;
    public GameObject pauseBackground; // Panel that shows Resume/Restart/MainMenu

    private bool isPaused = false;
    private bool canPause = false;
    private bool isPressed = false;
    private Coroutine pauseCoroutine;
    private Coroutine resumeCoroutine;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        if (pauseBackground != null)
            pauseBackground.SetActive(false); // hidden at start

        isPressed = false;
    }

    private void Update() {
        HandlePauseUI();

        if (Input.GetKeyDown(KeyCode.Escape) && !isPressed) {
            if (canPause) {
                if (isPaused) ResumeGame();
                else PauseGame();
            }
        }
    }

    // Called by your Pause Button in the UI
    public void OnPauseButtonClicked() {
        if (!isPressed) {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame() {
        if (!isPressed) {
            // Stop any running resume coroutine
            if (resumeCoroutine != null) {
                StopCoroutine(resumeCoroutine);
                resumeCoroutine = null;
            }

            // Start pause coroutine
            pauseCoroutine = StartCoroutine(PauseGameRoutine());
        }
    }

    private IEnumerator PauseGameRoutine() {
        isPressed = true;

        // Play the button sound
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlayUI("button");

            // Wait until the audio is no longer playing
            yield return new WaitWhile(() => AudioManager.Instance.IsPlaying());
        }

        isPaused = true;
        Time.timeScale = 0f;

        AudioListener.pause = true;

        // Pause all VideoPlayers
        foreach (var vp in FindObjectsOfType<UnityEngine.Video.VideoPlayer>()) {
            if (vp.isPlaying) vp.Pause();
        }

        if (pauseBackground != null)
            pauseBackground.SetActive(true);

        pauseCoroutine = null;
        isPressed = false;
    }

    public void ResumeGame() {
        if (!isPressed) {
            // Stop any running pause coroutine
            if (pauseCoroutine != null) {
                StopCoroutine(pauseCoroutine);
                pauseCoroutine = null;
            }

            // Start resume coroutine
            resumeCoroutine = StartCoroutine(ResumeGameRoutine());
        }
    }

    private IEnumerator ResumeGameRoutine() {
        isPressed = true;
        AudioListener.pause = false;

        // Play the button sound
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlayUI("button");

            // Wait until the audio is no longer playing
            yield return new WaitWhile(() => AudioManager.Instance.IsPlaying());
        }

        isPaused = false;
        Time.timeScale = 1f;

        // Resume all VideoPlayers
        foreach (var vp in FindObjectsOfType<UnityEngine.Video.VideoPlayer>()) {
            vp.Play();
        }

        if (pauseBackground != null)
            pauseBackground.SetActive(false);

        resumeCoroutine = null;
        isPressed = false;
    }

    public void RestartGame() {
        if (!isPressed) {
            // Start restart coroutine
            StartCoroutine(RestartGameRoutine());
        }
    }

    private IEnumerator RestartGameRoutine() {
        isPressed = true;

        AudioListener.pause = false;

        // Resume all VideoPlayers
        foreach (var vp in FindObjectsOfType<UnityEngine.Video.VideoPlayer>()) {
            vp.Play();
        }

        // Play the button sound
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlayUI("button");

            // Wait until the audio is no longer playing
            yield return new WaitWhile(() => AudioManager.Instance.IsPlaying());
        }

        // Reset pause state
        isPaused = false;
        Time.timeScale = 1f;

        // Hide pause background
        if (pauseBackground != null)
            pauseBackground.SetActive(false);

        // Restart the current scene
        SceneController.Instance.RestartCurrentScene();
    }

    public void QuitToMainMenu() {
        if (!isPressed) {
            // Start quit coroutine
            StartCoroutine(QuitToMainMenuRoutine());
        }
    }

    private IEnumerator QuitToMainMenuRoutine() {
        isPressed = true;

        AudioListener.pause = false;

        // Resume all VideoPlayers
        foreach (var vp in FindObjectsOfType<UnityEngine.Video.VideoPlayer>()) {
            vp.Play();
        }

        // Play the button sound
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlayUI("button");

            // Wait until the audio is no longer playing
            yield return new WaitWhile(() => AudioManager.Instance.IsPlaying());
        }

        // Reset pause state
        isPaused = false;
        Time.timeScale = 1f;

        // Hide pause background
        if (pauseBackground != null)
            pauseBackground.SetActive(false);

        // Load the main menu scene
        SceneController.Instance.LoadMainMenu();
    }

    public void SetBackgroundtoFalse() {
        if (pauseBackground != null)
            pauseBackground.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public bool IsPaused() {
        return isPaused;
    }

    void HandlePauseUI() {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "MainMenu" ||
            currentScene == "ChapterChoose" ||
            currentScene == "FailScene" ||
            currentScene == "NextScene" ||
            currentScene == "EndScene") {

            if (pauseUI != null)
                pauseUI.SetActive(false);

            Time.timeScale = 1f; // always reset time in non-play scenes
            isPaused = false;
            canPause = false;
        } else {
            if (pauseUI != null)
                pauseUI.SetActive(true);
            canPause = true;
        }
    }

    // Handle scene changes to reset pause state
    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // Reset pause state when a new scene is loaded
        isPaused = false;
        isPressed = false;
        Time.timeScale = 1f;
        if (pauseBackground != null)
            pauseBackground.SetActive(false);
    }
}