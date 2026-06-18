using UnityEngine;

public class CreatureEncounter : MonoBehaviour
{
    public CreatureData creatureSpecies;
    private CreatureInstance creature;
    void Start()
    {
        creature = new CreatureInstance(creatureSpecies);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            Vector3 direction = other.transform.position - transform.position;
            direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction);
            DialogueUI.Instance.SetPendingBattle(creature, transform);
            DialogueUI.Instance.StartDialogue(creatureSpecies.encounterText);
        }
    }
}