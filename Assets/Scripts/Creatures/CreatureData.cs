using UnityEngine;

[CreateAssetMenu(fileName = "CreatureData", menuName = "Creature/Creature Data")]
public class CreatureData : ScriptableObject
{
    public string creatureName;
    public int maxHP;
    public int attack;
    public int defense;
    public int speed;
    
    public MoveData[] moves;

    [TextArea]
    public string[] encounterText;
}