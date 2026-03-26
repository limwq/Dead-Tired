using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.GlobalIllumination;

public class BathroomLight : MonoBehaviour {
    public Light bathroomLight;
    public Light switchLight;

    public float minOnTime = 15f;   // minimum seconds light stays on
    public float maxOnTime = 20f;   // maximum seconds light stays on
    public bool isTurnedOn = false;

    
    void Start() {         
        OffLight(true);
        StartCoroutine(BathroomLightRoutine());
        
    }

    IEnumerator BathroomLightRoutine() {
        while (true) {
            Debug.Log("BathroomLight");
            float onTime = UnityEngine.Random.Range(minOnTime, maxOnTime);
            Debug.Log($"Waiting for[{onTime}]");
            yield return new WaitForSeconds(onTime);
            OffLight(false);

            // ⏸ Wait until player on it
            yield return new WaitUntil(() => isTurnedOn == true);

            Debug.Log("[BathroomLight] Light turned on, restarting onTime...");
        }
    }

    public void OffLight(bool state) {
        if (bathroomLight == null) return;

        // Turn OFF the light
        bathroomLight.enabled = state;
        switchLight.enabled = !state;
        isTurnedOn = state;

        if (AudioManager.Instance != null && !AudioManager.Instance.IsPlaying()) {
            AudioManager.Instance.PlaySFX("LightSwitch");
        }

        Debug.Log(state ? "Light On" : "Light Off");
    }
}
