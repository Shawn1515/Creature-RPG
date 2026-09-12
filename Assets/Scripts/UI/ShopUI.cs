using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;

    public GameObject shopPanel;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI hatCountText;

    public Button buyButton;
    public Button closeButton;

    public int hatsPerPurchase = 5;
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

        GameManager.Instance.CurrentState = GameState.Dialogue;
    }

    void UpdateUI()
    {
        moneyText.text = "$" + MoneyManager.Instance.money;
        hatCountText.text = "Hats: " + HatManager.Instance.hatCount;

        buyButton.interactable =
            MoneyManager.Instance.CanAfford(hatPrice);
    }

    void BuyHats()
    {
        if (!MoneyManager.Instance.SpendMoney(hatPrice))
            return;

        HatManager.Instance.AddHats(hatsPerPurchase);

        UpdateUI();
    }

    void CloseShop()
    {
        shopPanel.SetActive(false);

        GameManager.Instance.CurrentState = GameState.Exploration;
    }
}