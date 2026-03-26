using UnityEngine;
using System.Collections;

public class Monster2AI : MonoBehaviour {
    [Header("Player References")]
    public Transform player;
    public LookBehindChecker lookBehindChecker;

    [Header("Movement Settings")]
    public float followDistance = 10f;
    public float attackDistance = 2f;
    public float moveSpeed;

    [Header("Attack Settings")]
    public float attackCooldown = 3f;
    public bool forceLookBehindOnAttack = true;
    public bool revertLookWhenFar = true;

    [Header("Animator")]
    public Animator animator;

    [Header("Audio")]
    public AudioSource monsterAudio;   
    public AudioClip walkClip;
    public AudioClip crawlClip;

    public MonsterStartCrawlOnTrigger MonsterCrawl;

    public Chap2Win haswin;

    private float lastAttackTime = -999f;
    private bool hasForcedLookBehind = false;
    public bool isJumpScare = false;
    private bool isWalkingSoundPlaying = false;

    void Start() {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (monsterAudio == null)
            monsterAudio = GetComponent<AudioSource>();
    }

    void Update() {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= followDistance) {
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0f;
            transform.rotation = Quaternion.LookRotation(dir);

            if (distance > attackDistance) {
                // Monster is following the player (WALKING)
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
                animator.SetBool("isFollowing", true);

                StartWalkingSound();

                if (revertLookWhenFar && hasForcedLookBehind && lookBehindChecker != null) {
                    lookBehindChecker.isLookingBehind = false;
                    hasForcedLookBehind = false;
                }
            } else {
                // Monster is close enough to attack
                animator.SetBool("isFollowing", false);
                StopWalkingSound();

                if (!isJumpScare && !haswin.hasWon) {
                    isJumpScare = true;
                    TryAttack();
                }
            }
        } else {
            // Monster is too far, not moving
            animator.SetBool("isFollowing", false);
            StopWalkingSound();

            if (revertLookWhenFar && hasForcedLookBehind && lookBehindChecker != null) {
                lookBehindChecker.isLookingBehind = false;
                hasForcedLookBehind = false;
            }
        }
    }

    void TryAttack() {
        if (Time.time - lastAttackTime >= attackCooldown) {
            lastAttackTime = Time.time;

            // ✅ Play monster attack audio right away (still from AudioManager)
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMonsterAudio("monster attack sound maybe", transform.position);

            // ✅ Force player look behind only once per attack
            if (forceLookBehindOnAttack && lookBehindChecker != null) {
                lookBehindChecker.ForceBehindView();
                hasForcedLookBehind = true;
            }

            // ✅ Trigger correct animation
            if (lookBehindChecker != null && lookBehindChecker.IsLookingBehind())
                animator.SetTrigger("AttackBack");
            else
                animator.SetTrigger("AttackFront");

            Debug.Log("Player failed! Loading Fail Scene...");
            StartCoroutine(GameOver());
        }
    }

    IEnumerator GameOver() {
        // Wait until current animation finishes
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);

        SceneController.Instance.LoadFailScene();
    }

    public void StartWalkingSound() {
        if (monsterAudio == null) return;

        // 🔹 Decide which clip to use (walk or crawl)
        AudioClip targetClip = (MonsterCrawl != null && !MonsterCrawl.hasTriggered && crawlClip != null)
            ? walkClip
            : crawlClip;

        if (targetClip == null) return;

        // Only change if not already playing this clip
        if (!haswin.hasWon) {
            if (!isWalkingSoundPlaying || monsterAudio.clip != targetClip) {
                monsterAudio.clip = targetClip;
                monsterAudio.loop = true;
                monsterAudio.Play();
                isWalkingSoundPlaying = true;

                Debug.Log($"[Monster2AI] Playing {(targetClip == crawlClip ? "Crawl" : "Walk")} sound.");
            }
        }
    }

    public void StopWalkingSound() {
        if (monsterAudio == null) return;

        if (isWalkingSoundPlaying) {
            monsterAudio.Stop();
            isWalkingSoundPlaying = false;
            Debug.Log("Audio Stopped");
        }else if (haswin.hasWon) {
            monsterAudio.Stop();
            Debug.Log("Player win. Audio Stopped");
        }
    }
}
