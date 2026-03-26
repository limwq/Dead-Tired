using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    public Button playButton;
    public Button optionsButton;
    public Button quitButton;

    void Start()
    {
        
        playButton.onClick.AddListener(() => StartCoroutine(PlaySoundAndLoadScene(SceneController.Instance.Chap1Trans)));
        optionsButton.onClick.AddListener(() => StartCoroutine(PlaySoundAndLoadScene(SceneController.Instance.ChapterChoose)));
        quitButton.onClick.AddListener(QuitGame);
    }

    IEnumerator PlaySoundAndLoadScene(System.Action action) {
        // Play the button sound
        if (AudioManager.Instance != null) {

            AudioManager.Instance.PlayUI("button");

            // Wait until the audio is no longer playing
            yield return new WaitWhile(() => AudioManager.Instance.IsPlaying());
        } 

        if (action == SceneController.Instance.Chap1Trans) {
            AudioManager.Instance.PlayTransition("creepy-whispering-6690");
        }

            // Invoke the action after sound finishes
            action.Invoke();
    }

    void QuitGame()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUI("button");

        Debug.Log("遊戲退出");
        Application.Quit();
    }
}
