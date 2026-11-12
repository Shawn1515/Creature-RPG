
using UnityEngine;

public enum ItemCategory
{
    Consumable,
    Capture,
    Held
}

public enum ItemEffect
{
    HealHP,
    Revive
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Information")]
    public string displayName;
    [TextArea]
    public string description;
    public Sprite icon;

    [Header("Item Settings")]
    public ItemCategory category;
    public bool stackable = true;
    public int maxStack = 99;

    [Header("Consumable Effect")]
    public ItemEffect effect;
    public int effectAmount = 20;
    
    [Header("Held Item Stat Bonuses")]
    public float attackBonus = 1f;
    public float defenseBonus = 1f;
    public float speedBonus = 1f;
    public float healthBonus = 1f;
    public float experienceBonus = 1f;
}
