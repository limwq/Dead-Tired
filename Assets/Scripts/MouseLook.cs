using UnityEngine;

public class SmoothToggleLookBehind : MonoBehaviour
{
    public Transform cameraTransform;   // Just the camera, not the whole player
    public float lookBehindAngle = 180f;
    public float rotateSpeed = 5f;

    private bool isLookingBehind = false;
    private Quaternion forwardRotation;
    private Quaternion behindRotation;

    void Start()
    {
        // Save initial forward-facing rotation
        forwardRotation = cameraTransform.localRotation;
        behindRotation = forwardRotation * Quaternion.Euler(0f, lookBehindAngle, 0f);
    }

    void Update()
    {
        // Toggle state on F press
        if (Input.GetKeyDown(KeyCode.F))
        {
            isLookingBehind = !isLookingBehind;
        }

        // Choose which direction to face
        Quaternion target = isLookingBehind ? behindRotation : forwardRotation;

        // Smoothly rotate the camera
        cameraTransform.localRotation = Quaternion.Lerp(
            cameraTransform.localRotation,
            target,
            Time.deltaTime * rotateSpeed
        );
    }
}
