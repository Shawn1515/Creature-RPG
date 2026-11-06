using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Transform itemGrid;
    public ItemSlotUI[] itemSlots;
    public TMP_Text itemDescription;
    public Button closeButton;

    private Inventory inventory;

    private void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();
        inventoryPanel.SetActive(false);

        closeButton.onClick.AddListener(CloseInventory);
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState == GameState.Exploration &&
            Input.GetKeyDown(KeyCode.I))
        {
            OpenInventory();
        }
    }

    private void OpenInventory()
    {
        RefreshInventory();
        inventoryPanel.SetActive(true);
        GameManager.Instance.SetState(GameState.Party);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RefreshInventory()
    {
        if (inventory == null)
            return;

        itemDescription.text = "Select an item to see its description.";

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                ItemStack stack = inventory.items[i];

                itemSlots[i].gameObject.GetComponent<Button>().interactable = true;
                itemSlots[i].Setup(stack, () => SelectItem(stack.item));
            }
            else
            {
                itemSlots[i].gameObject.GetComponent<Button>().interactable = false;

                itemSlots[i].itemIcon.gameObject.SetActive(false);
                itemSlots[i].quantityText.gameObject.SetActive(false);
            }
        }
    }

    private void SelectItem(ItemData item)
    {
        itemDescription.text = item.displayName + "\n" + item.description;
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        GameManager.Instance.SetState(GameState.Exploration);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}