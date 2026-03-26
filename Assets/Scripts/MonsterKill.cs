using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MonsterKill : MonoBehaviour
{
    public float delayBeforeFail = 1.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(LoadFailSceneAfterDelay());
        }
    }

    private IEnumerator LoadFailSceneAfterDelay()
    {
        // Optional: trigger monster attack animation here
        yield return new WaitForSeconds(delayBeforeFail);
        SceneManager.LoadScene("FailScene");
    }
}
