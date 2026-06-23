using UnityEngine;

public class CreatureDatabase : MonoBehaviour
{
    public static CreatureDatabase Instance;

    public CreatureData[] creatures;

    private void Awake()
    {
        Instance = this;
    }

    public CreatureData GetCreatureByName(string creatureName)
    {
        foreach (CreatureData creature in creatures)
        {
            if (creature.creatureName == creatureName)
            {
                return creature;
            }
        }

        return null;
    }
}