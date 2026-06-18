using UnityEngine;

[System.Serializable]
public class CreatureInstance
{
    public CreatureData species;

    public int level;

    public int currentHP;

    public int experience;

    public int MaxHP;

    public int Attack;

    public int Defense;

    public int Speed;

    public MoveData[] Moves;


    public CreatureInstance(CreatureData data)
    {
        species = data;

        level = 1;

        currentHP = data.maxHP;

        experience = 0;

        MaxHP = species.maxHP;

        Attack = species.attack;

        Defense = species.defense;

        Speed = species.speed;

        Moves = species.moves;

    }

    public GameObject CreaturePrefab => species.creaturePrefab;
    public string CreatureName => species.creatureName;

    public int ExperienceNeeded()
    {
        return level * 25;
    }

    public bool GainExperience(int amount)
    {
        bool up = false;
        experience += amount;
        while(experience >= ExperienceNeeded())
        {
            experience -= ExperienceNeeded();
            up = true;
            LevelUp();
        }
        return up;
    }

    void LevelUp()
    {
        level++;

        MaxHP += 5;
        Attack += 2;
        Defense += 2;
        Speed += 1;

        currentHP += 5;
    }

    public void HealFull()
    {
        currentHP = MaxHP;
    }
}