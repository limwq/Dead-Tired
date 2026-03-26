using UnityEngine;
using System.Collections;

public class SpeedBoost : MonoBehaviour
{
    public float boostAmount = 5f;   // how much to increase speed
    public float boostDuration = 3f; // how long the boost lasts
    private PlayerMovement playerMovement;
    private bool isBoosting = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isBoosting)
        {
            playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                StartCoroutine(BoostPlayerSpeed());
            }
        }
    }

    IEnumerator BoostPlayerSpeed()
    {
        isBoosting = true;
        float originalSpeed = playerMovement.speed;

        // increase speed
        playerMovement.speed += boostAmount;

        // wait for duration
        yield return new WaitForSeconds(boostDuration);

        // reset speed
        playerMovement.speed = originalSpeed;
        isBoosting = false;
    }
}
