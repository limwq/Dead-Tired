using TMPro;
using UnityEngine;
using System.Collections;

public class AdjustAllLightsOnTrigger : MonoBehaviour
{
    [Header("New Light Timings")]
    public float minOnTime = 1f;
    public float maxOnTime = 3f;
    public float minOffTime = 0.5f;
    public float maxOffTime = 1.5f;

    [Header("Alert Text")]
    public TextMeshProUGUI alertText;
    public string Text;

    [Header("Turn Off All Lights Instead?")]
    public bool turnOffLights = false;

    [Header("Excluded Lights (Will Not Be Turned Off)")]
    public LightTrigger[] excludedLights; // ✅ Drag lights you want to keep ON here

    [Header("Adjust Monster Speed")]
    public Monster2AI monster;
    public float speed;

    [Header("Adjust Player Speed")]
    public ForwardMovement player;
    public bool changePlayerSpeed = false;
    public float playerspeed;

    private bool triggered = false;
    private void Start() {
        if (alertText != null) alertText.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // Find all LightTriggers in the scene
        LightTrigger[] allLights = FindObjectsOfType<LightTrigger>();

        foreach (var lightTrigger in allLights)
        {
            if (lightTrigger == null) continue;

            // ✅ Skip excluded lights
            if (IsExcluded(lightTrigger)) continue;

            if (turnOffLights)
            {
                // ✅ Stop the coroutine completely and disable the light
                lightTrigger.StopAllCoroutines();
                if (lightTrigger.spotLight != null)
                    lightTrigger.spotLight.enabled = false;

                if (AudioManager.Instance != null) {
                    AudioManager.Instance.PlayBGM("lv2 chase roar", true);
                }
            }
            else
            {
                // ✅ Update flicker timings
                lightTrigger.minOnTime = minOnTime;
                lightTrigger.maxOnTime = maxOnTime;
                lightTrigger.minOffTime = minOffTime;
                lightTrigger.maxOffTime = maxOffTime;
            }
        }

        if(Text != null) { 
            StartCoroutine(ShowAlert(Text));
        }


        if(monster != null) {
            ChangeMonsterSpeed(speed);
        }


        if (changePlayerSpeed) {
            ChangePlayerSpeed(playerspeed);
        }

    }

    bool IsExcluded(LightTrigger lightTrigger)
    {
        if (excludedLights == null || excludedLights.Length == 0)
            return false;

        foreach (var excluded in excludedLights)
        {
            if (excluded == lightTrigger)
                return true;
        }
        return false;
    }

    IEnumerator ShowAlert(string message) {
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlayMonsterAudio("monster sound1", transform.position);
        }

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

    private void ChangeMonsterSpeed(float monsterspeed) {
        Debug.Log($"Change Monster Speed to {monsterspeed}");
        monster.moveSpeed = monsterspeed;
        Debug.Log($"{monster.moveSpeed}");
    }

    private void ChangePlayerSpeed(float speed) {
        Debug.Log($"Change Player Speed to {speed}");
        player.speed = speed;
    }
}
