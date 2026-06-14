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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UpdateSlots();
        selectedIndex = -1;
    }

    void CloseParty()
    {
        GameManager.Instance.SetState(GameState.Exploration);
        partyPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        Debug.Log(
            "Selected " +
            PartyManager.Instance.party[index].species.creatureName
        );
    }

    void MakeLeader()
    {
        if (selectedIndex == -1)
        {
            return;
        }

        PartyManager.Instance.SetLeader(selectedIndex);

        UpdateSlots();

        selectedIndex = -1;
    }
}