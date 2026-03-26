using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioClip footstepClip; // Assign a single footstep loop sound in Inspector
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // Loop the sound while walking
        audioSource.playOnAwake = false; // Don't play at start
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = footstepClip;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
