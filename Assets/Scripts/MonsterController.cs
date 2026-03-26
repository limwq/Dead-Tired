using UnityEngine;
using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using System.Timers;

public class MonsterController : MonoBehaviour {
    public CarWindowController leftWindow;
    public CarWindowController rightWindow;
    public GameObject MonsterModel;

    [Header("Monster Settings")]
    public float windowOpenSpeed = 0.05f;
    public float attackCooldown = 5f;
    public float bangInterval = 3f;

    [Header("Player State")]
    public PlayerController1 player;

    [Header("Monster Animation")]
    public Animator monsterAnimator;

    [Header("Attack Points")]
    public Transform attackPointLeft;
    public Transform attackPointRight;
    public Transform attackPointBack;
    public Transform resetPoint;

    private enum AttackType { LeftWindow, RightWindow, Back }
    private AttackType currentAttack;
    private bool isAttacking = false;
    private bool windowForceStop = false;
    public bool isJumpscaring = false;
    private Vector3 spawnPos;
    private TimerUI timerUI;

    [Header("Light")]
    public Light redlight;

    void Start() {
        MonsterModel.SetActive(false);

        if (redlight != null) {
            redlight.enabled = false;
        }

        leftWindow.OnWindowClosed += HandleWindowClosed;
        rightWindow.OnWindowClosed += HandleWindowClosed;
        leftWindow.OnWindowOpened += HandleWindowOpened;
        rightWindow.OnWindowOpened += HandleWindowOpened;

        timerUI = FindFirstObjectByType<TimerUI>();

        StartCoroutine(MonsterAttackLoop());
    }

    private void Update() {
        if (timerUI != null && timerUI.alert3MinShown && !isJumpscaring) {
            attackCooldown = 5f;
            windowOpenSpeed = 0.1f;
            bangInterval = 2f;
        }
        
        if (timerUI != null && timerUI.alert2MinShown && !isJumpscaring) {
            attackCooldown = 3f;
            windowOpenSpeed = 0.12f;
            bangInterval = 2f;
        }

        if (timerUI != null && timerUI.alert1MinShown && !isJumpscaring) {
            attackCooldown = 2f;
            windowOpenSpeed = 0.15f;
            bangInterval = 1f;
        }
    }

    private void HandleWindowOpened(CarWindowController window) {
        Debug.Log($"Monster jump scare from {window.name}!");
        windowForceStop = true;
        window.StopMonsterOpen();

        if (monsterAnimator != null && !timerUI.timerFinished) {
            if (window == leftWindow) {
                // base position = attack point
                spawnPos = attackPointLeft.position;

                // add forward offset (tweak this value!)
                float forwardOffset = 4f; 
                spawnPos += attackPointLeft.forward * forwardOffset;

                // apply position & rotation
                MonsterModel.transform.position = spawnPos;
                MonsterModel.transform.rotation = attackPointLeft.rotation;
            } else { 
                // base position = attack point
                spawnPos = attackPointRight.position;

                // add forward offset (tweak this value!)
                float forwardOffset = 4f; 
                spawnPos += attackPointRight.forward * forwardOffset;

                // apply position & rotation
                MonsterModel.transform.position = spawnPos;
                MonsterModel.transform.rotation = attackPointRight.rotation;
            }

            monsterAnimator.SetTrigger("JS");
            isJumpscaring = true;

            // force player look
            if (window == leftWindow) {
                player.ForceLookPreset(0); // force look LEFT
            } else if (window == rightWindow) {
                player.ForceLookPreset(2); // force look RIGHT
            }

            // hard lock controls during jumpscare
            player.LockForJumpscare(true);

            AudioManager.Instance.PlayMonsterAudio("monster-roar-1", spawnPos);

            Debug.Log("Player failed! Loading Fail Scene...");
            StartCoroutine(GameOver());
 
        }
    }

    private void HandleWindowClosed(CarWindowController window) {
        Debug.Log($"Monster retreats from {window.name}!");
        windowForceStop = true;
        window.StopMonsterOpen();
        player.LockForJumpscare(false);
        monsterAnimator.SetTrigger("Exit");
        AudioManager.Instance.PlayMonsterAudio("monster scream 2", spawnPos);
        StartCoroutine(HideAfterAnimation()); // 👈 auto hide after anim
    }

    IEnumerator MonsterAttackLoop() {
        while (true) {
            yield return new WaitForSeconds(attackCooldown);
            ChooseRandomAttack();

            // ⏸ Wait until player clear it
            yield return new WaitUntil(() => isAttacking == false);

            Debug.Log("[MonsterController] Attack Completed, restarting attackCooldown...");
        }
    }

    void ChooseRandomAttack() {
        if (isAttacking) return;

        currentAttack = (AttackType)Random.Range(0, 3);
        isAttacking = true;

        switch (currentAttack) {
            case AttackType.LeftWindow:
                StartCoroutine(WindowAttack(leftWindow, attackPointLeft));
                break;
            case AttackType.RightWindow:
                StartCoroutine(WindowAttack(rightWindow, attackPointRight));
                break;
            case AttackType.Back:
                StartCoroutine(BackAttack());
                break;
        }
    }

