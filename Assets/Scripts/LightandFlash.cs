using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LightAndFlash : MonoBehaviour
{
    [Header("Screen Flash Settings")]
    public Image whiteScreen;
    public float flashDuration = 3f;
    public float fadeOutTime = 1f;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip flashSound;
    public AudioClip secondarySound;

    [Header("Cooldown Settings")]
    public float cooldownTime = 2f;
    private bool isOnCooldown = false;

    [Header("Look Behind Settings")]
    public LookBehindChecker lookBehindChecker;

    [Header("Monster Settings")]
    public Transform monster;
    public MonoBehaviour monsterAI;
    public Transform monsterSpawnPoint;
    private Animator monsterAnimator;

    [Header("Usage Limit")]
    public int maxUses = 5;
    private int remainingUses;

    [Header("Monster Stun Settings")]
    public float stunDuration = 3f; // How long monster stays disabled
    public float teleportDelay = 0.5f; // Delay before teleporting after freeze

    [Header("UI")]
    public TextMeshProUGUI usesText;

    void Start()
    {
        remainingUses = maxUses;

        if (monster != null)
            monsterAnimator = monster.GetComponent<Animator>();

        if (whiteScreen != null)
        {
            whiteScreen.color = new Color(1f, 1f, 1f, 0f);
            whiteScreen.enabled = false;
        }

        UpdateUsesUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isOnCooldown && remainingUses > 0)
        {
            if (lookBehindChecker != null && lookBehindChecker.IsLookingBehind())
            {
                StartCoroutine(FlashWithCooldown());
            }
        }
    }

    IEnumerator FlashWithCooldown()
    {
        isOnCooldown = true;
        remainingUses--;
        UpdateUsesUI();

        // 🎵 Play sounds
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("camera-flash-204151");
            AudioManager.Instance.PlayMonsterAudio("monster get flashed", monster.position);
        }

        yield return StartCoroutine(FlashAndFade());

        if (monster != null) {
            var ai = monsterAI as Monster2AI; // safe cast
            if (ai != null && !ai.isJumpScare)   // or ai.IsJumpscare if you use property
            {
                // ✅ Step 1: Freeze monster immediately
                ai.enabled = false;

                if (monsterAnimator != null)
                    monsterAnimator.enabled = false;

                Debug.Log("Monster frozen!");

                // ✅ Step 2: Wait for a short delay, then teleport
                yield return new WaitForSeconds(teleportDelay);

                monster.position = monsterSpawnPoint.position;
                Debug.Log("Monster teleported to spawn!");

                // ✅ Step 3: Wait for remaining stun duration before enabling again
                yield return new WaitForSeconds(stunDuration);

                if (monsterAnimator != null)
                    monsterAnimator.enabled = true;

                ai.enabled = true;

                Debug.Log("Monster re-enabled after stun.");
            }
        }

        yield return new WaitForSeconds(cooldownTime); 
        isOnCooldown = false;
    }

    IEnumerator FlashAndFade()
    {
        if (whiteScreen == null)
            yield break;

        whiteScreen.enabled = true;
        whiteScreen.color = Color.white;

        yield return new WaitForSeconds(flashDuration);

        float elapsed = 0f;
        Color startColor = whiteScreen.color;
        Color endColor = new Color(1f, 1f, 1f, 0f);

        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            whiteScreen.color = Color.Lerp(startColor, endColor, elapsed / fadeOutTime);
            yield return null;
        }

        whiteScreen.enabled = false;
    }

    void UpdateUsesUI() {
        if (usesText != null) {
            usesText.text = $"Remaining: {remainingUses}/{maxUses}";
        }
    }
}
