using UnityEngine;

public class ForwardMovement : MonoBehaviour
{
    public float speed = 5f;   // Forward speed
    private CharacterController controller;
	public LookBehindChecker lookBehindChecker;
    public Monster2AI monster;


    public Chap2Win haswin;


    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Default no movement
        Vector3 move = Vector3.zero;

        // Only move forward with W
        if (Input.GetKey(KeyCode.W) && !lookBehindChecker.IsLookingBehind() && !haswin.hasWon && !monster.isJumpScare) {

            move = transform.forward * speed;

            if (AudioManager.Instance != null) {
                if (speed <= 10) {
                    AudioManager.Instance.PlayLoopingSFX("character_walking");
                } else { 
                    AudioManager.Instance.PlayLoopingSFX("person-running-loop-245173");
                }
            }
        }else if(!haswin.hasWon){

            if (AudioManager.Instance != null) {
                AudioManager.Instance.StopLoopingSFX();
            }
        } else {
            move = transform.forward * 0;
            if (AudioManager.Instance != null) {
                AudioManager.Instance.StopLoopingSFX();
            }
        }

            // Apply movement
            controller.SimpleMove(move);
    }
}
