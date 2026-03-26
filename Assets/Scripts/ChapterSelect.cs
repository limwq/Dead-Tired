using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ChapterSelect : MonoBehaviour
{
    public Button chapter1Button;
    public Button chapter2Button;
    public Button chapter3Button;
    public Button backButton; // 返回主選單

    private bool isClikcked = false;

    void Start()
    {
        // 綁定按鈕事件
        chapter1Button.onClick.AddListener(() => StartCoroutine(PlaySoundAndLoad(SceneController.Instance.Chap1Trans)));
        chapter2Button.onClick.AddListener(() => StartCoroutine(PlaySoundAndLoad(SceneController.Instance.Chap2Trans)));
        chapter3Button.onClick.AddListener(() => StartCoroutine(PlaySoundAndLoad(SceneController.Instance.Chap3Trans)));
        backButton.onClick.AddListener(() => StartCoroutine(PlaySoundAndLoad(SceneController.Instance.LoadMainMenu)));
    }

    IEnumerator PlaySoundAndLoad(System.Action action) {
        // Play the button sound
        if (AudioManager.Instance != null && !isClikcked) {
            isClikcked = true;

            AudioManager.Instance.PlayUI("button");

            // Wait until the audio is no longer playing
            yield return new WaitWhile(() => AudioManager.Instance.IsPlaying());

            AudioManager.Instance.PlayTransition("creepy-whispering-6690");

        }

        // Invoke the action after sound finishes
        action.Invoke();
    }
}
