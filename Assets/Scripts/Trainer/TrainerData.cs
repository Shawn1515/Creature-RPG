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

    public TrainerType trainerType;

    public int badgeNumber;

    public string trainerName;

    [TextArea]
    public string[] introDialogue;

    [TextArea]
    public string[] defeatDialogue;

    public CreatureData[] creatures;

    public int[] creatureLevels;

    public int rewardMoney;

    public float groundOffset;

    public Vector3 position;
}