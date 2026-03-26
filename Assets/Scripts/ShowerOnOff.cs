using UnityEngine;
using System.Collections;

public class ShowerOnOff : MonoBehaviour {
    [Header("Shower Settings")]
    public float minOnTime = 10f;   // minimum seconds water stays on
    public float maxOnTime = 15f;   // maximum seconds water stays on

    [Header("References")]
    public ParticleSystem showerWater;

    public bool isTurnedOn = true;

    private Coroutine showerRoutineRef;

    void Start() {
        if (showerWater != null) {
            // Start ON by default
            ToggleWater(true);

            // begin auto on/off cycle
            showerRoutineRef = StartCoroutine(ShowerRoutine());
        }
    }

    IEnumerator ShowerRoutine() {
        while (true) {
            float onTime = UnityEngine.Random.Range(minOnTime, maxOnTime);
            Debug.Log($"[Shower] Water running for {onTime:F1}s");
            yield return new WaitForSeconds(onTime);

            ToggleWater(false);
            Debug.Log("[Shower] Water turned OFF");

            // ⏸ Wait until player on it
            yield return new WaitUntil(() => isTurnedOn == true);

            Debug.Log("[ShowerOnOff] Water turned on, restarting onTime...");
        }
    }

    public void ToggleWater(bool state) {
        if (showerWater == null) return;

        var emission = showerWater.emission;
        emission.enabled = state;
        isTurnedOn = state;
        Debug.Log(state ? "Water On" : "Water Off");
        if (AudioManager.Instance != null && isTurnedOn == true) {
            AudioManager.Instance.PlayShowerLooping ("Shower", true);
        } else if(AudioManager.Instance != null && isTurnedOn == false) {
            AudioManager.Instance.PlayShowerLooping("Bubble", true); 
        }
        
    }

    public void ForceStop() {
        if (showerRoutineRef != null) StopCoroutine(showerRoutineRef);
        ToggleWater(false);
    }
}
