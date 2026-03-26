using TMPro;
using UnityEngine;

public class LookBehindChecker : MonoBehaviour
{
    public Transform cameraTransform;
    public float lookBehindAngle = 180f;
    public float rotateSpeed = 5f;

    public bool isLookingBehind = false;
	
    private Quaternion forwardRotation;
    private Quaternion behindRotation;

    public TextMeshProUGUI usesText;

    public Chap2Win haswin;
    public Monster2AI monster;


    void Start()
    {
        forwardRotation = cameraTransform.localRotation;
        behindRotation = forwardRotation * Quaternion.Euler(0f, lookBehindAngle, 0f);

        if (usesText != null)
            usesText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !haswin.hasWon && !monster.isJumpScare)
        {
            isLookingBehind = !isLookingBehind;
        }

        Quaternion target = isLookingBehind ? behindRotation : forwardRotation;

        cameraTransform.localRotation = Quaternion.Lerp(
            cameraTransform.localRotation,
            target,
            Time.deltaTime * rotateSpeed
        );

        if (usesText != null) {
            usesText.gameObject.SetActive(isLookingBehind);
        }
    }

    // ✅ Public getter so other scripts can ask about the state
    public bool IsLookingBehind()
    {
        return isLookingBehind;
    }

    // ✅ Public method so other scripts can force behind view
    public void ForceBehindView()
    {
        isLookingBehind = true;
    }

}
