using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;

    public GameObject shopPanel;

    public TextMeshProUGUI moneyText;

    public Button buyButton;
    public Button closeButton;
    public int hatPrice = 50;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        buyButton.onClick.AddListener(BuyHats);
        closeButton.onClick.AddListener(CloseShop);
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);

        UpdateUI();

        GameManager.Instance.SetState(GameState.Party);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void UpdateUI()
    {
        buyButton.interactable =
            MoneyManager.Instance.CanAfford(hatPrice);
    }

    void BuyHats()
    {
        if (!MoneyManager.Instance.SpendMoney(hatPrice))
            return;

        HatManager.Instance.AddHats(1);

        UpdateUI();
    }

    void CloseShop()
    {
        shopPanel.SetActive(false);

        GameManager.Instance.SetState(GameState.Exploration);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}