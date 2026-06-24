using UnityEngine;

public class CreatureSpawnPoint : MonoBehaviour
{
    public GameObject[] possibleCreatures;

    public float respawnTime = 30f;

    private GameObject currentCreature;

    public void Start()
    {
        SpawnCreature();
    }

    public void SpawnCreature()
    {
        if(currentCreature != null)
        {
            return;
        }

        int index =
            Random.Range(
                0,
                possibleCreatures.Length
            );

        currentCreature =
            Instantiate(
                possibleCreatures[index],
                transform.position,
                Quaternion.identity
            );

        CreatureEncounter encounter =
            currentCreature.GetComponent<CreatureEncounter>();

        encounter.spawnPoint = this;
    }

    public void CreatureDefeated()
    {
        currentCreature = null;

        Invoke(
            nameof(SpawnCreature),
            respawnTime
        );
    }
}