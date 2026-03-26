using UnityEngine;
using System.Collections;

public class MonsterController3 : MonoBehaviour {
    [Header("References")]
    public SoreProgressManager soreProgress;
    public Chap3Window windowScript;
    public BathroomLight lightScript;
    public ShowerProgress showerprogress;

    public PlayerController3 player;       // assign PlayerController3
    public Animator monsterAnimator;       // assign monster Animator
    public GameObject monsterModel;        // monster prefab


    [Header("Jumpscare Settings")]
    public float windowTimeLightOn = 20f;  // slower when lights ON
    public float windowTimeLightOff = 15f; // faster when lights OFF

    private float windowTimer = 0f;
    public bool jumpscareTriggered = false;

    void Update() {
        if (jumpscareTriggered) return;

        // === Condition 1: Eye sore reaches 100% ===
        if (soreProgress != null && soreProgress.GetSoreProgress() >= 100f) {
            Debug.Log("[MonsterController3] Jumpscare because sore progress reached 100%!");
            TriggerJumpscare();
            return;
        }

        // === Condition 2: Monster at window ===
        if (windowScript != null && windowScript.isAppearing) {
            windowTimer += Time.deltaTime;

            float requiredTime = (lightScript != null && lightScript.isTurnedOn)
                ? windowTimeLightOn   // Lights ON → 20s
                : windowTimeLightOff; // Lights OFF → 15s

            float remainingTime = Mathf.Max(0, requiredTime - windowTimer);
            Debug.Log($"[MonsterController3] Monster at window. Time left before jumpscare: {remainingTime:F1}s (Required: {requiredTime}s)");

            if (windowTimer >= requiredTime) {
                Debug.Log("[MonsterController3] Jumpscare because monster stayed too long at window!");
                TriggerJumpscare();
            }
        } else {
            if (windowTimer > 0f)
                Debug.Log("[MonsterController3] Monster left window, timer reset.");
            windowTimer = 0f;
        }
    }

    public void TriggerJumpscare() {
        if (jumpscareTriggered || showerprogress.hasWon) return;
        jumpscareTriggered = true;

        Debug.Log("[MonsterController3] Jumpscare triggered!");

        // === Hide sore overlay ===
        if (soreProgress != null && soreProgress.soreOverlay != null) {
            Color c = soreProgress.soreOverlay.color;
            c.a = 0f; // fully transparent
            soreProgress.soreOverlay.color = c;
        }

        // === Show Monster ===
        if (monsterModel != null) {
            monsterModel.SetActive(true);            
        }

        // === Play Audio ===
        AudioManager.Instance.StopShowerLooping();
        AudioManager.Instance.StopLoopingSFX();
        AudioManager.Instance.PlayMonsterAudio("monster-roar-1", player.transform.position);

        // === Play Animation ===
        if (monsterAnimator != null) {
            monsterAnimator.SetTrigger("JS");
        }

        // === Force Player Look ===
        if (player != null) {
            player.ForceLookPreset(3); // 👈 pass the index for -15° preset
        }

        StartCoroutine(JumpscareRoutine());
    }

    IEnumerator JumpscareRoutine() {
        Debug.Log("Loading FailScene...");
        yield return new WaitForSeconds(3f); // let animation play
        SceneController.Instance.LoadFailScene();
    }
}
