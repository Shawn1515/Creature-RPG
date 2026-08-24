using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [TextArea]
    public string[] dialogueLines;
    public string NPCName;
    public void Interact() {
        FacePlayer();
        DialogueUI.Instance.StartDialogue(dialogueLines, NPCName);
    }

    void FacePlayer() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            return;
        }

        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}