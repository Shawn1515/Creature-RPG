using UnityEngine;

public class PlayerCreatureManager : MonoBehaviour
{
    public static PlayerCreatureManager Instance;

    [Header("Starter Creature")]
    public CreatureData starterCreatureData;

    private CreatureInstance starterCreature;

    private void Awake()
    {
        Instance = this;
        starterCreature = new CreatureInstance(starterCreatureData, 5);
        PartyManager.Instance.AddCreature(starterCreature);
    }
}