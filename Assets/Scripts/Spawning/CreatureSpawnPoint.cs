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

        if(GameManager.Instance.CurrentState != GameState.Exploration)
        {
            CreatureDefeated();
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
        
        Physics.Raycast(transform.position + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 200f, LayerMask.GetMask("Ground"));
        Vector3 pos = hit.point + Vector3.up * encounter.creatureSpecies.groundOffset;

        currentCreature.transform.position = pos;

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