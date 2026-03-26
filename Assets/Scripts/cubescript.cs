using UnityEngine;

public class CubeFollowerShift : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;            // Player to follow
    public Vector3 offset = Vector3.up; // Default offset from player

    [Header("Shift Settings")]
    public LookBehindChecker lookBehindChecker; // Reference to checker
    public float upAmount = 2f;                 // Extra Y shift when looking behind
    public float moveSpeed = 3f;                // Smooth follow speed

    private Vector3 targetPos;

    void Update()
    {
        if (player == null) return;

        // Base position = player's position + offset
        targetPos = player.position + offset;

        // If looking behind, add extra upward movement
        if (lookBehindChecker != null && lookBehindChecker.IsLookingBehind())
        {
            targetPos += Vector3.up * upAmount;
        }

        // Smoothly move cube towards target
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
    }
}
