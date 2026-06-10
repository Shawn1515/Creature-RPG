using TMPro;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public Transform playerCreatureTransform;
    public GameObject enemyHPBarCanvas;
    public GameObject playerHPBarCanvas;

    public enum BattlePhase
    {
        ChoosingMove,
        PlayerAttackMessage,
        PlayerDamage,
        EnemyAttackMessage,
        EnemyDamage
    }

    [Header("Battle Buttons")]
    public Button[] moveButtons;
    public Button runButton;

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

    private BattlePhase phase;

    private MoveData selectedMove;
    private MoveData enemyMove;
    private bool playerGoesFirst;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Battle)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            AdvanceBattle();
        }
    }

    public void StartBattle(CreatureData creature, Transform enemy)
    {
        enemyHPBarCanvas.SetActive(true);
        playerHPBarCanvas.SetActive(true);

        playerCreature = PlayerCreatureManager.Instance.starterCreature;

        enemyCreature = creature;
        enemyTransform = enemy;

        enemyTransform.GetComponent<CreatureWander>()?.StopMoving();

        playerCurrentHP = playerCreature.maxHP;
        enemyCurrentHP = enemyCreature.maxHP;

        GameManager.Instance.SetState(GameState.Battle);

        PositionBattleParticipants();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        exploreCamera.Priority = 10;
        battleCamera.Priority = 20;

        battlePanel.SetActive(true);

        creatureNameText.text = playerCreature.creatureName;
        playerCreatureName.text = enemyCreature.creatureName;

        UpdateHPUI();

        SetupMoveButtons();

        phase = BattlePhase.ChoosingMove;

        battleLogText.text = "Choose a move!";

        SetMoveButtonsActive(true);
    }

    public void EndBattle()
    {
        enemyHPBarCanvas.SetActive(false);
        playerHPBarCanvas.SetActive(false);

        battleCamera.Priority = 10;
        exploreCamera.Priority = 20;

        GameManager.Instance.SetState(GameState.Exploration);

        battlePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        enemyTransform.GetComponent<CreatureWander>()?.StartMoving();
    }

    void SelectMove(MoveData move)
    {
        if (phase != BattlePhase.ChoosingMove)
            return;

        selectedMove = move;
        enemyMove = enemyCreature.moves[0];

        playerGoesFirst =
            playerCreature.speed >= enemyCreature.speed;

        SetMoveButtonsActive(false);

        if (playerGoesFirst)
        {
            phase = BattlePhase.PlayerAttackMessage;

            battleLogText.text =
                $"{playerCreature.creatureName} used {move.moveName}!";
        }
        else
        {
            phase = BattlePhase.EnemyAttackMessage;

            battleLogText.text =
                $"Wild {enemyCreature.creatureName} used {enemyMove.moveName}!";
        }
    }

    void AdvanceBattle()
    {
        switch (phase)
        {
            case BattlePhase.PlayerAttackMessage:
                DoPlayerDamage();
                break;

            case BattlePhase.PlayerDamage:
                StartEnemyTurn();
                break;

            case BattlePhase.EnemyAttackMessage:
                DoEnemyDamage();
                break;

            case BattlePhase.EnemyDamage:
                StartPlayerTurn();
                break;
        }
    }

    void DoPlayerDamage()
    {
        int damage = Mathf.Max(
            1,
            playerCreature.attack +
            selectedMove.power -
            enemyCreature.defense
        );

        enemyCurrentHP -= damage;

        if (enemyCurrentHP < 0)
            enemyCurrentHP = 0;

        UpdateHPUI();

        if (enemyCurrentHP <= 0)
        {
            battleLogText.text = $"Wild {enemyCreature.creatureName} fainted!";
            Invoke(nameof(EndBattle), 1.5f);
            return;
        }

        battleLogText.text =
            $"Wild {enemyCreature.creatureName} took {damage} damage!";

        phase = BattlePhase.PlayerDamage;
    }

    void StartEnemyTurn()
    {
        phase = BattlePhase.EnemyAttackMessage;

        battleLogText.text =
            $"Wild {enemyCreature.creatureName} used {enemyMove.moveName}!";
    }

    void DoEnemyDamage()
    {
        int damage = Mathf.Max(
            1,
            enemyCreature.attack +
            enemyMove.power -
            playerCreature.defense
        );

        playerCurrentHP -= damage;

        if (playerCurrentHP < 0)
            playerCurrentHP = 0;

        UpdateHPUI();

        if (playerCurrentHP <= 0)
        {
            battleLogText.text =
                $"{playerCreature.creatureName} fainted!";

            Invoke(nameof(EndBattle), 1.5f);
            return;
        }

        battleLogText.text =
            $"{playerCreature.creatureName} took {damage} damage!";

        phase = BattlePhase.EnemyDamage;
    }

    void StartPlayerTurn()
    {
        phase = BattlePhase.ChoosingMove;

        battleLogText.text = "Choose a move!";

        SetMoveButtonsActive(true);
    }

    void SetMoveButtonsActive(bool active)
    {
        foreach (Button button in moveButtons)
        {
            button.interactable = active;
        }

        runButton.interactable = active;
    }

    void UpdateHPUI()
    {
        playerCreatureName.text =
            playerCreature.creatureName;

        enemyName.text =
            "Wild " + enemyCreature.creatureName;

        playerHPBar.value =
            (float)playerCurrentHP / playerCreature.maxHP;

        enemyHPBar.value =
            (float)enemyCurrentHP / enemyCreature.maxHP;
    }

    void SetupMoveButtons()
    {
        for (int i = 0; i < moveButtons.Length; i++)
        {
            moveButtons[i].onClick.RemoveAllListeners();

            if (i < playerCreature.moves.Length)
            {
                MoveData move = playerCreature.moves[i];

                moveButtons[i].gameObject.SetActive(true);

                moveButtons[i]
                    .GetComponentInChildren<TextMeshProUGUI>()
                    .text = move.moveName;

                moveButtons[i].onClick.AddListener(() =>
                {
                    SelectMove(move);
                });
            }
            else
            {
                moveButtons[i].gameObject.SetActive(false);
            }
        }

        runButton.onClick.RemoveAllListeners();

        runButton.onClick.AddListener(() =>
        {
            RunAway();
        });
    }

    void RunAway()
    {
        battleLogText.text = "Got away safely!";
        Invoke(nameof(EndBattle), 1f);
    }

    Vector3 GetGroundPosition(Vector3 position)
    {
        if (Physics.Raycast(
            position + Vector3.up * 20f,
            Vector3.down,
            out RaycastHit hit,
            100f))
        {
            return hit.point + Vector3.up * 1.1f;
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

        playerCreatureTransform.position = playerCreaturePos;
        enemyTransform.position = enemyPos;

        FaceHorizontally(playerCreatureTransform, enemyTransform);
        FaceHorizontally(enemyTransform, playerCreatureTransform);
    }

    void FaceHorizontally(Transform a, Transform b)
    {
        Vector3 direction = b.position - a.position;

        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            a.rotation = Quaternion.LookRotation(direction);
        }
    }
}