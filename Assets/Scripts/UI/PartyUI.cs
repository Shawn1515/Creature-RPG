using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyUI : MonoBehaviour
{
    public GameObject partyPanel;
    public Button[] slotButtons;
    public Button switchButton;
    public Button closeButton;
    private int selectedIndex = -1;
    private bool isBattleSwitch = false;

    [Header("Action Panel")]
    public GameObject actionPanel;

    public Button swapButton;
    public Button summaryButton;
    public Button movesetButton;
    public Button cancelButton;


    public GameObject summaryPanel;
    public TextMeshProUGUI creatureNameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI movesText;
    public Button summaryCloseButton;

    public GameObject movesetPanel;
    public Transform currentMovesContainer;
    public Transform unlockedMovesContainer;
    public Button moveButtonPrefab;

    private bool movingCreature = false;
    private int movingIndex = -1;
    private bool closePressed = false;

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

        swapButton.onClick.AddListener(StartPartyMove);
        closeButton.onClick.AddListener(() => {
            closePressed = true;
            CloseParty();
            closePressed = false;
        });
        summaryButton.onClick.AddListener(OpenSummary);
        movesetButton.onClick.AddListener(() => {
            MovesetUI.Instance.Open(PartyManager.Instance.party[selectedIndex]);
        });
        cancelButton.onClick.AddListener(CloseActionPanel);
        summaryCloseButton.onClick.AddListener(CloseSummary);
        switchButton.onClick.AddListener(() =>
        {
            if(selectedIndex == -1)
            {
                return;
            }
            BattleManager.Instance.SwitchCreature(selectedIndex);
        });
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
        actionPanel.SetActive(false);
        summaryPanel.SetActive(false);
        closeButton.gameObject.SetActive(true);
        switchButton.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UpdateSlots();
        selectedIndex = -1;
    }

    public void CloseSummary()
    {
        summaryPanel.SetActive(false);
    }

    public void CloseParty()
    {
        partyPanel.SetActive(false);
        actionPanel.SetActive(false);
        summaryPanel.SetActive(false);
        movingCreature = false;
        movingIndex = -1;
        selectedIndex = -1;

        if(isBattleSwitch && closePressed)
        {
            BattleManager.Instance.SetMoveButtonsActive(true);
            GameManager.Instance.SetState(GameState.Battle);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if(isBattleSwitch)
        {
            GameManager.Instance.SetState(GameState.Battle);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            GameManager.Instance.SetState(GameState.Exploration);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
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
        if(movingCreature)
        {
            FinishPartyMove(index);
            return;
        }
        selectedIndex = index;
        OpenActionPanel(index);
    }

    void FinishPartyMove(int newIndex)
    {
        PartyManager.Instance.SwapCreatures(movingIndex, newIndex);
        movingCreature = false;
        movingIndex = -1;
        UpdateSlots();
        selectedIndex = -1;
    }

    void OpenActionPanel(int index)
    {
        if(!isBattleSwitch)
        {
            actionPanel.SetActive(true);
            actionPanel.transform.position = slotButtons[index].transform.position + new Vector3(200f, 0f, 0f);
        }
    }

    void CloseActionPanel()
    {
        actionPanel.SetActive(false);
    }

    void StartPartyMove()
    {
        movingCreature = true;
        movingIndex = selectedIndex;
        actionPanel.SetActive(false);
        Debug.Log("Choose where to move the creature");
    }

    void OpenSummary()
    {
        actionPanel.SetActive(false);

        DisplayCreature(PartyManager.Instance.party[selectedIndex]);
    }

    void MakeLeader()
    {
        if (selectedIndex == -1 || PartyManager.Instance.party[selectedIndex].currentHP <= 0)
        {
            return;
        }

        PartyManager.Instance.SetLeader(selectedIndex);

        UpdateSlots();
        actionPanel.SetActive(false);

        selectedIndex = -1;
    }

    void DisplayCreature(CreatureInstance creature)
    {
        summaryPanel.SetActive(true);
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
        closeButton.gameObject.SetActive(!BattleManager.Instance.IsForcedSwitch);
        switchButton.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UpdateSlots();
    }
}