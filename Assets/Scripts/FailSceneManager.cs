using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Threading;

public class FailSceneManager : MonoBehaviour {
    public Button tryAgainButton;
    public Button mainMenuButton;

    public bool isPressed = false;

    void Start() {
        tryAgainButton.onClick.AddListener(() => StartCoroutine(PlaySoundAndLoadScene(SceneController.Instance.RestartLastScene)));
        mainMenuButton.onClick.AddListener(() => StartCoroutine(PlaySoundAndLoadScene(SceneController.Instance.LoadMainMenu)));
    }

    IEnumerator PlaySoundAndLoadScene(System.Action action) {
        // Play the button sound
        if (AudioManager.Instance != null && !isPressed) {
            isPressed = true;
            AudioManager.Instance.PlayUI("exhale");

            // Wait until the audio is no longer playing
            yield return new WaitWhile(() => AudioManager.Instance.IsPlaying());
            
        }

        // Invoke the action after sound finishes
        action.Invoke();
    }
}

