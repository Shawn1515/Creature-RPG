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

    [Header("Battle Buttons")]
    public Button[] moveButtons;
    public Button runButton;

    [Header("UI")]
    public GameObject battlePanel;
    public TextMeshProUGUI playerCreatureName;
    public TextMeshProUGUI enemyName;
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

    private MoveData selectedMove;
    private MoveData enemyMove;

    private void Awake()
    {
        Instance = this;
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

        enemyName.text =
            "Wild " + enemyCreature.creatureName;
        playerCreatureName.text =
            playerCreature.creatureName;

        UpdateHPUI();
        SetupMoveButtons();
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


    void UpdateHPUI()
    {
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


    void SelectMove(MoveData move)
    {
        selectedMove = move;
        enemyMove = enemyCreature.moves[0];
        SetMoveButtonsActive(false);

        BattleDialogueUI.Instance.ShowMessage(
            $"{playerCreature.creatureName} used {move.moveName}!",
            PlayerAttack
        );
    }


    void RunAway()
    {
        BattleDialogueUI.Instance.ShowMessage(
            "Got away safely!",
            EndBattle
        );
    }
    void PlayerAttack()
    {
        int damage = Mathf.Max(
            1,
            playerCreature.attack +
            selectedMove.power -
            enemyCreature.defense
        );

        enemyCurrentHP -= damage;

        if (enemyCurrentHP < 0)
        {
            enemyCurrentHP = 0;
        }

        UpdateHPUI();

        if (enemyCurrentHP <= 0)
        {
            BattleDialogueUI.Instance.ShowMessage(
                $"Wild {enemyCreature.creatureName} fainted!",
                EndBattle
            );

            return;
        }

        BattleDialogueUI.Instance.ShowMessage(
            $"Wild {enemyCreature.creatureName} used {enemyMove.moveName}!",
            EnemyAttack
        );
    }


    void EnemyAttack()
    {
        int damage = Mathf.Max(
            1,
            enemyCreature.attack +
            enemyMove.power -
            playerCreature.defense
        );
        playerCurrentHP -= damage;

        if (playerCurrentHP < 0)
        {
            playerCurrentHP = 0;
        }

        UpdateHPUI();

        if (playerCurrentHP <= 0)
        {
            BattleDialogueUI.Instance.ShowMessage(
                $"{playerCreature.creatureName} fainted!",
                EndBattle
            );

            return;
        }
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


        enemyTransform.position = enemyPos;
        playerCreatureTransform.position = playerCreaturePos;

        FaceHorizontally(
            playerCreatureTransform,
            enemyTransform
        );

        FaceHorizontally(
            enemyTransform,
            playerCreatureTransform
        );
    }

    void FaceHorizontally(
        Transform first,
        Transform second
    )
    {
        Vector3 direction =
            second.position - first.position;

        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            first.rotation =
                Quaternion.LookRotation(direction);
        }
    }
}