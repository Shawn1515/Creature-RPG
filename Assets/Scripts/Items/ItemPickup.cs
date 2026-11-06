
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData item;
    public int quantity = 1;

    public ItemSpawnPoint spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Inventory inventory = other.GetComponent<Inventory>();

        if (inventory == null)
        {
            Debug.LogWarning("Player has no Inventory component!");
            return;
        }

        if (item == null)
            return;

        inventory.AddItem(item, quantity);

        Debug.Log($"Collected {quantity} {item.displayName}!");

        spawnPoint?.ItemCollected();

        Destroy(gameObject);
    }
}