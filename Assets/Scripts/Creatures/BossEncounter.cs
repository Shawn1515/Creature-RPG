using UnityEngine;

public class BossEncounter : MonoBehaviour
{
    [Header("Boss Settings")]
    public CreatureData creatureSpecies;
    public int bossLevel = 20;
    public float statMultiplier = 2f;

    [TextArea]
    public string[] encounterDialogue;

    [Header("Unlock")]
    public CreatureData unlockedCreature;
    public CreatureSpawnPoint[] areaSpawnPoints;

    private CreatureInstance bossCreature;
    private PlayerMovement playerMovement;
    private bool defeated;

    private void Start()
    {
        bossCreature = new CreatureInstance(creatureSpecies, bossLevel);

        bossCreature.MaxHP = Mathf.CeilToInt(bossCreature.MaxHP * statMultiplier);
        bossCreature.Attack = Mathf.CeilToInt(bossCreature.Attack * statMultiplier);
        bossCreature.Defense = Mathf.CeilToInt(bossCreature.Defense * statMultiplier);
        bossCreature.Speed = Mathf.CeilToInt(bossCreature.Speed * statMultiplier);

        bossCreature.currentHP = bossCreature.MaxHP;

        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || defeated)
            return;

        playerMovement?.SnapToGround();

        DialogueUI.Instance.SetPendingBattle(bossCreature, transform);
        DialogueUI.Instance.StartDialogue(encounterDialogue, "", false);
    }

    public void DefeatBoss()
    {
        if (defeated)
            return;

        defeated = true;

        Debug.Log("BOSS DEFEATED: " + creatureSpecies.creatureName);
        Debug.Log("Spawn points assigned: " + areaSpawnPoints.Length);

        foreach (CreatureSpawnPoint spawnPoint in areaSpawnPoints)
        {
            if (spawnPoint != null)
            {
                Debug.Log("Unlocking spawn point: " + spawnPoint.name);
                spawnPoint.UnlockArea();
            }
            else
            {
                Debug.LogWarning("A spawn point reference is NULL!");
            }
        }
    }
}