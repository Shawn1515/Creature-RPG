using UnityEngine;

public class CreatureEncounter : MonoBehaviour
{
    public CreatureData creatureData;
    private bool hasTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) {
            return;
        }

        if (other.CompareTag("Player")) {
            hasTriggered = true;
            Vector3 direction = other.transform.position - transform.position;
            direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction);
            DialogueUI.Instance.SetPendingBattle(creatureData, transform);
            DialogueUI.Instance.StartDialogue(creatureData.encounterText);
        }
    }
}