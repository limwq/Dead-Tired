using UnityEngine;
using System.Collections;

public class PlayerController3 : MonoBehaviour {
    // ===== View rotation =====
    private int currentViewIndex = 1; // 0 = Left, 1 = Center, 2 = Right
    private Quaternion[] viewRotations;
    private Quaternion targetRotation;
    private float turnDuration = 0.4f;

    // ===== Animators =====
    public Animator playerAnimator;
    public Animator cameraAnimator;

    // ===== States =====
    public bool isAnimationLocked = false;
    public bool isTurning = false;

    // ===== UI =====
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject LeftViewUI;
    [SerializeField] private GameObject CenterViewUI;
    [SerializeField] private GameObject RightViewUI;

    // ===== References =====
    public BathroomLight bathroomLighting;
    public ShowerOnOff water;
    public ShowerProgress showerProgress;
    public Chap3Window windowMonster;
    public SoreProgressManager soreProgress;
    public MonsterController3 monsterController3;

    void Start() {
        // Pre-set rotations for Left, Center, Right
        viewRotations = new Quaternion[] {
            Quaternion.Euler(-15, -90, 0), // Left
            Quaternion.Euler(0, 0, 0),     // Center
            Quaternion.Euler(5, 90, 0),    // Right
            Quaternion.Euler(-15, 0, 0),   //JumpScare
        };

        targetRotation = viewRotations[currentViewIndex];
        transform.localRotation = targetRotation;
    }

    void Update() {
        if (isAnimationLocked || isTurning)
            return;

        // === Turn Left ===
        if (Input.GetKeyDown(KeyCode.A) && currentViewIndex > 0) {
            StartCoroutine(TurnToView(currentViewIndex - 1));
        }

        // === Turn Right ===
        if (Input.GetKeyDown(KeyCode.D) && currentViewIndex < viewRotations.Length - 2) {
            StartCoroutine(TurnToView(currentViewIndex + 1));
        }

        // === Interact (F) ===
        if (Input.GetKeyDown(KeyCode.F)) {
            if (!showerProgress.hasWon) {
                TryInteract();
            }
        }

        // === Interact (F) ===
        if (Input.GetKeyUp(KeyCode.F)) {
            if (showerProgress != null || monsterController3.jumpscareTriggered)
                showerProgress.StopShower();
        }

        SetActiveUI();
    }

    void SetActiveView(GameObject activeView) {
        // Ensure UI stays active
        UI.SetActive(true);

        // Disable all children first
        if (LeftViewUI != null) LeftViewUI.SetActive(false);
        if (CenterViewUI != null) CenterViewUI.SetActive(false);
        if (RightViewUI != null) RightViewUI.SetActive(false);

        // Enable the one we want
        if (activeView != null) activeView.SetActive(true);
    }

    void SetActiveUI() {
        switch (currentViewIndex) {
            case 0: SetActiveView(LeftViewUI); break;
            case 1: SetActiveView(CenterViewUI); break;
            case 2: SetActiveView(RightViewUI); break;
            case 3: SetActiveView(null); break;
        }
    }

    void TryInteract() {
        // 👀 check sore progress
        if (soreProgress != null && soreProgress.GetSoreProgress() >= 50f) {
            if (Random.value < 0.5f) { // 50% chance fail
                Debug.LogWarning("[PlayerController3] Interaction FAILED due to sore progress being high!");
                return; // ❌ stop interaction
            }
        }

        // ✅ if not failed → do interaction
        Interact();
    }

    void Interact() {
        switch (currentViewIndex) {
            case 0: // Left view → control window
                Debug.Log("Interact with window");
                if (windowMonster.isAppearing) {
                    if (bathroomLighting != null && !bathroomLighting.isTurnedOn) {
                        // 🚨 Lights are off + monster at window → instant jumpscare
                        Debug.LogWarning("[PlayerController3] Monster jumpscare triggered due to interacting in the dark!");
                        if (monsterController3 != null) {
                            monsterController3.TriggerJumpscare();
                        }
                    } else {
                        // Normal retreat if light is on
                        windowMonster.ToggleWindow(false);
                        Debug.Log("Monster Retreat!");
                    }
                }
                break;

            case 1: // Center view → control shower
                Debug.Log("Interact with shower");
                if (!water.isTurnedOn) {
                    water.ToggleWater(true);
                    Debug.Log("Turn on water");

                    
                } else {
                    if (showerProgress != null)
                        showerProgress.StartShower();
                }
                break;

            case 2: // Right view → control light
                Debug.Log("Interact with light");
                if (!bathroomLighting.isTurnedOn) {
                    bathroomLighting.OffLight(true);
                }
                break;
        }
    }

    // ===== Animation Lock System =====
    void PlayAnimationAndLock(Animator animator, string triggerName) {
        StartCoroutine(PlayAnimationAndLockCoroutine(animator, triggerName));
    }

    IEnumerator PlayAnimationAndLockCoroutine(Animator animator, string triggerName) {
        animator.SetTrigger(triggerName);
        yield return null; // wait a frame
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float clipLength = stateInfo.length;
        isAnimationLocked = true;
        yield return new WaitForSeconds(clipLength);
        isAnimationLocked = false;
    }

    IEnumerator TurnToView(int newIndex) {
        isTurning = true;
        Quaternion startRot = transform.localRotation;
        Quaternion endRot = viewRotations[newIndex];
        currentViewIndex = newIndex;

        float elapsed = 0f;
        while (elapsed < turnDuration) {
            elapsed += Time.deltaTime;
            float t = elapsed / turnDuration;
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        transform.localRotation = endRot;
        isTurning = false;
    }

    public void ForceLookPreset(int newIndex) {
        StartCoroutine(ForceLookPresetCoroutine(newIndex));
    }

    IEnumerator ForceLookPresetCoroutine(int newIndex) {
        LockControls(true);
        Quaternion startRot = transform.localRotation;
        Quaternion endRot = viewRotations[newIndex];

        float elapsed = 0f;
        while (elapsed < turnDuration) {
            elapsed += Time.deltaTime;
            float t = elapsed / turnDuration;
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        transform.localRotation = endRot;
    }

    public void LockControls(bool state) {
        isAnimationLocked = state;
        isTurning = state;
    }
}
