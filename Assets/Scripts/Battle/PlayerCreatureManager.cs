using UnityEngine;

public class PlayerCreatureManager : MonoBehaviour
{
    public static PlayerCreatureManager Instance;
    public CreatureData starterCreature;
    private void Awake()
    {
        Instance = this;
    }
}