
using UnityEngine;

public class HatManager : MonoBehaviour
{
    public static HatManager Instance;

    [Header("Inventory Connection")]
    public ItemData hatItem;

    public int hatCount = 0;

    private Inventory inventory;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();

        if (inventory == null || hatItem == null)
        {
            Debug.LogError("HatManager is missing its Inventory or Hat Item reference!");
            return;
        }

        // Add the starting hats if the inventory doesn't already contain any.
        if (inventory.GetItemCount(hatItem) == 0 && hatCount > 0)
        {
            inventory.AddItem(hatItem, hatCount);
        }

        RefreshHatCount();
    }

    private Inventory GetInventory()
    {
        if (inventory == null)
            inventory = FindAnyObjectByType<Inventory>();

        return inventory;
    }

    public void RefreshHatCount()
    {
        Inventory inv = GetInventory();

        if (inv == null || hatItem == null)
            return;

        hatCount = inv.GetItemCount(hatItem);
    }

    public bool HasHat()
    {
        Inventory inv = GetInventory();

        return inv != null &&
               hatItem != null &&
               inv.GetItemCount(hatItem) > 0;
    }

    public bool UseHat()
    {
        Inventory inv = GetInventory();

        if (inv == null || hatItem == null)
            return false;

        return inv.RemoveItem(hatItem, 1);
    }

    public void AddHats(int amount)
    {
        Inventory inv = GetInventory();

        if (inv == null || hatItem == null || amount <= 0)
            return;

        inv.AddItem(hatItem, amount);
    }
}
