using UnityEngine;

public class TurnOffAssignedLightsOnly : MonoBehaviour
{
    private bool hasTriggered = false;
    public Transform player;

    [Header("Lights to Turn Off")]
    public Light[] lightsToTurnOff; // ✅ Assign specific lights you want OFF

    [Header("Disable LightTriggers Too?")]
    public bool disableLightTriggers = true; // Toggle if you want to stop triggers

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        // ✅ Turn off only assigned lights
        foreach (Light light in lightsToTurnOff)
        {
            if (light != null)
                light.enabled = false;
        }

        if (disableLightTriggers)
        {
            // ✅ Stop all LightTrigger coroutines and disable them
            LightTrigger[] allTriggers = FindObjectsOfType<LightTrigger>();
            foreach (LightTrigger trigger in allTriggers)
            {
                trigger.StopAllCoroutines();
                trigger.enabled = false;
            }
        }

        hasTriggered = true;
    }
}
