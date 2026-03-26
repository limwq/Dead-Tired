using UnityEngine;
using System.Collections;

public class LightSequenceController : MonoBehaviour
{
    [Header("Setup")]
    public LightTrigger[] lightTriggers; // Drag all LightTrigger scripts in scene here
    public Transform[] triggerPoints;    // Drag 3 empty GameObjects here

    [Header("Sequence Settings")]
    public Vector2[] onTimes;  // 3 pairs of min/max on times
    public Vector2[] offTimes; // 3 pairs of min/max off times

    private int currentStep = 0;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Check which trigger point was hit
        for (int i = 0; i < triggerPoints.Length; i++)
        {
            if (other.transform == triggerPoints[i])
            {
                HandleTriggerStep(i);
                break;
            }
        }
    }

    void HandleTriggerStep(int step)
    {
        if (step < currentStep) return; // Prevent re-triggering
        currentStep = step + 1;

        if (currentStep < 3)
        {
            // Adjust flicker times for all lights
            foreach (var lt in lightTriggers)
            {
                lt.minOnTime = onTimes[currentStep - 1].x;
                lt.maxOnTime = onTimes[currentStep - 1].y;
                lt.minOffTime = offTimes[currentStep - 1].x;
                lt.maxOffTime = offTimes[currentStep - 1].y;
            }
        }
        else
        {
            // Final step: turn all lights off permanently
            foreach (var lt in lightTriggers)
            {
                lt.StopAllCoroutines();
                if (lt.spotLight != null)
                    lt.spotLight.enabled = false;
                lt.enabled = false;
            }
        }
    }
}
