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

    public void SetLeader(int index)
    {
        if(index <= 0 || index >= party.Count)
        {
            return;
        }
        (party[index], party[0]) = (party[0], party[index]);
        FollowerManager.Instance.SpawnFollower();
    }
}