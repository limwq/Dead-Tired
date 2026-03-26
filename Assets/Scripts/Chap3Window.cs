using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;

public class Chap3Window : MonoBehaviour {
    [Header("References")]
    public GameObject monsterHand;   // assign in Inspector
    public Transform windowPivot;    // assign pivot of the window

    [Header("Settings")]
    public float minOnTime = 20f;
    public float maxOnTime = 25f;
    public float windowOpenAngle = -45f;  // Z rotation for open
    public float rotateSpeed = 3f;        // smooth rotate speed

    [HideInInspector]
    public bool isAppearing = false;

    private Coroutine rotateRoutine;
    public ShowerProgress haswin;

    void Start() {
        isAppearing = false;

        // make sure hand & window start closed
        if (monsterHand != null) monsterHand.SetActive(false);
        if (windowPivot != null) windowPivot.localRotation = Quaternion.Euler(0, 0, 0);

        StartCoroutine(WindowRoutine());
    }

    private void Update() {
        if (haswin.hasWon) {
            if (AudioManager.Instance != null) {
                AudioManager.Instance.StopMonsterAudio();
            }
        }
    }

    IEnumerator WindowRoutine() {
        while (true) {
            // Wait for cooldown (only while monster hidden)
            float cooldown = UnityEngine.Random.Range(minOnTime, maxOnTime);
            Debug.Log($"[Chap3Window] Waiting {cooldown:F1}s before monster can appear again...");
            yield return new WaitForSeconds(cooldown);

            // Monster appears
            ToggleWindow(true);

            // ⏸ Wait until player closes it
            yield return new WaitUntil(() => isAppearing == false);

            Debug.Log("[Chap3Window] Monster retreated, restarting cooldown...");
        }
    }

    public void ToggleWindow(bool state) {
        if (monsterHand == null || windowPivot == null || haswin.hasWon) return;

        isAppearing = state;

        // toggle monster hand
        monsterHand.SetActive(state);

        // rotate window pivot smoothly
        if (rotateRoutine != null) StopCoroutine(rotateRoutine);
        rotateRoutine = StartCoroutine(RotateWindow(state ? windowOpenAngle : 0f));

        if (AudioManager.Instance != null && state == true) {
            AudioManager.Instance.PlaySFX("window-open");
            AudioManager.Instance.PlayMonsterAudio("Monster3 Appear", monsterHand.transform.position);
        } else if (AudioManager.Instance != null && state == false) {
            AudioManager.Instance.PlaySFX("window-close");
            AudioManager.Instance.PlayMonsterAudio("monster scream 2", monsterHand.transform.position);
        }

        Debug.Log(state ? "Monster Appears & Window Opens" : "Monster Retreats & Window Closes");

    }

    IEnumerator RotateWindow(float targetAngle) {
        Quaternion startRot = windowPivot.localRotation;
        Quaternion endRot = Quaternion.Euler(0, 0, targetAngle);

        float t = 0f;
        while (t < 1f) {
            t += Time.deltaTime * rotateSpeed;
            windowPivot.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        windowPivot.localRotation = endRot;
    }
}
