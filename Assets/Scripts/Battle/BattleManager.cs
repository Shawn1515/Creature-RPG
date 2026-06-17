using TMPro;
using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;


    [Header("Battle Buttons")]
    public Button[] moveButtons;
    public Button runButton;
    public Button catchButton;
    public Button switchButton;

    [Header("UI")]
    public GameObject battlePanel;

    [Header("Cameras")]
    public CinemachineCamera exploreCamera;
    public CinemachineCamera battleCamera;

    private CreatureInstance enemyCreature;

    private Transform enemyTransform;

    private MoveData selectedMove;
    private MoveData enemyMove;
    private bool playerFirst;
    private CreatureBattleUI playerUI;
    private CreatureBattleUI enemyUI;
    private Transform playerCreatureTransform;
    private bool enemyFreeTurn;

    private void Awake()
    {
        Instance = this;
    }

    public void StartBattle(CreatureInstance creature, Transform enemy)
    {
        playerCreatureTransform = FollowerManager.Instance.currentFollower.transform;

        enemyCreature = creature;
        enemyTransform = enemy;

        enemyTransform.GetComponent<CreatureWander>()?.StopMoving();

        GameManager.Instance.SetState(GameState.Battle);

        playerUI = playerCreatureTransform.GetComponent<CreatureBattleUI>();
        enemyUI = enemyTransform.GetComponent<CreatureBattleUI>();

        playerUI.Setup(PartyManager.Instance.GetActiveCreature());
        enemyUI.Setup(enemyCreature);

        playerUI.Show();
        enemyUI.Show();

        PositionBattleParticipants();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        exploreCamera.Priority = 10;
        battleCamera.Priority = 20;
        battlePanel.SetActive(true);

        UpdateHPUI();
        SetupMoveButtons();
        SetMoveButtonsActive(true);
    }


    public void EndBattle()
    {
        playerUI.Hide();
        enemyUI.Hide();

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
        StartCoroutine(AnimateHPBar(playerUI.hpSlider, (float)PartyManager.Instance.GetActiveCreature().currentHP / PartyManager.Instance.GetActiveCreature().MaxHP));
        StartCoroutine(AnimateHPBar(enemyUI.hpSlider, (float)enemyCreature.currentHP / enemyCreature.MaxHP));
    }

    IEnumerator AnimateHPBar(Slider bar, float targetValue)
    {
        float startValue = bar.value;
        float timer = 0f;
        float duration = 0.5f;
        while(timer < duration)
        {
            timer += Time.deltaTime;

            bar.value = Mathf.Lerp(
                startValue,
                targetValue,
                timer / duration
            );

            yield return null;
        }
        bar.value = targetValue;
    }


    void SetupMoveButtons()
    {
        for (int i = 0; i < moveButtons.Length; i++)
        {
            moveButtons[i].onClick.RemoveAllListeners();


            if (i < PartyManager.Instance.GetActiveCreature().Moves.Length)
            {
                MoveData move = PartyManager.Instance.GetActiveCreature().Moves[i];


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
        catchButton.onClick.RemoveAllListeners();
        catchButton.onClick.AddListener(() =>
        {
            TryCatch();
        });
        switchButton.onClick.RemoveAllListeners();
        switchButton.onClick.AddListener(() =>
        {
            OpenSwitchMenu();
        });
    }


    void SelectMove(MoveData move)
    {
        selectedMove = move;
        enemyMove = enemyCreature.Moves[Random.Range(0, enemyCreature.Moves.Length)];
        SetMoveButtonsActive(false);

        if(PartyManager.Instance.GetActiveCreature().Speed > enemyCreature.Speed || (PartyManager.Instance.GetActiveCreature().Speed == enemyCreature.Speed && Random.Range(0f, 1.0f) > 0.5f))
        {
            playerFirst = true;
            BattleDialogueUI.Instance.ShowMessage(
                $"{PartyManager.Instance.GetActiveCreature().CreatureName} used {move.moveName}!",
                PlayerAttack
            );
        }
        else
        {
            playerFirst = false;
            BattleDialogueUI.Instance.ShowMessage(
                $"Wild {enemyCreature.CreatureName} used {enemyMove.moveName}!",
                EnemyAttack
            );
        }
    }


    void RunAway()
    {
        BattleDialogueUI.Instance.ShowMessage(
            "Got away safely!",
            EndBattle
        );
    }

    public void SwitchCreature(int partyIndex)
    {
        if (partyIndex <= 0)
        {
            return;
        }

        CreatureInstance creature = PartyManager.Instance.party[partyIndex];

        if (creature.currentHP <= 0)
        {
            return;
        }

        PartyManager.Instance.SetLeader(partyIndex);

        playerCreatureTransform = FollowerManager.Instance.currentFollower.transform;

        playerUI = playerCreatureTransform.GetComponent<CreatureBattleUI>();
        playerUI.Setup(PartyManager.Instance.GetActiveCreature());
        playerUI.Show();

        SetupMoveButtons();
        UpdateHPUI();

        PartyUI.Instance.CloseParty();

        BattleDialogueUI.Instance.ShowMessage(
            $"Go {creature.CreatureName}!",
            EnemyFreeAttack
        );
    }

    void OpenSwitchMenu()
    {
        SetMoveButtonsActive(false);
        GameManager.Instance.SetState(GameState.BattleParty);
        PartyUI.Instance.OpenForBattle();
    }
    void PlayerAttack()
    {
        int damage = Mathf.Max(
            1,
            PartyManager.Instance.GetActiveCreature().Attack +
            selectedMove.power -
            enemyCreature.Defense
        );

        enemyCreature.currentHP -= damage;

        if (enemyCreature.currentHP < 0)
        {
            enemyCreature.currentHP = 0;
        }

        StartCoroutine(AttackAnimation(playerCreatureTransform, enemyTransform));

        UpdateHPUI();

        if (enemyCreature.currentHP <= 0)
        {
            BattleDialogueUI.Instance.ShowMessage(
                $"Wild {enemyCreature.CreatureName} fainted!",
                GiveExperience
            );

            return;
        }

        if(playerFirst) {
            BattleDialogueUI.Instance.ShowMessage(
                $"Wild {enemyCreature.CreatureName} used {enemyMove.moveName}!",
                EnemyAttack
            );
        }
        else
        {
            SetMoveButtonsActive(true);
        }
    }

    void GiveExperience()
    {
        int xp = enemyCreature.species.experienceReward;
        PartyManager.Instance.GetActiveCreature().GainExperience(xp);
        BattleDialogueUI.Instance.ShowMessage(
            $"{PartyManager.Instance.GetActiveCreature().CreatureName} gained {xp} XP!",
            EndBattle
        );
    }

    void EnemyFreeAttack()
    {
        enemyFreeTurn = true;
        enemyMove = enemyCreature.Moves[Random.Range(0, enemyCreature.Moves.Length)];

        BattleDialogueUI.Instance.ShowMessage($"Wild {enemyCreature.CreatureName} used {enemyMove.moveName}!", EnemyAttack);
    }


    void EnemyAttack()
    {
        int damage = Mathf.Max(
            1,
            enemyCreature.Attack +
            enemyMove.power -
            PartyManager.Instance.GetActiveCreature().Defense
        );
        PartyManager.Instance.GetActiveCreature().currentHP -= damage;

        if (PartyManager.Instance.GetActiveCreature().currentHP < 0)
        {
            PartyManager.Instance.GetActiveCreature().currentHP = 0;
        }

        StartCoroutine(AttackAnimation(enemyTransform, playerCreatureTransform));

        UpdateHPUI();

        if (PartyManager.Instance.GetActiveCreature().currentHP <= 0)
        {
            BattleDialogueUI.Instance.ShowMessage(
                $"{PartyManager.Instance.GetActiveCreature().CreatureName} fainted!",
                EndBattle
            );

            return;
        }
        if (enemyFreeTurn)
        {
            enemyFreeTurn = false;
            SetMoveButtonsActive(true);
        }
        else if (!playerFirst)
        {
            BattleDialogueUI.Instance.ShowMessage(
                $"{PartyManager.Instance.GetActiveCreature().CreatureName} used {selectedMove.moveName}!",
                PlayerAttack
            );
        }
        else
        {
            SetMoveButtonsActive(true);
        }
    }

    public void SetMoveButtonsActive(bool active)
    {
        foreach (Button button in moveButtons)
        {
            button.interactable = active;
        }

        runButton.interactable = active;
        catchButton.interactable = active;
        switchButton.interactable = active;
    }

    void TryCatch()
    {
        SetMoveButtonsActive(false);

        float hpPercent = (float)enemyCreature.currentHP / enemyCreature.MaxHP;
        float catchChance = 1f - hpPercent;
        float roll = Random.Range(0f, 1f);

        if (roll < catchChance)
        {
            BattleDialogueUI.Instance.ShowMessage(
                "Gotcha! " +
                enemyCreature.CreatureName +
                " was caught!",
                CatchSuccess
            );
        }
        else
        {
            BattleDialogueUI.Instance.ShowMessage(
                enemyCreature.CreatureName +
                " broke free!",
                EnemyFreeAttack
            );
        }
    }

    void CatchSuccess()
    {
        if(PartyManager.Instance.AddCreature(enemyCreature))
        {
            Destroy(enemyTransform.gameObject);
            EndBattle();
        }
        else
        {
            //party is full message
        }
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
    IEnumerator AttackAnimation(Transform attacker, Transform target)
    {
        Vector3 startPosition = attacker.position;
        Vector3 direction =(target.position - attacker.position).normalized;
        Vector3 attackPosition = target.position - direction * 8f;

        float timer = 0f;
        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            attacker.position = Vector3.Lerp(
                startPosition,
                attackPosition,
                timer / 0.2f
            );

            yield return null;
        }

        timer = 0f;
        while (timer < 0.2f)
        {
            timer += Time.deltaTime;

            attacker.position = Vector3.Lerp(
                attackPosition,
                startPosition,
                timer / 0.2f
            );

            yield return null;
        }
        attacker.position = startPosition;
    }
}