using UnityEngine;

public enum TrainerType
    {
        Normal,
        GymLeader,
        Champion
    }

[CreateAssetMenu(fileName = "TrainerData", menuName = "Trainer/Trainer Data")]
public class TrainerData : ScriptableObject
{
    public string trainerName;

    [TextArea]
    public string[] introDialogue;

    [TextArea]
    public string[] defeatDialogue;

    [Header("Random Party")]
    public CreatureData[] possibleCreatures;
    public int minPartySize = 2;
    public int maxPartySize = 4;

    public CreatureData[] creatures;

    public int[] creatureLevels;

    public int rewardMoney;

    public float groundOffset;
}