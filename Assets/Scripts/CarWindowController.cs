using UnityEngine;
using System;

public class CarWindowController : MonoBehaviour {
    [Header("Knob Settings")]
    public Transform knobPivot;
    public float knobRotateSpeed = 60f;

    [Header("Window Settings")]
    public Transform windowGlass;
    public float glassMoveSpeed = 8f;

    [Header("Window Detection Colliders")]
    public Collider closedCollider; // Enabled only when rolling up
    public Collider openCollider;   // Always enabled (losing trigger)

    public bool IsWindowClosed { get; private set; } = true;

    // Events for monster AI
    public event Action<CarWindowController> OnWindowClosed;
    public event Action<CarWindowController> OnWindowOpened;

    private bool isMovingUp = false;
    public bool monsterOpening = false;
    private float monsterOpenSpeed;

    [Header("Light Settings")]
    public Light redlight;

    private TimerUI timerUI;

    private void Awake() {
        if (closedCollider != null) {
            closedCollider.enabled = false;
            Debug.Log($"[{name}] Stop collider initially DISABLED");
        }

        if(redlight != null) {
            redlight.enabled = false;
        }

        timerUI = FindFirstObjectByType<TimerUI>();
    }

    void Update() {
        if (isMovingUp && !IsWindowClosed) {

            if (!AudioManager.Instance.IsPlaying()) {
                AudioManager.Instance.PlaySFX("for window handle");
            }

        }

        if (monsterOpening) {
            windowGlass.Translate(Vector3.down * monsterOpenSpeed * Time.deltaTime, Space.World);

            if (!AudioManager.Instance.IsPlaying()) {
                AudioManager.Instance.PlaySFX("for window handle");
            }

        }

        if (timerUI != null && timerUI.timer <= 100f) {
            glassMoveSpeed = 5f;
        }
    }

    public void RollWindowUp() {
        if (!IsWindowClosed) {
            isMovingUp = true;
            windowGlass.Translate(Vector3.up * glassMoveSpeed * Time.deltaTime, Space.World);
            knobPivot.Rotate(Vector3.forward, knobRotateSpeed * Time.deltaTime);
        }
        if (monsterOpening && closedCollider != null && !closedCollider.enabled) {
            closedCollider.enabled = true;
            Debug.Log($"[{name}] Stop collider ENABLED");
        }

    }

    public void StopRollingUp() {
        isMovingUp = false;
    }

    public void MonsterOpenWindow(float openSpeed) {
        if (IsWindowClosed) {
            IsWindowClosed = false;
        }
        monsterOpening = true;
        monsterOpenSpeed = openSpeed;
        redlight.enabled = true;
    }

    public void StopMonsterOpen() {
        monsterOpening = false;
        redlight.enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        // Case 1: Closed collider hit
        if (closedCollider != null && other == closedCollider) {
            IsWindowClosed = true;
            Debug.Log($"[{name}] Window STOP detected — Monster should retreat");
            isMovingUp = false;
            closedCollider.enabled = false;

            Debug.Log($"{name} window closed!");
            OnWindowClosed?.Invoke(this); // Notify monster
        }

        // Case 2: Open collider hit
        else if (openCollider != null && other == openCollider) {
            Debug.Log($"[{name}] Window fully OPEN — Monster can attack!");
            OnWindowOpened?.Invoke(this); // Notify monster
        }
    }
}
