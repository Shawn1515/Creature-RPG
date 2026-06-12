using UnityEngine;

[System.Serializable]
public class CreatureInstance
{
    public CreatureData species;

    public int level;

    public int currentHP;

    public int experience;


    public CreatureInstance(CreatureData data)
    {
        species = data;

        level = 1;

        currentHP = data.maxHP;

        experience = 0;
    }

    public string CreatureName => species.creatureName;

    public int MaxHP => species.maxHP;

    public int Attack => species.attack;

    public int Defense => species.defense;

    public int Speed => species.speed;

    public MoveData[] Moves => species.moves;
}