using TMPro;
using UnityEngine;
using Unity.Cinemachine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [Header("UI")]
    public GameObject battlePanel;
    public TextMeshProUGUI creatureNameText;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;
    public TextMeshProUGUI battleLogText;

    [Header("Cameras")]
    public CinemachineCamera exploreCamera;
    public CinemachineCamera battleCamera;

    private CreatureData playerCreature;
    private CreatureData enemyCreature;

    private int playerCurrentHP;
    private int enemyCurrentHP;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Battle)
            return;

        for (int i = 0; i < playerCreature.moves.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                BattleRound(playerCreature.moves[i]);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RunAway();
        }
    }

    public void StartBattle(CreatureData creature)
    {
        playerCreature = PlayerCreatureManager.Instance.starterCreature;

        enemyCreature = creature;

        playerCurrentHP = playerCreature.maxHP;
        enemyCurrentHP = enemyCreature.maxHP;

        GameManager.Instance.SetState(GameState.Battle);

        exploreCamera.Priority = 10;
        battleCamera.Priority = 20;

        battlePanel.SetActive(true);

        creatureNameText.text =
            playerCreature.creatureName +
            " vs Wild " +
            enemyCreature.creatureName;

        UpdateHPUI();
        UpdateMoveText();
    }

    public void EndBattle()
    {
        battleCamera.Priority = 10;
        exploreCamera.Priority = 20;

        GameManager.Instance.SetState(GameState.Exploration);

        battlePanel.SetActive(false);
    }

    void UpdateMoveText()
    {
        string text = "";
        for (int i = 0; i < playerCreature.moves.Length; i++)
        {
            text += $"{i + 1}: {playerCreature.moves[i].moveName}\n";
        }
        battleLogText.text = text;
    }

    void UpdateHPUI()
    {
        playerHPText.text =
            $"{playerCreature.creatureName} HP: {playerCurrentHP}/{playerCreature.maxHP}";

        enemyHPText.text =
            $"Wild {enemyCreature.creatureName} HP: {enemyCurrentHP}/{enemyCreature.maxHP}";
    }

    void BattleRound(MoveData playerMove)
    {
        if (playerCreature.speed >= enemyCreature.speed)
        {
            PlayerAttack(playerMove);

            if (enemyCurrentHP <= 0) {
                return;
            }

            EnemyAttack();
        }
        else
        {
            EnemyAttack();

            if (playerCurrentHP <= 0) {
                return;
            }

            PlayerAttack(playerMove);
        }

        UpdateHPUI();
    }

    void PlayerAttack(MoveData move)
    {
        int damage = Mathf.Max(1, playerCreature.attack + move.power - enemyCreature.defense);

        enemyCurrentHP -= damage;

        if (enemyCurrentHP < 0) {
            enemyCurrentHP = 0;
        }

        battleLogText.text = $"{playerCreature.creatureName} used {move.moveName}!";

        if (enemyCurrentHP <= 0)
        {
            WinBattle();
        }
    }

    void EnemyAttack()
    {
        MoveData move = enemyCreature.moves[0];

        int damage = Mathf.Max(1, enemyCreature.attack + move.power - playerCreature.defense);

        playerCurrentHP -= damage;

        if (playerCurrentHP < 0) {
            playerCurrentHP = 0;
        }
        
        battleLogText.text += $"\nWild {enemyCreature.creatureName} used {move.moveName}!";

        if (playerCurrentHP <= 0)
        {
            LoseBattle();
        }
    }

    void WinBattle()
    {
        Debug.Log("Victory!");
        EndBattle();
    }

    void LoseBattle()
    {
        Debug.Log("Defeat!");
        EndBattle();
    }

    void RunAway()
    {
        Debug.Log("Ran Away!");
        EndBattle();
    }
}