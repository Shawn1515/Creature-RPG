using UnityEngine;
using System.Collections.Generic;

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

    public List<MoveData> Moves = new List<MoveData>();

    public List<MoveData> UnlockedMoves = new List<MoveData>();

    public MoveData NewlyUnlockedMove {get; private set;}

    public bool CanEvolve;
    private int statTotal;


    public CreatureInstance(CreatureData data)
    {
        species = data;

        level = 1;

        CanEvolve = CheckEvolution();

        experience = 0;

        statTotal = species.total;

        MaxHP = Mathf.CeilToInt(species.maxHP * statTotal);

        currentHP = MaxHP;

        Attack = Mathf.CeilToInt(species.attack * statTotal);

        Defense = Mathf.CeilToInt(species.defense * statTotal);

        Speed = Mathf.CeilToInt(species.speed * statTotal);

        foreach(MoveData move in species.moves)
        {
            Moves.Add(move);
        }
        foreach(LearnableMove learnable in species.learnableMoves)
        {
            if(learnable.level <= level && !UnlockedMoves.Contains(learnable.move))
            {
                UnlockedMoves.Add(learnable.move);
            }
        }

    }

    public CreatureInstance(CreatureData data, int lvl)
    {
        species = data;

        level = lvl;

        CanEvolve = CheckEvolution();

        statTotal = species.total + (level - 1) * 5;


        experience = 0;

        MaxHP = Mathf.CeilToInt(species.maxHP * statTotal);
        Attack = Mathf.CeilToInt(species.attack * statTotal);
        Defense = Mathf.CeilToInt(species.defense * statTotal);
        Speed = Mathf.CeilToInt(species.speed * statTotal);
        
        currentHP = MaxHP;

        foreach(MoveData move in species.moves)
        {
            Moves.Add(move);
        }

        foreach(LearnableMove learnable in species.learnableMoves)
        {
            if(learnable.level <= level && !UnlockedMoves.Contains(learnable.move))
            {
                UnlockedMoves.Add(learnable.move);
            }
        }

    }

    public GameObject CreaturePrefab => species.creaturePrefab;
    public GameObject WildPrefab => species.wildPrefab;
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
            if(level >= 50)
            {
                experience = 0;
            }
            else
            {
                up = true;
                LevelUp();
            }
        }
        return up;
    }

    void LevelUp()
    {
        level++;

        CanEvolve = CheckEvolution();

        int prevMaxHP = MaxHP;
        statTotal += 5;
        MaxHP = Mathf.CeilToInt(species.maxHP * statTotal);
        Attack = Mathf.CeilToInt(species.attack * statTotal);
        Defense = Mathf.CeilToInt(species.defense * statTotal);
        Speed = Mathf.CeilToInt(species.speed * statTotal);

        currentHP += MaxHP - prevMaxHP;

        MoveData newMove = UnlockMoves();
        NewlyUnlockedMove = null;
        if(newMove != null)
        {
            NewlyUnlockedMove = newMove;
        }
    }

    MoveData UnlockMoves()
    {
        foreach (LearnableMove learnable in species.learnableMoves)
        {
            if (learnable.level == level && UnlockMove(learnable.move))
            {
                return learnable.move;
            }
        }
        return null;
    }

    public bool UnlockMove(MoveData move)
    {
        if (UnlockedMoves.Contains(move))
        {
            return false;
        }

        UnlockedMoves.Add(move);
        return true;
    }

    public void HealFull()
    {
        currentHP = MaxHP;
    }

    public bool CheckEvolution()
    {
        if(species.evolution != null && level >= species.evolutionLevel)
        {
            return true;
        }
        return false;
    }

    public void Evolve()
    {
        species = species.evolution;
        int temp = MaxHP - currentHP;
        statTotal = species.total + (level - 1) * 5;
        MaxHP = Mathf.CeilToInt(species.maxHP * statTotal);
        Attack = Mathf.CeilToInt(species.attack * statTotal);
        Defense = Mathf.CeilToInt(species.defense * statTotal);
        Speed = Mathf.CeilToInt(species.speed * statTotal);
        currentHP = MaxHP - temp;
        FollowerManager.Instance.SpawnFollower();
    }
}