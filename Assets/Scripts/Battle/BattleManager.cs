using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour {
    public static BattleManager Instance;
    public GameObject battlePanel;
    public TextMeshProUGUI creatureNameText;

    private void Awake() {
        Instance = this;
    }

    public void StartBattle(CreatureData creature) {
        Debug.Log("Called");
        battlePanel.SetActive(true);
        creatureNameText.text = "Wild " + creature.creatureName;
    }
    public void EndBattle() {
        battlePanel.SetActive(false);
    }
}