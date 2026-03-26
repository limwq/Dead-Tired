using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SoreProgressManager : MonoBehaviour {
    [Header("References")]
    public Image soreOverlay;           // Red overlay (UI Image full screen)
    public ShowerOnOff showerScript;    // drag in Inspector
    public BathroomLight lightScript;   // drag in Inspector
    public MonsterController3 monster;
    public ParticleSystem bubbleParticles; // Bubble effect particles

    [Header("Settings")]
    public float increaseRate = 2f;     // rate when shower off
    public float lightMultiplier = 2f;  // extra multiplier if light off
    public float decreaseRate = 1f;     // rate when shower on
    public float showerOffDelay = 5f;   // delay before sore starts

    private float soreProgress = 0f;    // 0–100
    private float showerOffTimer = 0f;

    void Update() {
        if (showerScript == null || lightScript == null) return;

        bool showerIsOn = showerScript.isTurnedOn;
        bool lightIsOn = lightScript.isTurnedOn;

        // === Shower OFF ===
        if (!showerIsOn) {
            
            
            showerOffTimer += Time.deltaTime;

            if (showerOffTimer >= showerOffDelay) {
                float rate = increaseRate;

                // Faster if light is also OFF
                if (!lightIsOn) {
                    rate *= lightMultiplier;
                }

                soreProgress += rate * Time.deltaTime;
            }
        } else {
            // Reset timer + heal when water is on
            showerOffTimer = 0f;
            soreProgress -= decreaseRate * Time.deltaTime;
            
        }

        // Clamp to safe range
        soreProgress = Mathf.Clamp(soreProgress, 0f, 100f);

        // === Apply alpha to overlay ===
        if (!monster.jumpscareTriggered && soreOverlay != null) {
            Color c = soreOverlay.color;
            c.a = soreProgress / 100f; // map progress to transparency
            soreOverlay.color = c;
        }

        // === Control bubble particles intensity ===
        if (bubbleParticles != null) {
            var emission = bubbleParticles.emission;
            // Square or cube to accelerate later
            float curve = Mathf.Pow(soreProgress / 100f, 2f);
            // emission rate proportional to sore progress
            emission.rateOverTime = Mathf.Lerp(0, 1000, curve);
        }
    }

    public float GetSoreProgress() {
        return soreProgress;
    }
}
