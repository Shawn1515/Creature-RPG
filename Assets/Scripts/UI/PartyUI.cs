using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyUI : MonoBehaviour
{
    public GameObject partyPanel;
    public Button[] slotButtons;
    public Button makeLeaderButton;
    public Button closeButton;
    private int selectedIndex = -1;
    private bool isBattleSwitch = false;

    [Header("Details Panel")]
    public GameObject detailsPanel;

    public TextMeshProUGUI creatureNameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI movesText;

    public static PartyUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        partyPanel.SetActive(false);

        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i;

            slotButtons[i].onClick.AddListener(() =>
            {
                SelectSlot(index);
            });
        }

        makeLeaderButton.onClick.AddListener(MakeLeader);
        closeButton.onClick.AddListener(CloseParty);
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState == GameState.Exploration && Input.GetKeyDown(KeyCode.P))
        {
            OpenParty();
        }
    }

    void OpenParty()
    {
        GameManager.Instance.SetState(GameState.Party);

        partyPanel.SetActive(true);
        detailsPanel.SetActive(false);

        makeLeaderButton.GetComponentInChildren<TextMeshProUGUI>().text = "Make Leader";
        makeLeaderButton.onClick.RemoveAllListeners();
        makeLeaderButton.onClick.AddListener(MakeLeader);
        closeButton.interactable = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UpdateSlots();
        selectedIndex = -1;
    }

    public void CloseParty()
    {
        partyPanel.SetActive(false);
        detailsPanel.SetActive(false);

        if(isBattleSwitch)
        {
            GameManager.Instance.SetState(GameState.Battle);
            BattleManager.Instance.SetMoveButtonsActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            GameManager.Instance.SetState(GameState.Exploration);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        makeLeaderButton.gameObject.SetActive(true);
        isBattleSwitch = false;
    }

    void UpdateSlots()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            TextMeshProUGUI text =
                slotButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            if (i < PartyManager.Instance.party.Count)
            {
                CreatureInstance creature =
                    PartyManager.Instance.party[i];

                text.text =
                    creature.species.creatureName +
                    "\nLv " +
                    creature.level +
                    "\nHP " +
                    creature.currentHP +
                    "/" +
                    creature.MaxHP;
            }
            else
            {
                text.text = "Empty";
            }
        }
    }

    void SelectSlot(int index)
    {
        if (index >= PartyManager.Instance.party.Count)
        {
            return;
        }

        selectedIndex = index;
        DisplayCreature(PartyManager.Instance.party[index]);
    }

    void MakeLeader()
    {
        if (selectedIndex == -1 || PartyManager.Instance.party[selectedIndex].currentHP <= 0)
        {
            return;
        }

        PartyManager.Instance.SetLeader(selectedIndex);

        UpdateSlots();
        detailsPanel.SetActive(false);

        selectedIndex = -1;
    }

    void DisplayCreature(CreatureInstance creature)
    {
        detailsPanel.SetActive(true);
        creatureNameText.text = creature.CreatureName;

        levelText.text = "Level: " + creature.level;
        hpText.text = "HP: " + creature.currentHP + "/" + creature.MaxHP;
        attackText.text = "Attack: " + creature.Attack;
        defenseText.text = "Defense: " + creature.Defense;
        speedText.text = "Speed: " + creature.Speed;
        xpText.text = "XP: " + creature.experience + "/" + creature.ExperienceNeeded();
        string moveString = "Moves:\n";

        foreach (MoveData move in creature.Moves)
        {
            moveString += move.moveName + "\n";
        }

        movesText.text = moveString;
    }

    public void OpenForBattle()
    {
        isBattleSwitch = true;
        partyPanel.SetActive(true);
        closeButton.interactable = !BattleManager.Instance.IsForcedSwitch;
        makeLeaderButton.GetComponentInChildren<TextMeshProUGUI>().text = "Switch";
        makeLeaderButton.onClick.RemoveAllListeners();
        makeLeaderButton.onClick.AddListener(() =>
        {
            if(selectedIndex == -1)
            {
                return;
            }
            BattleManager.Instance.SwitchCreature(selectedIndex);
        });
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UpdateSlots();
    }
}