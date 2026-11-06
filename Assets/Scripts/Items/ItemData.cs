
using UnityEngine;

public enum ItemCategory
{
    Consumable,
    Capture,
    Held
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
}
