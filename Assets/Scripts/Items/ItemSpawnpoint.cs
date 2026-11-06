
using UnityEngine;
using System.Collections;

public class ItemSpawnPoint : MonoBehaviour
{
    public ItemData[] possibleItems;
    public GameObject itemPickupPrefab;
    public float respawnTime = 30f;

    private GameObject currentItem;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        bool firstAttempt = true;

        while (true)
        {
            if (currentItem != null)
            {
                firstAttempt = false;
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            if (!firstAttempt)
                yield return new WaitForSeconds(respawnTime);

            firstAttempt = false;

            while (currentItem == null &&
                   GameManager.Instance.CurrentState != GameState.Exploration)
            {
                yield return new WaitForSeconds(0.5f);
            }

            if (currentItem != null)
                continue;

            SpawnItem();
        }
    }

    private void SpawnItem()
    {
        if (possibleItems == null || possibleItems.Length == 0)
            return;

        if (itemPickupPrefab == null)
            return;

        if (GameManager.Instance.CurrentState != GameState.Exploration)
            return;

        ItemData item = possibleItems[Random.Range(0, possibleItems.Length)];

        if (item == null)
            return;

        int groundMask = LayerMask.GetMask("Ground");

        if (!Physics.Raycast(
            transform.position + Vector3.up * 50f,
            Vector3.down,
            out RaycastHit hit,
            200f,
            groundMask))
        {
            Debug.LogWarning($"{name}: Ground raycast failed.");
            return;
        }

        currentItem = Instantiate(
            itemPickupPrefab,
            hit.point + new Vector3(0f,0.07f,0f),
            transform.rotation
        );

        ItemPickup pickup = currentItem.GetComponent<ItemPickup>();

        if (pickup != null)
        {
            pickup.item = item;
            pickup.spawnPoint = this;
        }
    }

    public void ItemCollected()
    {
        currentItem = null;
    }
}
