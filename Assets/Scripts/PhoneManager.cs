using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Threading;

public class PhoneManager : MonoBehaviour {
    [Header("UI References")]
    public GameObject phoneUI;          // Main phone UI panel (chat screen)
    public Transform messageContainer;  // Parent object for messages
    public GameObject bossMessagePrefab;
    public GameObject playerReplyPrefab;
    public GameObject callScreen;       // Boss call overlay
    public Button replyButton;

    [Header("Timers")]
    public float messageInterval = 15f; 
    public float replyTime = 10f;       // Must reply in 10s
    public float callDuration = 10f;    // Locked for 10s

    private bool waitingForReply = false;
    private PlayerController1 player;
    private MonsterController monster;
    private TimerUI timerUI;

    private GameObject currentBossMessage; // track last boss bubble
    private Coroutine messageLoopCoroutine; // keep reference

    void Start() {
        if (replyButton != null) {
            replyButton.onClick.AddListener(HandleReply);
            replyButton.gameObject.SetActive(false); // hidden at start
        }

        if (callScreen != null)
            callScreen.SetActive(false);

        player = FindFirstObjectByType<PlayerController1>();
        monster = FindFirstObjectByType<MonsterController>();
        timerUI = FindFirstObjectByType<TimerUI>();


        // clear any old messages
        foreach (Transform child in messageContainer)
            Destroy(child.gameObject);

        StartMessageLoop();
    }

    private void Update() {
        if (timerUI != null && timerUI.alert3MinShown) {
            messageInterval = 12f;
            replyTime = 10f;
        }
        
        if (timerUI != null && timerUI.alert2MinShown) {
            messageInterval = 10f;
            replyTime = 8f;
        }

        if (timerUI != null && timerUI.alert1MinShown) {
            messageInterval = 8f;
            replyTime = 6f;
        }
    }

    // === Message Loop Control ===
    void StartMessageLoop() {
        if (messageLoopCoroutine != null) StopCoroutine(messageLoopCoroutine);
        messageLoopCoroutine = StartCoroutine(MessageLoop());
    }

    void StopMessageLoop() {
        if (messageLoopCoroutine != null) {
            StopCoroutine(messageLoopCoroutine);
            messageLoopCoroutine = null;
        }
    }

    IEnumerator MessageLoop() {
        while (true) {
            
            Debug.Log($"[PhoneManager] Waiting {messageInterval} seconds before next message...");
            yield return new WaitForSeconds(messageInterval);

            ReceiveMessage();
            StartReplyTimer();

            // ⏸ Wait until player on it
            yield return new WaitUntil(() => waitingForReply == false);

            Debug.Log("[PhoneManager] Replied, restarting messageInterval...");

            
        }
    }

    void ReceiveMessage() {
        Debug.Log("[PhoneManager] Boss sent a message!");
        AudioManager.Instance.PlaySFX("phone notification");

        // spawn new boss message prefab
        currentBossMessage = Instantiate(bossMessagePrefab, messageContainer);

        waitingForReply = true;

        if (replyButton != null)
            replyButton.gameObject.SetActive(true);

        ScrollToBottom();
    }

    void StartReplyTimer() {
        Debug.Log($"[PhoneManager] Starting reply timer ({replyTime}s)");
        StartCoroutine(ReplyCountdown());
    }

    IEnumerator ReplyCountdown() {
        float timer = replyTime;
        while (timer > 0 && waitingForReply) {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (waitingForReply) {
            Debug.Log("[PhoneManager] Reply missed → Boss calling!");
            StartCoroutine(BossCall());
        }
    }

    void HandleReply() {
        if (!waitingForReply) return;

        waitingForReply = false;
        Debug.Log("[PhoneManager] Player replied to boss.");
        AudioManager.Instance.PlaySFX("button");

        // move boss prefab upward slightly to simulate chat scroll
        if (currentBossMessage != null) {
            RectTransform rt = currentBossMessage.GetComponent<RectTransform>();
            rt.anchoredPosition += new Vector2(0, 100); // adjust offset
        }

        // spawn player reply prefab
        Instantiate(playerReplyPrefab, messageContainer);

        if (replyButton != null)
            replyButton.gameObject.SetActive(false);

        ScrollToBottom();
    }

    IEnumerator BossCall() {
        Debug.Log("Start Boss Call");
        waitingForReply = false;
        Debug.Log("Stopped MessageLoop");
        // stop message loop while in call
        StopMessageLoop();

        if (replyButton != null)
            replyButton.gameObject.SetActive(false);

        if (phoneUI != null)
            phoneUI.SetActive(false); // hide chat

        if (callScreen != null)
            callScreen.SetActive(true); // show call

        // 🚨 Lock phone so player can't close it
        player.phoneLockedByBoss = true;

        Debug.Log("enter phone");
        if (player != null) {
            yield return player.EnterPhoneRoutine(true);  // directly wait for it
        }

        Debug.Log("[PhoneManager] Boss call started!");
        AudioManager.Instance.PlayLoopingSFX("phone scolding ver 2");
        
        float timer = callDuration;
        while (timer > 0f) {
            // 🛑 End call if monster jumpscares
            if (monster != null && monster.isJumpscaring) {
                Debug.Log("[PhoneManager] Boss call interrupted by monster jumpscare!");
                break;
            }

            timer -= Time.deltaTime;
            yield return null;
        }
        
        Debug.Log("[PhoneManager] Boss call ended.");
        AudioManager.Instance.StopLoopingSFX();

        if (callScreen != null)
            callScreen.SetActive(false);

        if (phoneUI != null)
            phoneUI.SetActive(true); // restore chat

        if (player != null) {
            player.ExitPhone();
            player.phoneLockedByBoss = false;
            player.LockControls(false);
        }

        // restart loop AFTER call
        StartMessageLoop();
    }

    void ScrollToBottom() {
        Canvas.ForceUpdateCanvases();
        var scroll = messageContainer.GetComponentInParent<ScrollRect>();
        if (scroll != null) {
            scroll.verticalNormalizedPosition = 0f; // 0 = bottom
            Canvas.ForceUpdateCanvases();
        }
    }
}
