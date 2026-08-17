using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovesetUI : MonoBehaviour
{
    public static MovesetUI Instance;

    public GameObject panel;

    public Transform currentContainer;
    public Transform unlockedContainer;

    public Button moveButtonPrefab;

    public Button closeButton;

    private CreatureInstance creature;

    private MoveData selectedUnlockedMove;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        closeButton.onClick.AddListener(() => {
            panel.SetActive(false);
        });
    }

    public void Open(CreatureInstance selectedCreature)
    {
        creature = selectedCreature;
        panel.SetActive(true);
        Refresh();
    }

    void Refresh()
    {
        BuildCurrentMoves();
        BuildUnlockedMoves();
    }

    void BuildCurrentMoves()
    {
        foreach(Transform child in currentContainer)
        {
            Destroy(child.gameObject);
        }
        for(int i = 0; i < creature.Moves.Count; i++)
        {
            int index = i;
            Button button = Instantiate(moveButtonPrefab, currentContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = creature.Moves[i].moveName;
            button.onClick.AddListener(() =>
            {
                ReplaceMove(index);
            });
        }
    }

    void BuildUnlockedMoves()
    {
        foreach(Transform child in unlockedContainer)
        {
            Destroy(child.gameObject);
        }
        foreach(MoveData move in creature.UnlockedMoves)
        {
            Button button = Instantiate(moveButtonPrefab, unlockedContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = move.moveName;
            button.onClick.AddListener(() =>
            {
                selectedUnlockedMove = move;
            });
        }
    }

    void ReplaceMove(int index)
    {
        if(selectedUnlockedMove == null || creature.Moves.Contains(selectedUnlockedMove))
        {
            return;
        }
        creature.Moves[index] = selectedUnlockedMove;
        selectedUnlockedMove = null;
        Refresh();
    }


}