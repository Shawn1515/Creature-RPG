using UnityEngine;
using System.Collections;

public class CreatureEncounter : MonoBehaviour
{
    public CreatureData creatureSpecies;
    private CreatureInstance creature;
    private PlayerMovement playerMovement;

    [HideInInspector]
    public CreatureSpawnPoint spawnPoint;

    void Start()
    {
        int level = Random.Range(1, 4);
        creature = new CreatureInstance(creatureSpecies, level);
        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            playerMovement?.SnapToGround();
            DialogueUI.Instance.SetPendingBattle(creature, transform);
            DialogueUI.Instance.StartDialogue(creatureSpecies.encounterText, "");
            StartCoroutine(FacePlayer(other.transform));
        }
    }

    IEnumerator FacePlayer(Transform player)
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                6f * Time.deltaTime
            );

            yield return null;
        }
        transform.rotation = targetRotation;
    }
}