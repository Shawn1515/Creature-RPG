using UnityEngine;

[CreateAssetMenu(
    fileName = "MoveData",
    menuName = "Creature/Move Data"
)]
public class MoveData : ScriptableObject
{
    public string moveName;

    public CreatureType moveType;
    public int power = 5;
}