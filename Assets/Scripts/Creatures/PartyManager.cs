using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance;
    public List<CreatureInstance> party = new List<CreatureInstance>();

    private void Awake()
    {
        Instance = this;
    }

    public CreatureInstance GetActiveCreature()
    {
        return party[0];
    }

    public bool AddCreature(CreatureInstance creature)
    {
        if(party.Count >= 6)
        {
            return false;
        }
        party.Add(creature);
        return true;
    }

    public void SwapCreatures(int firstIndex, int secondIndex)
    {
        (party[firstIndex], party[secondIndex]) = (party[secondIndex], party[firstIndex]);
        if(firstIndex == 0 || secondIndex == 0)
        {
            FollowerManager.Instance.SpawnFollower();
        }
    }

    public void SetLeader(int index)
    {
        if(index <= 0 || index >= party.Count)
        {
            return;
        }
        SwapCreatures(0, index);
    }

    public bool HasUsableCreature()
    {
        foreach(CreatureInstance creature in party)
        {
            if(creature.currentHP > 0)
            {
                return true;
            }
        }
        return false;
    }

    public void HealParty()
    {
        foreach(CreatureInstance creature in party)
        {
            creature.HealFull();
        }
    }
}