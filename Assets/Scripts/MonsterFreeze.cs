using UnityEngine;

public class MonsterFreezeOnLook : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;
    public Transform monster;
    public MonoBehaviour monsterAI;
    public Animator monsterAnimator;

    [Header("Settings")]
    public float viewAngle = 60f;
    public LightTrigger[] lightTriggers; // Drag all light triggers here

    //[Header("Speed Boost Settings")]
    //public float boostedSpeed = 6f; 
    //public float boostDuration = 3f; 
    //public float originalSpeed;
    //private bool isBoosted = false;
    //private float boostTimer = 0f;

    private bool isFrozen = false;

    // ✅ New flag so we can stop speed boost when crawling starts
    //[HideInInspector] public bool ignoreSpeedBoost = true;

    void Start()
    {
        //if (monsterAI is Monster2AI ai)
        //{
            //originalSpeed = ai.moveSpeed;
        //}
    }

    void Update()
    {
        if (playerCamera == null || monster == null) return;

        bool playerUnderActiveLight = IsPlayerUnderAnyActiveLight();
        bool monsterInView = IsMonsterInView();

        bool shouldFreeze = playerUnderActiveLight && monsterInView;

        if (shouldFreeze) FreezeMonster();
        else UnfreezeMonster();

        //if (!ignoreSpeedBoost) 
           // HandleMonsterBoost();
    }

    bool IsPlayerUnderAnyActiveLight()
    {
        if (lightTriggers == null || lightTriggers.Length == 0) return false;

        foreach (var trigger in lightTriggers)
        {
            if (trigger != null && trigger.IsPlayerInsideActive() && trigger.spotLight.enabled)
                return true;
        }
        return false;
    }

    bool IsMonsterUnderAnyActiveLight()
    {
        if (lightTriggers == null || lightTriggers.Length == 0) return false;

        foreach (var trigger in lightTriggers)
        {
            if (trigger != null && trigger.IsMonsterInsideActive() && trigger.spotLight.enabled)
                return true;
        }
        return false;
    }

    bool IsMonsterInView()
    {
        Vector3 toMonster = (monster.position - playerCamera.position).normalized;
        float angle = Vector3.Angle(playerCamera.forward, toMonster);
        return angle <= viewAngle;
    }

    public void FreezeMonster()
    {
        if (isFrozen) return;

        isFrozen = true;
        if (monsterAI != null) { 
            monsterAI.enabled = false;
            Debug.Log("Freezed");

            // 🔊 Stop monster audio if it has walking sound
            if (monsterAI is Monster2AI ai) {
                ai.StopWalkingSound();
            }
        }
        if (monsterAnimator != null) monsterAnimator.enabled = false;
    }

    void UnfreezeMonster()
    {
        if (!isFrozen) return;

        isFrozen = false;
        if (monsterAI != null) monsterAI.enabled = true;
        if (monsterAnimator != null) monsterAnimator.enabled = true;
    }

    //void HandleMonsterBoost()
    //{
        //if (monsterAI is Monster2AI ai)
        //{
            //if (IsMonsterUnderAnyActiveLight())
            //{
                //boostTimer = boostDuration;
                //if (!isBoosted)
                //{
                    //ai.moveSpeed = boostedSpeed;
                    //isBoosted = true;
                //}
            //}

            //if (isBoosted)
            //{
                //boostTimer -= Time.deltaTime;
                //if (boostTimer <= 0f)
                //{
                    //ai.moveSpeed = originalSpeed;
                    //isBoosted = false;
                //}
            //}
        //}
    //}
}
