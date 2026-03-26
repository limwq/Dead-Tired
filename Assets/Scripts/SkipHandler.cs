using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;


public class SkipHandler : MonoBehaviour {
    [Header("Skip Settings")]
    public string targetSceneName;  
    public float delayTime = 10f;
    public Button skipButton;

    [Header("Video Settings")]
    public VideoPlayer videoPlayer;   // Assign your VideoPlayer component in Inspector

    private bool hasSkipped = false;

    void Start() {
        if (skipButton != null)
            skipButton.onClick.AddListener(() => StartCoroutine(SkipToTargetScene()));

        if (videoPlayer != null) {
            // Subscribe to video finished event
            videoPlayer.loopPointReached += OnVideoFinished;
        }

        StartCoroutine(DelaySkip());
    }

    IEnumerator DelaySkip() {
        yield return new WaitForSeconds(delayTime);

        if (!hasSkipped)
            SkipToTargetScene();
    }

    private void OnVideoFinished(VideoPlayer vp) {
        if (!hasSkipped) {
            SceneController.Instance.LoadSceneByName(targetSceneName);
        }
    }

    IEnumerator SkipToTargetScene() {
        if (hasSkipped) yield break;

        hasSkipped = true;

        // Play the button sound
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlayUI("button");

            // Wait until the audio is no longer playing
            yield return new WaitWhile(() => AudioManager.Instance.IsPlaying());
        }

        if (SceneController.Instance != null)
            SceneController.Instance.LoadSceneByName(targetSceneName);
        else
            Debug.LogError("[SkipHandler] SceneController.Instance is null!");
    }
}
