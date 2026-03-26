using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Threading;

public class ShowerProgress : MonoBehaviour {
    [Header("UI")]
    public Slider progressSlider;   // assign your shower progress slider here

    [Header("Settings")]
    public float increaseRate = 1f; // speed of progress when shower is ON
    public float decreaseRate = 0f; // speed of decrease when shower is OFF

    public ShowerOnOff water;
    public BathroomLight bathroomlight;
    public Chap3Window window;
    public SoreProgressManager sore;
    public MonsterController3 monster;

    // To prevent showing multiple alerts for the same time
    public bool alert25Shown = false;
    public bool alert50Shown = false;
    public bool alert75Shown = false;
    public TextMeshProUGUI alertText;


    private bool isRunning = false;
    public bool hasWon { get; private set; } = false;

    void Start() {
        if (progressSlider != null) {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 100f;
            progressSlider.value = 0f;
        }

        if (alertText != null) alertText.gameObject.SetActive(false);
    }

    void Update() {
        if (progressSlider == null || hasWon) return;

        if (!water.isTurnedOn) {
            decreaseRate = 2f;
        } else {
            decreaseRate = 0f;
        }

        if (isRunning) {
            // Increase progress when shower ON
            progressSlider.value += increaseRate * Time.deltaTime;
        } else {
            // Decrease progress when shower OFF
            progressSlider.value -= decreaseRate * Time.deltaTime;
        }

        // Clamp between 0 and 100
        progressSlider.value = Mathf.Clamp(progressSlider.value, 0f, progressSlider.maxValue);

        if (progressSlider.value >= 25f && !alert25Shown) {
            alert25Shown = true;
            water.minOnTime = 5f;
            water.maxOnTime = 10f;
            bathroomlight.minOnTime = 5f;
            bathroomlight.maxOnTime = 10f;
            window.minOnTime = 10f;
            window.maxOnTime = 15f;
            monster.windowTimeLightOn = 18f;
            monster.windowTimeLightOff = 12f;
            sore.showerOffDelay = 3f;
            StartCoroutine(ShowAlert("Seems like its too easy!"));
        }
        if (progressSlider.value >= 50f && !alert50Shown) {
            alert50Shown = true;
            water.minOnTime = 5f;
            water.maxOnTime = 8f;
            bathroomlight.minOnTime = 5f;
            bathroomlight.maxOnTime = 8f;
            window.minOnTime = 8f;
            window.maxOnTime = 10f;
            monster.windowTimeLightOn = 15f;
            monster.windowTimeLightOff = 10f;
            sore.showerOffDelay = 2.5f;
            StartCoroutine(ShowAlert("Well Done! But its not over yet!"));
        }
        if (progressSlider.value >= 75f && !alert75Shown) {
            alert75Shown = true;
            water.minOnTime = 5f;
            water.maxOnTime = 8f;
            bathroomlight.minOnTime = 5f;
            bathroomlight.maxOnTime = 8f;
            window.minOnTime = 5f;
            window.maxOnTime = 8f;
            monster.windowTimeLightOn = 10f;
            monster.windowTimeLightOff = 8f;
            sore.showerOffDelay = 2f;

            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlayBGM("lv3 bgm", true);
            }

            StartCoroutine(ShowAlert("Faster! Faster!"));

            
        }

        // Win condition
        if (progressSlider.value >= progressSlider.maxValue && !monster.jumpscareTriggered) {
            hasWon = true;
            StartCoroutine(Win());
        }
    }

    IEnumerator Win() {
        Debug.Log("[ShowerProgress] Player wins! Shower complete.");
        AudioManager.Instance.StopMonsterAudio();
        AudioManager.Instance.StopLoopingSFX();
        AudioManager.Instance.StopShowerLooping();
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlayTransition("Ding");
        yield return new WaitForSeconds(3f);
        if (SceneController.Instance != null)
            SceneController.Instance.LoadEndScene();
    }

    public void StartShower() {
        isRunning = true;
        Debug.Log("StartShower");

        // 🔊 Play wash hair sound (looping SFX)
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlayLoopingSFX("WashHair");
        }
    }

    public void StopShower() {
        isRunning = false;
        Debug.Log("StoppedShower");

        // 🔇 Stop wash hair sound
        if (AudioManager.Instance != null) {
            AudioManager.Instance.StopLoopingSFX();
        }
    }

    IEnumerator ShowAlert(string message) {
        AudioManager.Instance.PlayTransition("monster sound1");

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
                tc.a = Mathf.Lerp(0f, 1f, t);
                alertText.color = tc;
            }

            yield return null;
        }


        // Hold for 3 seconds
        yield return new WaitForSeconds(3f);

        // Fade OUT text only (image stays until timer ends)
        elapsed = 0f;
        while (elapsed < fadeDuration) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            if (alertText != null) {
                Color tc = alertText.color;
                tc.a = Mathf.Lerp(1f, 0f, t);
                alertText.color = tc;
            }

            yield return null;
        }

        if (alertText != null) {
            alertText.gameObject.SetActive(false);
        }

    }
}
