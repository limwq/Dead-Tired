using UnityEngine;

public class MonsterStartCrawlOnTrigger : MonoBehaviour
{
    [Header("Monster References")]
    public Animator monsterAnimator;  
    public MonoBehaviour monsterAI;   
    public MonsterFreezeOnLook freezeOnLook; // ✅ Drag your MonsterFreezeOnLook script here

    [Header("Animation Settings")]
    public string crawlTriggerName = "StartCrawl";

    [Header("Movement Settings")]
    public float crawlSpeed = 2f;
    public bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        if (monsterAnimator != null)
        {
            monsterAnimator.SetTrigger(crawlTriggerName);
        }

        if (monsterAI is Monster2AI ai)
        {
            ai.moveSpeed = crawlSpeed;
        }

        // ✅ Disable boost logic once crawling starts
        if (freezeOnLook != null)
            //freezeOnLook.ignoreSpeedBoost = true;

        hasTriggered = true;
    }
}
