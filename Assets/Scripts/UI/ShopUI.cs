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

        GameManager.Instance.SetState(GameState.Party);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void BuyHats()
    {
        if (!MoneyManager.Instance.SpendMoney(hatPrice))
        {
            string[] dialogue = {"You can't do that!"};
            DialogueUI.Instance.DialogueWithin(dialogue, "");
        }
        else
        {
            HatManager.Instance.AddHats(1);
            string[] dialogue = {"You bought a hat!"};
            DialogueUI.Instance.DialogueWithin(dialogue, "");
        }
    }

    void CloseShop()
    {
        shopPanel.SetActive(false);

        GameManager.Instance.SetState(GameState.Exploration);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}