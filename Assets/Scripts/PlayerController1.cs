using UnityEngine;
using System.Collections;

public class PlayerController1 : MonoBehaviour {
    // ===== View rotation =====
    private int currentViewIndex = 1; // 0 = Left, 1 = Center, 2 = Right
    private Quaternion[] viewRotations;
    private Quaternion targetRotation;
    private float turnDuration = 0.4f;

    // ===== Animators =====
    public Animator playerAnimator;
    public Animator cameraAnimator;

    // ===== Car Windows =====
    public CarWindowController leftWindow;
    public CarWindowController rightWindow;

    // ===== States =====
    public bool isCrouching { get; private set; }= false;
    public bool isPhoneOut = false;
    private bool isAnimationLocked = false;
    public bool isTurning = false;
    public bool phoneLockedByBoss = false;
    private bool hardLock = false;

    // ===== UI =====
    [SerializeField] private GameObject phone;
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject LeftViewUI;     
    [SerializeField] private GameObject CenterViewUI;    
    [SerializeField] private GameObject RightViewUI;
    [SerializeField] private GameObject CrouchUI;
    [SerializeField] private GameObject PhoneUI;

    void OnDisable() {
        LockForJumpscare(false);
        LockControls(false);
    }

    void Start() {
        // Pre-set rotations for Left, Center, Right
        viewRotations = new Quaternion[] {
            Quaternion.Euler(5, -90, 0), // Left
            Quaternion.Euler(0, 0, 0),   // Center
            Quaternion.Euler(5, 90, 0),   // Right
            Quaternion.Euler(0, 180, 0)  // Back
        };

        targetRotation = viewRotations[currentViewIndex];
        transform.localRotation = targetRotation;

        if (phone != null)
            phone.SetActive(false);
    }

    void Update() {
        SetActiveUI();

        if (isAnimationLocked || isTurning)
            return;

        // === Turn Left ===
        if (Input.GetKeyDown(KeyCode.A) && currentViewIndex > 0 && !isCrouching) {
            if (isPhoneOut) ExitPhone();
            StartCoroutine(TurnToView(currentViewIndex - 1));
        }

        // === Turn Right ===
        if (Input.GetKeyDown(KeyCode.D) && currentViewIndex < viewRotations.Length - 2 && !isCrouching) {
            if (isPhoneOut) ExitPhone();
            StartCoroutine(TurnToView(currentViewIndex + 1));
        }

        // === Interact (F) ===
        if (Input.GetKeyDown(KeyCode.F)) {
            if (isPhoneOut) ExitPhone();
            Interact();
        }
        if (Input.GetKeyUp(KeyCode.F)) {
            if (leftWindow != null) leftWindow.StopRollingUp();
            if (rightWindow != null) rightWindow.StopRollingUp();
        }

        // === Phone Toggle (E) ===
        if (Input.GetKeyDown(KeyCode.E) && !phoneLockedByBoss) {
            if (isPhoneOut) ExitPhone();
            else if (currentViewIndex == 1 && !isCrouching)
                EnterPhone(false);
        }

        
    }

    void SetActiveView(GameObject activeView) {
        // Ensure UI stays active
        UI.SetActive(true);

        // Disable all children first
        if (LeftViewUI != null) LeftViewUI.SetActive(false);
        if (CenterViewUI != null) CenterViewUI.SetActive(false);
        if (RightViewUI != null) RightViewUI.SetActive(false);
        if (CrouchUI != null) CrouchUI.SetActive(false);
        if (PhoneUI != null) PhoneUI.SetActive(false);


        // Enable the one we want
        if (activeView != null) activeView.SetActive(true);
    }

    void SetActiveUI() {
        if (isCrouching) {
            SetActiveView(CrouchUI);
        } else if (isPhoneOut) {
            SetActiveView(PhoneUI);
        } else {
            switch (currentViewIndex) {
                case 0:
                    SetActiveView(LeftViewUI);
                    break;
                case 1:
                    SetActiveView(CenterViewUI);
                    break;
                case 2:
                    SetActiveView(RightViewUI);
                    break;
                case 3:
                    SetActiveView(null);
                    break;
                default:
                    SetActiveView(CrouchUI);
                    break;

            }
        }
    }

    void Interact() {
        switch (currentViewIndex) {
            case 0: // Left view → Roll up left window
                if (leftWindow != null) {
                    leftWindow.RollWindowUp();
                }
                break;

            case 1: // Center view → Crouch toggle
                if (!isCrouching) {
                    PlayAnimationAndLock(playerAnimator, "TriggerCrouch");
                    isCrouching = true;
                } else {
                    PlayAnimationAndLock(playerAnimator, "TriggerExit");
                    isCrouching = false;
                }
                break;

            case 2: // Right view → Roll up right window
                if (rightWindow != null) {
                    rightWindow.RollWindowUp();
                }
                break;
        }
    }

    public Coroutine EnterPhone(bool lockAfter = false) {
        return StartCoroutine(EnterPhoneRoutine(lockAfter));
        
    }
    public IEnumerator EnterPhoneRoutine(bool lockAfter) {
        if (isCrouching) {
            PlayAnimationAndLock(playerAnimator, "TriggerExit");
            isCrouching = false;
        } else if (isTurning || currentViewIndex != 1) {
            yield return StartCoroutine(ForceLookPresetCoroutine(1));
            Debug.Log("done Force Look 1");
        }

        if (!isPhoneOut) {
            Debug.Log("entering phone");
            PlayAnimationAndLock(cameraAnimator, "TakeOutPhone");
            if (phone != null) { phone.SetActive(true); }
            isPhoneOut = true;
            Debug.Log("done entering phone");
        }
        // wait until animations are done (optional small delay)
        yield return new WaitForSeconds(0.2f);

        if (lockAfter) {
            LockControls(true);
            Debug.Log("locked controls");
        }

        Debug.Log("done enter phone coroutine");

    }

    public void ExitPhone() {
        isPhoneOut = false;
        PlayAnimationAndLock(cameraAnimator, "PutAwayPhone");
        if (phone != null) phone.SetActive(false);
        LockControls(false);
    }

    // ===== Animation Lock System =====
    void PlayAnimationAndLock(Animator animator, string triggerName) {
        StartCoroutine(PlayAnimationAndLockCoroutine(animator, triggerName));
    }

    IEnumerator PlayAnimationAndLockCoroutine(Animator animator, string triggerName) {
        isAnimationLocked = true;
        animator.SetTrigger(triggerName);

        yield return null; // wait a frame

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float clipLength = stateInfo.length > 0 ? stateInfo.length : 0.5f; // fallback

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
        if (isPhoneOut) ExitPhone();
        if (isCrouching) {            
            PlayAnimationAndLock(playerAnimator, "TriggerExit");
        }
        StartCoroutine(ForceLookPresetCoroutine(newIndex));
    }

    IEnumerator ForceLookPresetCoroutine(int newIndex) {
        if (isTurning) yield return null;

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
        currentViewIndex = newIndex;

        if (!hardLock) { // only unlock if not in jumpscare
            LockControls(false);
        }
    }


    public void LockControls(bool state) {
        if (!hardLock) { // only allow normal locks if not hard locked
            isAnimationLocked = state;
            isTurning = state;
        }
    }

    public void LockForJumpscare(bool state) {
        hardLock = state;
        isAnimationLocked = state;
        isTurning = state;
    }
}
