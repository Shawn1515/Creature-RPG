
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
    public Button useButton;

    private Inventory inventory;
    private ItemData selectedItem;
    private ItemData itemBeingUsed;

    public static InventoryUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();

        inventoryPanel.SetActive(false);

        closeButton.onClick.AddListener(CloseInventory);
        useButton.onClick.AddListener(UseSelectedItem);

        useButton.interactable = false;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState == GameState.Exploration &&
            Input.GetKeyDown(KeyCode.I))
        {
            OpenInventory();
        }
    }

    public void OpenInventory()
    {
        RefreshInventory();

        inventoryPanel.SetActive(true);
        GameManager.Instance.SetState(GameState.Party);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private CreatureInstance GetCurrentFollowerCreature()
    {
        foreach (CreatureInstance creature in PartyManager.Instance.party)
        {
            if (creature.currentHP > 0)
                return creature;
        }

        return null;
    }

    public void RefreshInventory()
    {
        if (inventory == null)
            return;

        itemDescription.text = "Select an item to see its description.";

        selectedItem = null;
        useButton.interactable = false;
        useButton.GetComponentInChildren<TMP_Text>().text = "";

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
        selectedItem = item;

        itemDescription.text = item.displayName + "\n" + item.description;

        useButton.interactable =
            item.category == ItemCategory.Consumable;

        if (useButton.interactable)
        {
            useButton.GetComponentInChildren<TMP_Text>().text = "Use";
        }
        else
        {
            useButton.GetComponentInChildren<TMP_Text>().text = "";
        }
    }

    private void UseSelectedItem()
    {
        if (selectedItem == null)
            return;

        if (selectedItem.category != ItemCategory.Consumable)
            return;

        itemBeingUsed = selectedItem;

        inventoryPanel.SetActive(false);

        PartyUI.Instance.BeginItemTargetSelection(ApplyItem);
    }

    private void ApplyItem(CreatureInstance creature)
    {
        if (creature == null)
        {
            itemBeingUsed = null;
            return;
        }

        if (itemBeingUsed == null)
        {
            Debug.LogError("No item was selected for use!");
            return;
        }

        CreatureInstance oldFollower = GetCurrentFollowerCreature();

        bool itemUsed = false;

        switch (itemBeingUsed.effect)
        {
            case ItemEffect.HealHP:

                if (creature.currentHP > 0 &&
                    creature.currentHP < creature.MaxHP)
                {
                    creature.currentHP = Mathf.Min(
                        creature.currentHP + itemBeingUsed.effectAmount,
                        creature.MaxHP
                    );

                    itemUsed = true;
                }

                break;

            case ItemEffect.Revive:

                if (creature.currentHP <= 0)
                {
                    int restoredHP = Mathf.CeilToInt(
                        creature.MaxHP * 0.5f
                    );

                    creature.currentHP = Mathf.Clamp(
                        restoredHP,
                        1,
                        creature.MaxHP
                    );

                    itemUsed = true;
                }

                break;
        }

        if (itemUsed)
        {
            inventory.RemoveItem(itemBeingUsed);
            CreatureInstance newFollower = GetCurrentFollowerCreature();

            if (oldFollower != newFollower)
            {
                ScreenFade.Instance.BattleTransition(
                    FollowerManager.Instance.SpawnFollower
                );
            }

            Debug.Log(creature.CreatureName + " used " +
                      itemBeingUsed.displayName + "!");

            itemBeingUsed = null;

            RefreshInventory();
            PartyUI.Instance.RefreshPartySlots();
        }
        else
        {
            itemDescription.text =
                "That item cannot be used on " + creature.CreatureName + ".";

            itemBeingUsed = null;
        }
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);

        selectedItem = null;
        itemBeingUsed = null;

        GameManager.Instance.SetState(GameState.Exploration);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}