using UnityEngine;

public class CubeFollower : MonoBehaviour
{
    public Transform player;       // Assign player in Inspector
    public Vector3 offset; // Position offset (1 unit above player)

    void Update()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }
}
