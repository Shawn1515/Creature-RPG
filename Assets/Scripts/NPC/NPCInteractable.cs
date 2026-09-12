using UnityEngine;
using UnityEngine.Events;
public class NPCInteractable : MonoBehaviour, IInteractable
{
    [TextArea]
    public string[] dialogueLines;
    public string NPCName;

    [TextArea]
    public string[] repeatDialogue;

    public UnityEvent action;

    private bool didAction = false;
    public void Interact() {
        FacePlayer();
        if(!didAction && action.GetPersistentEventCount() > 0)
        {
            didAction = true;
            DialogueUI.Instance.SetOnFinished(() => action?.Invoke());
            DialogueUI.Instance.StartDialogue(dialogueLines, NPCName);
        }
        else
        {
            if(repeatDialogue.Length != 0)
            {
                DialogueUI.Instance.StartDialogue(repeatDialogue, NPCName);
            }
            else
            {
                DialogueUI.Instance.StartDialogue(dialogueLines, NPCName);
            }
        }
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