using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;
    private string lastSceneName;

    void Awake()
    {
        // 如果已經有一個 GameHandler，刪掉新的，確保只保留唯一一個
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 初始化單例
        Instance = this;
        DontDestroyOnLoad(gameObject); // 切換場景不銷毀
    }

    // 設定上一個場景
    public void SetLastScene(string sceneName)
    {
        lastSceneName = sceneName;
    }

    // 獲取上一個場景名稱
    public string GetLastScene()
    {
        return lastSceneName;
    }

    // 載入失敗場景
    public void LoadFailScene()
    {
        SetLastScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("FailScene");
    }
}
