using UnityEngine;
using TMPro;

public class TipsManager : MonoBehaviour {
    [Header("UI")]
    public TextMeshProUGUI tipText;

    private void Start() {
        if (tipText == null) {
            Debug.LogWarning("[TipsManager] No TextMeshProUGUI assigned!");
            return;
        }

        string lastScene = SceneController.Instance != null ? SceneController.Instance.lastSceneName : "";
        tipText.text = GetRandomTip(lastScene);
    }

    private string GetRandomTip(string sceneName) {
        string[] tipsPool;

        switch (sceneName) {
            case "Chap1Car":
                tipsPool = new string[] {
                    "Not all sounds come from the same place.",
                    "The phone can be a distraction… or worse." ,
                    "When the boss calls, you’re stuck.",
                    "Sometimes staying low saves you."
                };
                break;

            case "Chap2Street":
                tipsPool = new string[] {
                    "Light is your safest shelter.",
                    "A flash of light can push it back.",
                    "Don’t waste your light… it needs time.",
                    "Staring at it might hold it still."
                };
                break;

            case "Chap3Bath":
                tipsPool = new string[] {
                   "Shutting out the dark can be dangerous.",
                   "Blurry eyes make clumsy hands.",
                   "Bright rooms slow its hunger.",
                   "Water soothes more than you think."
                };
                break;

            default:
                tipsPool = new string[] {
                    "Stay calm and think before you act.",
                    "Watch your environment for clues.",
                    "Quick reactions can make the difference."
                };
                break;
        }

        int randIndex = Random.Range(0, tipsPool.Length);
        return tipsPool[randIndex];
    }
}
