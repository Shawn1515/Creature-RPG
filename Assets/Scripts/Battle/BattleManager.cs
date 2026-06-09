using TMPro;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public Transform playerTransform;
    public Transform playerCreatureTransform;
    public GameObject enemyHPBarCanvas;

    [Header("UI")]
    public GameObject battlePanel;
    public TextMeshProUGUI creatureNameText;
    public TextMeshProUGUI playerCreatureName;
    public TextMeshProUGUI enemyName;
    public TextMeshProUGUI battleLogText;
    public Slider playerHPBar;
    public Slider enemyHPBar;

    [Header("Cameras")]
    public CinemachineCamera exploreCamera;
    public CinemachineCamera battleCamera;

    private CreatureData playerCreature;
    private CreatureData enemyCreature;

    private int playerCurrentHP;
    private int enemyCurrentHP;
    private Transform enemyTransform;

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

    public void StartBattle(CreatureData creature, Transform enemy)
    {
        enemyHPBarCanvas.SetActive(true);

        playerCreature = PlayerCreatureManager.Instance.starterCreature;

        enemyCreature = creature;
        enemyTransform = enemy;

        enemyTransform.GetComponent<CreatureWander>()?.StopMoving();
        playerCurrentHP = playerCreature.maxHP;
        enemyCurrentHP = enemyCreature.maxHP;

        GameManager.Instance.SetState(
            GameState.Battle
        );

        PositionBattleParticipants();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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
        enemyHPBarCanvas.SetActive(false);
        battleCamera.Priority = 10;
        exploreCamera.Priority = 20;

        GameManager.Instance.SetState(GameState.Exploration);

        battlePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        playerCreatureName.text = $"{playerCreature.creatureName}";
        enemyName.text =  $"Wild {enemyCreature.creatureName}";

        playerHPBar.value = (float)playerCurrentHP / playerCreature.maxHP;
        enemyHPBar.value = (float)enemyCurrentHP / enemyCreature.maxHP;
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

        UpdateHPUI();

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

        UpdateHPUI();
        
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

    Vector3 GetGroundPosition(Vector3 position)
    {
        if (Physics.Raycast(position + Vector3.up * 20f, Vector3.down, out RaycastHit hit, 100f))
        {
            return hit.point;
        }
        return position;
    }

    void PositionBattleParticipants()
    {
        Vector3 enemyPos =
            GetGroundPosition(
                BattlePositions.Instance.enemySpot.position
            );
        Vector3 playerCreaturePos =
            GetGroundPosition(
                BattlePositions.Instance.playerCreatureSpot.position
            );
        Vector3 playerPos =
            GetGroundPosition(
                BattlePositions.Instance.playerSpot.position
            );
        playerTransform.position = playerPos;
        playerCreatureTransform.position = playerCreaturePos;
        enemyTransform.position = enemyPos;
        Vector3 enemyLookPos = enemyTransform.position;
        enemyLookPos.y = playerCreatureTransform.position.y;

        playerCreatureTransform.LookAt(enemyLookPos);

        Vector3 playerLookPos = playerCreatureTransform.position;
        playerLookPos.y = enemyTransform.position.y;

        enemyTransform.LookAt(playerLookPos);
    }
}