    IEnumerator WindowAttack(CarWindowController targetWindow, Transform attackPoint) {
        Debug.Log($"Monster attacking {targetWindow.name}");
        windowForceStop = false;

        AudioManager.Instance.PlayMonsterAudio("monster arrive hint2", attackPoint.position);
        // Wait until the audio is no longer playing
        yield return new WaitWhile(() => AudioManager.Instance.IsSourcePlaying("monster arrive hint2"));


        // Move monster to attack position
        if (attackPoint != null) {
            Debug.Log($"Monster spawn at {attackPoint.name}");
            MonsterModel.SetActive(true); // 👈 show
            MonsterModel.transform.position = attackPoint.position;
            MonsterModel.transform.rotation = attackPoint.rotation;
        }

        while (!windowForceStop) {
            targetWindow.MonsterOpenWindow(windowOpenSpeed);
            yield return null;
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    IEnumerator BackAttack() {
        Debug.Log("Monster preparing back attack!");

        // Play the bang sound
        if (AudioManager.Instance != null) {
            Debug.Log("Bang 1");
            AudioManager.Instance.PlayMonsterAudio("monster arrive hint1", attackPointBack.position);

            // Wait until the audio is no longer playing
            yield return new WaitWhile(() => AudioManager.Instance.IsSourcePlaying("monster arrive hint1"));

            yield return new WaitForSeconds(bangInterval);

            Debug.Log("Bang 2");
            AudioManager.Instance.PlayMonsterAudio("monster arrive hint1", attackPointBack.position);

            // Wait until the audio is no longer playing
            yield return new WaitWhile(() => AudioManager.Instance.IsSourcePlaying("monster arrive hint1"));

            yield return new WaitForSeconds(bangInterval);

            Debug.Log("Bang 3");
            AudioManager.Instance.PlayMonsterAudio("monster arrive hint2", attackPointBack.position);

            // Wait until the audio is no longer playing
            yield return new WaitWhile(() => AudioManager.Instance.IsSourcePlaying("monster arrive hint2"));
                                   
        }

        if (attackPointBack != null) {
            MonsterModel.transform.position = attackPointBack.position;
            MonsterModel.transform.rotation = attackPointBack.rotation;
        }

        Debug.Log("Monster appears at the back!");

        if (!player.isCrouching) {            
            if (monsterAnimator != null && !timerUI.timerFinished) {
                // base position = attack point
                Vector3 spawnPos = attackPointBack.position;

                // add forward offset (tweak this value!)
                float forwardOffset = 1f;
                spawnPos += attackPointBack.forward * forwardOffset;

                // apply position & rotation
                MonsterModel.transform.position = spawnPos;
                MonsterModel.transform.rotation = attackPointBack.rotation;
                MonsterModel.SetActive(true); // 👈 show

                //light active
                if (redlight != null) {
                    redlight.enabled = true;
                }

                monsterAnimator.SetTrigger("JSB");
                isJumpscaring = true;

                // force player look
                player.ForceLookPreset(3);

                // hard lock controls during jumpscare
                player.LockForJumpscare(true);

                AudioManager.Instance.PlayMonsterAudio("monster sound 3", attackPointBack.position);
                // Wait until the audio is no longer playing
                yield return new WaitWhile(() => AudioManager.Instance.IsSourcePlaying("monster sound 3"));
                AudioManager.Instance.PlayMonsterAudio("monster-roar-1", attackPointBack.position);
                // Wait until the audio is no longer playing
                yield return new WaitWhile(() => AudioManager.Instance.IsSourcePlaying("monster-roar-1"));

                Debug.Log("Game Over - Player not crouching!");
                Debug.Log("Player failed! Loading Fail Scene...");
                StartCoroutine(GameOver());
            }

        } else {
            if (monsterAnimator != null) {
                MonsterModel.SetActive(true); // 👈 show

                //light active
                if (redlight != null) {
                    redlight.enabled = true;
                }

                player.LockForJumpscare(false);

                monsterAnimator.SetTrigger("TriggerBehind");
                AudioManager.Instance.PlayMonsterAudio("monster sound 3", attackPointBack.position);
                // Wait until the audio is no longer playing
                yield return new WaitWhile(() => AudioManager.Instance.IsSourcePlaying("monster sound 3"));

                
                if (redlight != null) {
                    redlight.enabled = false;
                }

                StartCoroutine(HideAfterAnimation());
            }

            Debug.Log("Player dodged back attack!");
            yield return new WaitForSeconds(3f);
        }
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    IEnumerator HideAfterAnimation() {
        // Wait until current animation finishes
        float animLength = monsterAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);

        MonsterModel.transform.position = resetPoint.position;
        
        monsterAnimator.SetTrigger("next");
        // Give Animator one frame to process the transition
        yield return null;
        MonsterModel.SetActive(false); // hide again

        Debug.Log("reset to position and hidden");
    }

    IEnumerator GameOver() {
        // Wait until current animation finishes
        float animLength = monsterAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);

        SceneController.Instance.LoadFailScene();
    }
}
