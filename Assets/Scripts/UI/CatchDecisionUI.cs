
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CatchDecisionUI : MonoBehaviour
{
    public static CatchDecisionUI Instance;

    public GameObject panel;
    public TMP_Text messageText;
    public Transform creatureButtonsContainer;
    public Button creatureButtonPrefab;
    public Button releaseButton;

    private Action<int> onChoice;

    private void Awake()
    {
        Instance = this;
    }

    public void Show(CreatureInstance caughtCreature, Action<int> callback)
    {
        onChoice = callback;

        messageText.text = $"Your party is full! Choose a creature to release and make room for {caughtCreature.CreatureName}, or release them instead";

        foreach (Transform child in creatureButtonsContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < PartyManager.Instance.party.Count; i++)
        {
            int partyIndex = i;
            CreatureInstance creature = PartyManager.Instance.party[i];

            Button button = Instantiate(
                creatureButtonPrefab,
                creatureButtonsContainer
            );

            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = creature.CreatureName + "\nLv " + creature.level;
            }

            button.onClick.AddListener(() =>
            {
                Choose(partyIndex);
            });
        }

        releaseButton.onClick.RemoveAllListeners();
        TMP_Text releaseButtonText = releaseButton.GetComponentInChildren<TMP_Text>();
        releaseButtonText.text = $"Release {caughtCreature.CreatureName}";
        releaseButton.onClick.AddListener(() =>
        {
            Choose(-1);
        });

        panel.SetActive(true);
    }

    private void Choose(int partyIndex)
    {
        panel.SetActive(false);

        Action<int> callback = onChoice;
        onChoice = null;

        callback?.Invoke(partyIndex);
    }
}
