using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CreatureInstance
{
    public CreatureData species;

    public int level;

    public int currentHP;

    public int experience;

    public int baseHP;

    public float statMultiplier = 1f;

    private int baseAttack;
    private int baseDefense;
    private int baseSpeed;

    public int MaxHP => Mathf.RoundToInt(
        baseHP * statMultiplier * (heldItem != null ? heldItem.healthBonus : 1f)
    );

    public int Attack => Mathf.RoundToInt(
        baseAttack * statMultiplier * (heldItem != null ? heldItem.attackBonus : 1f)
    );

    public int Defense => Mathf.RoundToInt(
        baseDefense * statMultiplier * (heldItem != null ? heldItem.defenseBonus : 1f)
    );

    public int Speed => Mathf.RoundToInt(
        baseSpeed * statMultiplier * (heldItem != null ? heldItem.speedBonus : 1f)
    );

    public ItemData heldItem;

    public List<MoveData> Moves = new List<MoveData>();

    public List<MoveData> UnlockedMoves = new List<MoveData>();

    public MoveData NewlyUnlockedMove { get; private set; }

    public bool CanEvolve;
    private int statTotal;

    public CreatureInstance(CreatureData data)
    {
        species = data;

        level = 1;

        CanEvolve = CheckEvolution();

        experience = 0;

        statTotal = species.total;

        baseHP = Mathf.CeilToInt(species.maxHP * statTotal);
        currentHP = MaxHP;

        baseAttack = Mathf.CeilToInt(species.attack * statTotal);

        baseDefense = Mathf.CeilToInt(species.defense * statTotal);

        baseSpeed = Mathf.CeilToInt(species.speed * statTotal);

        foreach (MoveData move in species.moves)
        {
            Moves.Add(move);
        }

        foreach (LearnableMove learnable in species.learnableMoves)
        {
            if (learnable.level <= level && !UnlockedMoves.Contains(learnable.move))
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

        baseHP = Mathf.CeilToInt(species.maxHP * statTotal);
        baseAttack = Mathf.CeilToInt(species.attack * statTotal);

        baseDefense = Mathf.CeilToInt(species.defense * statTotal);

        baseSpeed = Mathf.CeilToInt(species.speed * statTotal);

        currentHP = MaxHP;

        foreach (MoveData move in species.moves)
        {
            Moves.Add(move);
        }

        foreach (LearnableMove learnable in species.learnableMoves)
        {
            if (learnable.level <= level && !UnlockedMoves.Contains(learnable.move))
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
        if (heldItem != null)
        {
            amount = Mathf.RoundToInt(amount * heldItem.experienceBonus);
        }
        experience += amount;
        while (experience >= ExperienceNeeded())
        {
            experience -= ExperienceNeeded();

            if (level >= 50)
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

        baseHP = Mathf.CeilToInt(species.maxHP * statTotal);
        
        baseAttack = Mathf.CeilToInt(species.attack * statTotal);

        baseDefense = Mathf.CeilToInt(species.defense * statTotal);

        baseSpeed = Mathf.CeilToInt(species.speed * statTotal);

        currentHP += MaxHP - prevMaxHP;

        MoveData newMove = UnlockMoves();

        NewlyUnlockedMove = null;

        if (newMove != null)
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
        if (species.evolution != null && level >= species.evolutionLevel)
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

        baseHP = Mathf.CeilToInt(species.maxHP * statTotal);

        baseAttack = Mathf.CeilToInt(species.attack * statTotal);

        baseDefense = Mathf.CeilToInt(species.defense * statTotal);

        baseSpeed = Mathf.CeilToInt(species.speed * statTotal);

        currentHP = MaxHP - temp;

        FollowerManager.Instance.SpawnFollower();
    }

    public bool EquipItem(ItemData item, Inventory inventory)
    {
        if (item == null ||
            item.category != ItemCategory.Held ||
            inventory == null ||
            inventory.GetItemCount(item) <= 0)
        {
            return false;
        }

        if (!inventory.RemoveItem(item))
        {
            return false;
        }

        if (heldItem != null)
        {
            currentHP = Mathf.RoundToInt(currentHP / heldItem.healthBonus);
            inventory.AddItem(heldItem);
        }
        currentHP = Mathf.RoundToInt(currentHP * item.healthBonus);
        heldItem = item;
        currentHP = Mathf.Min(currentHP, MaxHP);
        return true;
    }

    public void RemoveHeldItem(Inventory inventory)
    {
        if (heldItem == null || inventory == null)
        {
            return;
        }
        ItemData oldItem = heldItem;
        currentHP = Mathf.RoundToInt(currentHP / oldItem.healthBonus);
        heldItem = null;
        currentHP = Mathf.Min(currentHP, MaxHP);
        inventory.AddItem(oldItem);
    }
}
