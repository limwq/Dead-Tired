using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIImageSwitcher : MonoBehaviour
{
    [Header("UI Images")]
    public Image firstImage;   // Default (looking forward)
    public Image secondImage;  // When looking back
    public Image thirdImage;   // Temporary (on E key)

    [Header("References")]
    public LookBehindChecker lookBehindChecker; // Drag your LookBehindChecker here

    [Header("Settings")]
    public float thirdImageDuration = 10f;

    private Coroutine thirdImageCoroutine;
    private bool isThirdImageActive = false;

    void Start()
    {
        ShowFirstImage();
    }

    void Update()
    {
        bool isLookingBack = lookBehindChecker != null && lookBehindChecker.IsLookingBehind();

        // E key toggles third image (only when looking back)
        if (isLookingBack && Input.GetKeyDown(KeyCode.E))
        {
            ShowThirdImageTemporarily();
        }

        // If third image is active, we don't stop it — we just decide what to show
        if (isThirdImageActive)
        {
            if (isLookingBack)
                ShowThirdImage();
            else
                ShowFirstImage(); // Show first image while looking forward, but timer keeps running
        }
        else
        {
            // Normal switching when third image not active
            if (isLookingBack)
                ShowSecondImage();
            else
                ShowFirstImage();
        }
    }

    void ShowFirstImage()
    {
        if (firstImage != null) firstImage.enabled = true;
        if (secondImage != null) secondImage.enabled = false;
        if (thirdImage != null) thirdImage.enabled = false;
    }

    void ShowSecondImage()
    {
        if (firstImage != null) firstImage.enabled = false;
        if (secondImage != null) secondImage.enabled = true;
        if (thirdImage != null) thirdImage.enabled = false;
    }

    void ShowThirdImage()
    {
        if (firstImage != null) firstImage.enabled = false;
        if (secondImage != null) secondImage.enabled = false;
        if (thirdImage != null) thirdImage.enabled = true;
    }

    void ShowThirdImageTemporarily()
    {
        if (thirdImageCoroutine != null)
            StopCoroutine(thirdImageCoroutine);

        isThirdImageActive = true;
        thirdImageCoroutine = StartCoroutine(ThirdImageTimer());
    }

    IEnumerator ThirdImageTimer()
    {
        float elapsed = 0f;

        while (elapsed < thirdImageDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        isThirdImageActive = false;
        thirdImageCoroutine = null;
    }
}
