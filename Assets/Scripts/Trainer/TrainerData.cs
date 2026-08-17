using UnityEngine;

[CreateAssetMenu(fileName = "TrainerData", menuName = "Trainer/Trainer Data")]
public class TrainerData : ScriptableObject
{
    public string trainerName;

    [TextArea]
    public string[] introDialogue;

    [TextArea]
    public string[] defeatDialogue;

    public CreatureData[] creatures;

    public int[] creatureLevels;

    public int rewardMoney;

    public float groundOffset;
}