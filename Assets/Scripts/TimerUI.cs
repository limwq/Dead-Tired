using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TimerUI : MonoBehaviour {
    [Header("Timer Settings")]
    public float survivalTime = 300f; // 5 minutes

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI alertText;
    //public Image alertImage;

    public float timer { get; private set; }
    private bool timerRunning = true;
    public bool timerFinished = false;

    // To prevent showing multiple alerts for the same time
    public bool alert3MinShown = false;
    public bool alert2MinShown = false;
    public bool alert1MinShown = false;

    public MonsterController monster;

    void Start() {
        timer = survivalTime;
        UpdateTimerUI();

        if (alertText != null) alertText.gameObject.SetActive(false);
    }

    void Update() {
        if (!timerRunning || timerFinished) return;

        timer -= Time.deltaTime;

        if (timer <= 0f && !monster.isJumpscaring) {
            timer = 0f;
            timerRunning = false;
            UpdateTimerUI();

            timerFinished = true;
            StartCoroutine(Win());
        }

        // === Check alerts ===
        if (timer <= 180f && !alert3MinShown) {
            alert3MinShown = true;
            StartCoroutine(ShowAlert("Too Slow!",0.25f));
        }

        if (timer <= 120f && !alert2MinShown) {
            alert2MinShown = true;
            StartCoroutine(ShowAlert("Try Harder!",0.5f));
        }

        if (timer <= 60f && !alert1MinShown) {
            alert1MinShown = true;
            if(AudioManager.Instance != null) {
                AudioManager.Instance.PlayBGM("tiktok clock lv1", true);
            }
            StartCoroutine(ShowAlert("Give Up Already!", 0.75f));
        }

        UpdateTimerUI();
    }

    IEnumerator Win() {
        Debug.Log("[TimerUI] Player wins! Timer complete.");
        AudioManager.Instance.StopMonsterAudio();
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlayTransition("Ding");

        yield return new WaitForSeconds(3f);
        if (SceneController.Instance != null) {
            SceneController.Instance.LoadNextScene();
        }
    }

    void UpdateTimerUI() {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timer / 60f) + 1;
        timerText.text = $"{minutes:00}:00";
    }

    public void StopTimer() {
        timerRunning = false;
    }

    IEnumerator ShowAlert(string message, float alphaValue) {

        if (alertText != null) {
            alertText.text = message;
            alertText.gameObject.SetActive(true);
        }

        // Fade IN both text and image
        float fadeDuration = 1f;
        float elapsed = 0f;

        while (elapsed < fadeDuration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            if (alertText != null) {
                Color tc = alertText.color;
                tc.a = Mathf.Lerp(0f, 0.5f, t);
                alertText.color = tc;
            }

            yield return null;
        }

        AudioManager.Instance.PlayTransition("monster sound1");

        // Hold for 3 seconds
        yield return new WaitForSeconds(3f);

        // Fade OUT text only (image stays until timer ends)
        elapsed = 0f;
        while (elapsed < fadeDuration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            if (alertText != null) {
                Color tc = alertText.color;
                tc.a = Mathf.Lerp(0.5f, 0f, t);
                alertText.color = tc;
            }

            yield return null;
        }

        if (alertText != null) {
            alertText.gameObject.SetActive(false);
        }

    }
}
