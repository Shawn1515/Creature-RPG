using UnityEngine;

public class HealStation : MonoBehaviour, IInteractable
{
    public string[] healDialogue =
    {
        "Welcome to the Creature Center!",
        "Let me heal your creatures.",
        "...",
        "All healed up!",
        "Please come again!"
    };

    public void Interact()
    {
        FacePlayer();
        DialogueUI.Instance.StartDialogue(healDialogue, "Jim");
        HealPlayer();
    }

    public void HealPlayer()
    {
        PartyManager.Instance.HealParty();
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