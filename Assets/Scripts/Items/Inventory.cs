
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemStack
{
    public ItemData item;
    public int quantity;

    public ItemStack(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

public class Inventory : MonoBehaviour
{
    public List<ItemStack> items = new List<ItemStack>();

    public void AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return;

        int maxStack = item.stackable ? item.maxStack : 1;

        while (amount > 0)
        {
            ItemStack stack = items.Find(x =>
                x.item == item && x.quantity < maxStack
            );

            if (stack == null)
            {
                int quantityToAdd = Mathf.Min(amount, maxStack);
                items.Add(new ItemStack(item, quantityToAdd));
                amount -= quantityToAdd;
            }
            else
            {
                int space = maxStack - stack.quantity;
                int quantityToAdd = Mathf.Min(amount, space);

                stack.quantity += quantityToAdd;
                amount -= quantityToAdd;
            }
        }
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        int total = GetItemCount(item);

        if (total < amount)
            return false;

        for (int i = items.Count - 1; i >= 0 && amount > 0; i--)
        {
            if (items[i].item != item)
                continue;

            int remove = Mathf.Min(amount, items[i].quantity);
            items[i].quantity -= remove;
            amount -= remove;

            if (items[i].quantity <= 0)
                items.RemoveAt(i);
        }

        return true;
    }

    public int GetItemCount(ItemData item)
    {
        int count = 0;

        foreach (ItemStack stack in items)
        {
            if (stack.item == item)
                count += stack.quantity;
        }

        return count;
    }
}
