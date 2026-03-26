using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class NextSceneManager : MonoBehaviour {
    public Button nextButton;
    public Button restartButton;
    public Button mainMenuButton;
    private string nextScene;

    void Start() {
        // 🔹 Find the next scene based on last scene
        nextScene = SceneController.Instance.GetNextScene(SceneController.Instance.lastSceneName);

        nextButton.onClick.AddListener(() => StartCoroutine(PlaySoundAndLoadScene(() => SceneController.Instance.LoadSceneByName(nextScene))));
        restartButton.onClick.AddListener(() => StartCoroutine(PlaySoundAndLoadScene(SceneController.Instance.RestartLastScene)));
        mainMenuButton.onClick.AddListener(() => StartCoroutine(PlaySoundAndLoadScene(SceneController.Instance.LoadMainMenu)));
    }

    IEnumerator PlaySoundAndLoadScene(System.Action action) {
        // Play the button sound
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlayUI("exhale");

            // Wait until the audio is no longer playing
            yield return new WaitWhile(() => AudioManager.Instance.IsPlaying());
        }

        // Invoke the action after sound finishes
        action.Invoke();
    }
}

