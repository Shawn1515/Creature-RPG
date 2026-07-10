using UnityEngine;

[CreateAssetMenu(fileName = "CreatureData", menuName = "Creature/Creature Data")]
public class CreatureData : ScriptableObject
{
    public string creatureName;
    public int maxHP;
    public int attack;
    public int defense;
    public int speed;

    public float groundOffset;
    
    [Header("Starting Moves")]
    public MoveData[] moves;

    [Header("Moves Learned")]
    public LearnableMove[] learnableMoves;
    public GameObject creaturePrefab;
    public GameObject wildPrefab;

    [TextArea]
    public string[] encounterText;

    [Header("Rewards")]
    public int experienceReward = 20;

    [Header("Evolution")]
    public CreatureData evolution;
    public int evolutionLevel;

    [Header("Typing")]
    public CreatureType primaryType;
}