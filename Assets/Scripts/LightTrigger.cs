using UnityEngine;
using System.Collections;

public class LightTrigger : MonoBehaviour {
    [Header("Light Settings")]
    public Light spotLight; // assign in Inspector
    public float minOnTime = 2f;   // minimum seconds light stays on
    public float maxOnTime = 5f;   // maximum seconds light stays on
    public float minOffTime = 1f;  // minimum seconds light stays off
    public float maxOffTime = 4f;  // maximum seconds light stays off

    [Header("Trigger States")]
    public bool playerInside = false;
    public bool monsterInside = false;

    [Header("Audio")]
    public AudioSource audioSource;   // 🎵 assign AudioSource in Inspector
    public AudioClip onClip;          // sound when light turns ON
    public AudioClip offClip;         // sound when light turns OFF

    void Start() {
        if (spotLight != null) {
            spotLight.enabled = false;
            StartCoroutine(FlickerRoutine());
        }

        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>(); // fallback
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
            playerInside = true;
        if (other.CompareTag("Monster"))
            monsterInside = true;
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
            playerInside = false;
        if (other.CompareTag("Monster"))
            monsterInside = false;
    }

    IEnumerator FlickerRoutine() {
        while (true) {
            // Turn ON the light
            spotLight.enabled = true;
            PlayClip(onClip);
            float onTime = UnityEngine.Random.Range(minOnTime, maxOnTime);
            yield return new WaitForSeconds(onTime);

            // Turn OFF the light
            spotLight.enabled = false;
            PlayClip(offClip);
            float offTime = UnityEngine.Random.Range(minOffTime, maxOffTime);
            yield return new WaitForSeconds(offTime);
        }
    }

    void PlayClip(AudioClip clip) {
        if (audioSource != null && clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }

    // ✅ Methods for external checks
    public bool IsMonsterInside() => monsterInside;
    public bool IsPlayerInsideActive() => playerInside && spotLight != null && spotLight.enabled;
    public bool IsMonsterInsideActive() => monsterInside && spotLight != null && spotLight.enabled;
}
