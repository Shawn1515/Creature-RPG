using UnityEngine;

public class CreatureEncounter : MonoBehaviour
{
    public CreatureData creatureSpecies;
    private CreatureInstance creature;
    private PlayerMovement playerMovement;
    void Start()
    {
        int level = Random.Range(2, 8);
        creature = new CreatureInstance(creatureSpecies, level);
        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            Vector3 direction = other.transform.position - transform.position;
            direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction);
            playerMovement?.SnapToGround();
            DialogueUI.Instance.SetPendingBattle(creature, transform);
            DialogueUI.Instance.StartDialogue(creatureSpecies.encounterText);
        }
    }
}