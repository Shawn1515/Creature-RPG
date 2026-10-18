using UnityEngine;
using System.Collections;

public class CreatureSpawnPoint : MonoBehaviour
{
    public GameObject[] possibleCreatures;
    public float respawnTime = 30f;

    private GameObject currentCreature;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        bool firstAttempt = true;

        while (true)
        {
            if (currentCreature != null)
            {
                firstAttempt = false;
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            if (!firstAttempt)
            {
                yield return new WaitForSeconds(respawnTime);
            }

            firstAttempt = false;

            while (currentCreature == null &&
                   GameManager.Instance.CurrentState != GameState.Exploration)
            {
                yield return new WaitForSeconds(respawnTime);
            }

            if (currentCreature != null)
            {
                continue;
            }

            SpawnCreature();
        }
    }

    public void SpawnCreature()
    {
        if (currentCreature != null)
        {
            return;
        }

        if (GameManager.Instance.CurrentState != GameState.Exploration)
        {
            return;
        }

        if (possibleCreatures == null || possibleCreatures.Length == 0)
        {
            Debug.LogWarning($"{name}: No possible creatures assigned.");
            return;
        }

        int groundMask = LayerMask.GetMask("Ground");

        if (!Physics.Raycast(
                transform.position + Vector3.up * 50f,
                Vector3.down,
                out RaycastHit hit,
                200f,
                groundMask))
        {
            Debug.LogWarning($"{name}: Ground raycast failed.");
            return;
        }

        int index = Random.Range(0, possibleCreatures.Length);

        GameObject prefab = possibleCreatures[index];

        if (prefab == null)
        {
            Debug.LogWarning($"{name}: Selected creature prefab is missing.");
            return;
        }

        CreatureEncounter prefabEncounter =
            prefab.GetComponent<CreatureEncounter>();

        if (prefabEncounter == null)
        {
            Debug.LogWarning(
                $"{name}: {prefab.name} has no CreatureEncounter component.");
            return;
        }

        Vector3 spawnPosition =
            hit.point + Vector3.up * prefabEncounter.creatureSpecies.groundOffset;

        currentCreature = Instantiate(
            prefab,
            spawnPosition,
            Quaternion.identity
        );

        CreatureEncounter encounter =
            currentCreature.GetComponent<CreatureEncounter>();

        encounter.spawnPoint = this;

        Debug.Log($"{name}: Spawned {currentCreature.name}.");
    }

    public void CreatureDefeated()
    {
        currentCreature = null;
    }
}
