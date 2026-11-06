
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemSlotUI : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text quantityText;
    public Button button;

    public void Setup(ItemStack stack, Action onClick)
    {
        itemIcon.sprite = stack.item.icon;
        itemIcon.enabled = stack.item.icon != null;

        quantityText.text = stack.quantity.ToString();
        itemIcon.gameObject.SetActive(true);
        quantityText.gameObject.SetActive(true);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
    }
}
