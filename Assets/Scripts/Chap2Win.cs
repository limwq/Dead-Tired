using System.Threading;
using UnityEngine;
using System.Collections;

public class Chap2Win : MonoBehaviour
{
    public Monster2AI monster;
    public ForwardMovement walking;
    public MonsterFreezeOnLook freeze;
    public bool hasWon { get; private set; } = false;


    void OnTriggerEnter(Collider other) {
        // Win condition
        if (!monster.isJumpScare && !hasWon) {
            hasWon = true;
            StartCoroutine(Win());
        }
    }

    IEnumerator Win() {
        Debug.Log("[Chap2Win] Player wins! Walking complete.");



        freeze.FreezeMonster();

        monster.StopWalkingSound();

        if (AudioManager.Instance != null) {
            AudioManager.Instance.StopMonsterAudio();
            AudioManager.Instance.StopLoopingSFX();
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlayTransition("Ding");
        }
        yield return new WaitForSeconds(3f);
        if (SceneController.Instance != null)
            SceneController.Instance.LoadNextScene();
    }
}
