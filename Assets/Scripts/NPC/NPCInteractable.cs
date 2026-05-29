using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [TextArea]
    public string[] dialogueLines;
    public void Interact()
    {
        DialogueUI.Instance.StartDialogue(dialogueLines);
    }
}