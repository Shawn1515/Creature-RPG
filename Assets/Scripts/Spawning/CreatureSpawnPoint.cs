using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreatureSpawnPoint : MonoBehaviour
{
    public GameObject[] possibleCreatures;
    public float respawnTime = 30f;

    [Header("Boss Area")]
    public bool requiresBossDefeat;
    public GameObject unlockedCreaturePrefab;

    private bool areaUnlocked;

    private GameObject currentCreature;

    private List<GameObject> availableCreatures = new List<GameObject>();

    private void Start()
    {
        if (!requiresBossDefeat)
        {
            availableCreatures.AddRange(possibleCreatures);
            areaUnlocked = true;
        }
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
        if (!areaUnlocked)
        {
            return;
        }

        if (currentCreature != null)
        {
            return;
        }

        if (GameManager.Instance.CurrentState != GameState.Exploration)
        {
            return;
        }

        if (availableCreatures.Count == 0)
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

        int index = Random.Range(0, availableCreatures.Count);
        GameObject prefab = availableCreatures[index];
        
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

    public void UnlockCreature(GameObject creaturePrefab)
    {
        if (creaturePrefab == null)
            return;

        if (!availableCreatures.Contains(creaturePrefab))
        {
            availableCreatures.Add(creaturePrefab);
            Debug.Log($"{name}: Unlocked {creaturePrefab.name}!");
        }
    }

    public void UnlockArea()
    {
        if (areaUnlocked)
            return;

        areaUnlocked = true;
        availableCreatures.Clear();

        if (unlockedCreaturePrefab == null)
        {
            Debug.LogError(name + ": Unlocked creature prefab is NULL!");
            return;
        }

        availableCreatures.Add(unlockedCreaturePrefab);

        Debug.Log(name + ": AREA UNLOCKED! Available creature: " + unlockedCreaturePrefab.name);
    }

    public void CreatureDefeated()
    {
        currentCreature = null;
    }
}
