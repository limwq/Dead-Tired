using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour {
    public static SceneFader Instance;

    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    private bool isFading = false;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeImage != null) {
            // Start fully transparent
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.raycastTarget = false; // Don't block UI interactions when transparent
        }
    }

    public void FadeToScene(string sceneName) {
        if (!isFading) {
            StartCoroutine(FadeOut(sceneName));
        }
    }

    private IEnumerator FadeOut(string sceneName) {
        isFading = true;

        if (fadeImage == null) {
            Debug.LogError("[SceneFader] No fade image assigned!");
            yield break;
        }

        // Enable the image and make sure it's on top
        fadeImage.raycastTarget = true; // Block interactions during fade
        fadeImage.gameObject.SetActive(true);

        // Fade to black
        float t = 0f;
        while (t < fadeDuration) {
            t += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(t / fadeDuration));
            yield return null;
        }

        // Load next scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // Fade back in
        t = 0f;
        while (t < fadeDuration) {
            t += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0,  1- Mathf.Clamp01(t / fadeDuration));
            yield return null;
        }

        // Ensure it's completely transparent
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.raycastTarget = false; // Allow interactions again

        isFading = false;
    }
}
