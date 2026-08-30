using TMPro;
using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.UI;
using System.Collections.Generic;

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

    public GameObject namePanel;
    public TextMeshProUGUI nameText;

    public Transform playerTransform;

    private CreatureInstance enemyCreature;
    public CreatureInstance playerCreature;

    public GameObject playerObject;

    private Transform enemyTransform;

    private MoveData selectedMove;
    private MoveData enemyMove;
    private bool playerFirst;
    private CreatureBattleUI playerUI;
    private CreatureBattleUI enemyUI;
    private Transform playerCreatureTransform;
    private bool enemyFreeTurn;
    private bool run;
    private bool forcedSwitch;

    private List<CreatureInstance> enemyParty;
    private int enemyPartyIndex;
    private bool trainerBattle;
    private TrainerEncounter currentTrainer;
    private GameObject currentEnemyCreatureObject;

    private void Awake()
    {
        Instance = this;
    }

    private void StartBattle(CreatureInstance creature, Transform enemy)
    {
        Vector3 forward = exploreCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();
        BattlePositions.Instance.battleCenter.rotation = Quaternion.LookRotation(forward);
        BattlePositions.Instance.battleCameraTarget.rotation = Quaternion.LookRotation(forward) * Quaternion.Euler(20f, 40f, 0f);
        enemyFreeTurn = false;
        forcedSwitch = false;
        run = false;

        playerCreature = PartyManager.Instance.GetActiveCreature();
        playerCreatureTransform = FollowerManager.Instance.currentFollower.transform;

        enemyCreature = creature;
        enemyTransform = enemy;

        enemyTransform.GetComponent<CreatureWander>()?.StopMoving();

        GameManager.Instance.SetState(GameState.Battle);

        playerUI = playerCreatureTransform.GetComponent<CreatureBattleUI>();
        enemyUI = enemyTransform.GetComponent<CreatureBattleUI>();

        playerUI.Setup(playerCreature);
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

    public void StartWildBattle(CreatureInstance creature, Transform enemy)
    {
        trainerBattle = false;
        currentTrainer = null;
        enemyParty = null;
        enemyPartyIndex = 0;
        StartBattle(creature, enemy);
    }

    public void StartTrainerBattle(TrainerEncounter trainer)
    {
        trainerBattle = true;

        currentTrainer = trainer;

        enemyParty = trainer.GetParty();

        enemyPartyIndex = 0;

        SendOutTrainerCreature();
    }

    void SendOutTrainerCreature()
    {
        if(currentEnemyCreatureObject != null)
        {
            Destroy(currentEnemyCreatureObject);
        }

        enemyCreature = enemyParty[enemyPartyIndex];

        currentEnemyCreatureObject = Instantiate(enemyCreature.WildPrefab, BattlePositions.Instance.enemySpot.position, Quaternion.identity);

        StartBattle(enemyCreature, currentEnemyCreatureObject.transform);
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

        if(trainerBattle)
        {
            Destroy(currentEnemyCreatureObject);
            namePanel.SetActive(false);
        }
        else if(!run)
        {
            DestroyEnemy();
        }
        else
        {
            enemyTransform.GetComponent<CreatureWander>()?.StartMoving();
        }

        trainerBattle = false;
        currentTrainer = null;
        enemyParty = null;
        enemyPartyIndex = 0;
    }

    void DestroyEnemy()
    {
        CreatureEncounter encounter = enemyTransform.GetComponent<CreatureEncounter>();
        encounter?.spawnPoint.CreatureDefeated();
        Destroy(enemyTransform.gameObject);
    }


    void UpdateHPUI()
    {
        StartCoroutine(AnimateHPBar(playerUI.hpSlider, (float)playerCreature.currentHP / playerCreature.MaxHP));
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


            if (i < playerCreature.Moves.Count)
            {
                MoveData move = playerCreature.Moves[i];


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
            SetMoveButtonsActive(false);
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
        Debug.Log("Move Selected!");
        selectedMove = move;
        enemyMove = enemyCreature.Moves[Random.Range(0, enemyCreature.Moves.Count)];
        SetMoveButtonsActive(false);

        if(playerCreature.Speed > enemyCreature.Speed || (playerCreature.Speed == enemyCreature.Speed && Random.Range(0f, 1.0f) > 0.5f))
        {
            playerFirst = true;
            BattleDialogueUI.Instance.ShowMessage(
                $"{playerCreature.CreatureName} used {move.moveName}!",
                PlayerAttack
            );
        }
        else
        {
            playerFirst = false;
            if(trainerBattle)
            {
                BattleDialogueUI.Instance.ShowMessage(
                $"{currentTrainer.trainer.trainerName}'s {enemyCreature.CreatureName} used {enemyMove.moveName}!",
                EnemyAttack
            );
            }
            else{
            BattleDialogueUI.Instance.ShowMessage(
                $"Wild {enemyCreature.CreatureName} used {enemyMove.moveName}!",
                EnemyAttack
            );
            }
        }
    }

    void RunAway()
    {
        if(trainerBattle)
        {
            BattleDialogueUI.Instance.ShowMessage(
                "You can't do that right now!",
                EnemyFreeAttack
            );
            return;
        }
        run = true;
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

        playerCreature = creature;

        PartyManager.Instance.SetLeader(partyIndex);

        playerCreatureTransform = FollowerManager.Instance.currentFollower.transform;
        FaceHorizontally(
            playerCreatureTransform,
            enemyTransform
        );

        playerUI = playerCreatureTransform.GetComponent<CreatureBattleUI>();
        playerUI.Setup(playerCreature);
        playerUI.Show();

        UpdateHPUI();
        SetupMoveButtons();
        SetMoveButtonsActive(false);

        PartyUI.Instance.CloseParty();

        if (forcedSwitch)
        {
            forcedSwitch = false;
            BattleDialogueUI.Instance.ShowMessage(
                $"Go {creature.CreatureName}!",
                () =>
                {
                    SetupMoveButtons();
                    SetMoveButtonsActive(true);
                }
            );
        }
        else
        {
            BattleDialogueUI.Instance.ShowMessage(
                $"Go {creature.CreatureName}!",
                EnemyFreeAttack
            );
        }
    }

    void OpenSwitchMenu()
    {
        SetMoveButtonsActive(false);
        GameManager.Instance.SetState(GameState.BattleParty);
        PartyUI.Instance.OpenForBattle();
    }
    void PlayerAttack()
    {
        Debug.Log("Player Attack!");
        int mult = 0;
        float multiplier = TypeChart.GetMultiplier(selectedMove.moveType, enemyCreature.species.primaryType);
        if(multiplier > 1)
        {
            mult = 1;
        }
        if(multiplier < 1)
        {
            mult = -1;
        }
        if(selectedMove.moveType == playerCreature.species.primaryType)
        {
            multiplier *= 1.5f;
        }
        int damage = Mathf.Max(
            1,
            Mathf.RoundToInt(multiplier * (playerCreature.Attack +
            selectedMove.power -
            enemyCreature.Defense))
        );

        enemyCreature.currentHP -= damage;

        if (enemyCreature.currentHP < 0)
        {
            enemyCreature.currentHP = 0;
        }

        StartCoroutine(AttackAnimation(playerCreatureTransform, enemyTransform));

        UpdateHPUI();

        if(mult == 1)
        {
            BattleDialogueUI.Instance.ShowMessage(
                "It's super effective!",
                PlayerEffect
            );
            return;
        }

        if(mult == -1)
        {
            BattleDialogueUI.Instance.ShowMessage(
                "It's not very effective...",
                PlayerEffect
            );
            return;
        }

        if (enemyCreature.currentHP <= 0)
        {
            if(trainerBattle)
            {
                BattleDialogueUI.Instance.ShowMessage(
                    $"{currentTrainer.trainer.trainerName}'s {enemyCreature.CreatureName} fainted!",
                    GiveExperience
                );
            }
            else {
                BattleDialogueUI.Instance.ShowMessage(
                    $"Wild {enemyCreature.CreatureName} fainted!",
                    GiveExperience
                );
            }

            return;
        }

        if(playerFirst) {
            if(trainerBattle)
            {
                BattleDialogueUI.Instance.ShowMessage(
                    $"{currentTrainer.trainer.trainerName}'s {enemyCreature.CreatureName} used {enemyMove.moveName}!",
                    EnemyAttack
                );
            }
            else
            {
                BattleDialogueUI.Instance.ShowMessage(
                    $"Wild {enemyCreature.CreatureName} used {enemyMove.moveName}!",
                    EnemyAttack
                );
            }
        }
        else
        {
            SetMoveButtonsActive(true);
        }
    }
    void TrainerWon()
    {
        currentTrainer.MarkDefeated();

        DialogueUI.Instance.SetOnFinished(EndBattle);

        DialogueUI.Instance.StartDialogue(
            currentTrainer.trainer.defeatDialogue, currentTrainer.trainer.trainerName
        );
    }

    void PlayerEffect()
    {
        if (enemyCreature.currentHP <= 0)
        {
            if(trainerBattle)
            {
                BattleDialogueUI.Instance.ShowMessage(
                $"{currentTrainer.trainer.trainerName}'s {enemyCreature.CreatureName} fainted!",
                GiveExperience
            );
            }
            else {
            BattleDialogueUI.Instance.ShowMessage(
                $"Wild {enemyCreature.CreatureName} fainted!",
                GiveExperience
            );
            }

            return;
        }

        if(playerFirst) {
            if(trainerBattle)
            {
                BattleDialogueUI.Instance.ShowMessage(
                $"{currentTrainer.trainer.trainerName}'s {enemyCreature.CreatureName} used {enemyMove.moveName}!",
                EnemyAttack
            );
            }
            else{
            BattleDialogueUI.Instance.ShowMessage(
                $"Wild {enemyCreature.CreatureName} used {enemyMove.moveName}!",
                EnemyAttack
            );
            }
        }
        else
        {
            SetMoveButtonsActive(true);
        }
    }

    void GiveExperience()
    {
        int xp = enemyCreature.species.experienceReward * enemyCreature.level;

        bool leveledUp = playerCreature.GainExperience(xp);

        if(leveledUp) {
            BattleDialogueUI.Instance.ShowMessage(
                $"{playerCreature.CreatureName} gained {xp} XP!",
                LevelUpText
            );
        }
        else
        {
            BattleDialogueUI.Instance.ShowMessage(
                $"{playerCreature.CreatureName} gained {xp} XP!",
                ContinueBattle
            );
        }
    }

    void LevelUpText()
    {
        BattleDialogueUI.Instance.ShowMessage($"{playerCreature.CreatureName} grew to level {playerCreature.level}!",
            PossibleMoveUnlock);
    }

    void PossibleMoveUnlock()
    {
        if(playerCreature.NewlyUnlockedMove != null) {
            BattleDialogueUI.Instance.ShowMessage($"{playerCreature.CreatureName} can now learn a new move!",
                ContinueBattle);
        }
        else
        {
            ContinueBattle();
        }
    }

    void ContinueBattle()
    {
        if (!trainerBattle)
        {
            EndBattle();
            return;
        }

        enemyPartyIndex++;

        if(enemyPartyIndex >= enemyParty.Count)
        {
            TrainerWon();
            return;
        }

        BattleDialogueUI.Instance.ShowMessage(
            $"{currentTrainer.trainer.trainerName} sent out {enemyParty[enemyPartyIndex].CreatureName}!",
            SendOutTrainerCreature
        );
    }

    public void EnemyFreeAttack()
    {
        enemyFreeTurn = true;
        enemyMove = enemyCreature.Moves[Random.Range(0, enemyCreature.Moves.Count)];

        if(trainerBattle)
        {
            BattleDialogueUI.Instance.ShowMessage($"{currentTrainer.trainer.trainerName}'s {enemyCreature.CreatureName} used {enemyMove.moveName}!", EnemyAttack);
        }
        else {
            BattleDialogueUI.Instance.ShowMessage($"Wild {enemyCreature.CreatureName} used {enemyMove.moveName}!", EnemyAttack);
        }
    }


    void EnemyAttack()
    {
        int mult = 0;
        float multiplier = TypeChart.GetMultiplier(enemyMove.moveType, playerCreature.species.primaryType);
        if(multiplier > 1)
        {
            mult = 1;
        }
        if(multiplier < 1)
        {
            mult = -1;
        }
        if(enemyMove.moveType == enemyCreature.species.primaryType)
        {
            multiplier *= 1.5f;
        }
        int damage = Mathf.Max(
            1,
            Mathf.RoundToInt(multiplier * (enemyCreature.Attack +
            enemyMove.power -
            playerCreature.Defense
        )));
        playerCreature.currentHP -= damage;

        if (playerCreature.currentHP <= 0)
        {
            playerCreature.currentHP = 0;
        }

        StartCoroutine(AttackAnimation(enemyTransform, playerCreatureTransform));

        UpdateHPUI();

        if(mult == 1)
        {
            BattleDialogueUI.Instance.ShowMessage(
                "It's super effective!",
                EnemyEffect
            );
            return;
        }

        if(mult == -1)
        {
            BattleDialogueUI.Instance.ShowMessage(
                "It's not very effective...",
                EnemyEffect
            );
            return;
        }

        if (playerCreature.currentHP <= 0)
        {
            enemyFreeTurn = false;
            BattleDialogueUI.Instance.ShowMessage(
                $"{playerCreature.CreatureName} fainted!",
                OnPlayerCreatureFainted
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
                $"{playerCreature.CreatureName} used {selectedMove.moveName}!",
                PlayerAttack
            );
        }
        else
        {
            SetMoveButtonsActive(true);
        }
    }

    void EnemyEffect()
    {
        if (playerCreature.currentHP <= 0)
        {
            enemyFreeTurn = false;
            BattleDialogueUI.Instance.ShowMessage(
                $"{playerCreature.CreatureName} fainted!",
                OnPlayerCreatureFainted
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
                $"{playerCreature.CreatureName} used {selectedMove.moveName}!",
                PlayerAttack
            );
        }
        else
        {
            SetMoveButtonsActive(true);
        }
    }

    void OnPlayerCreatureFainted()
    {
        if(!PartyManager.Instance.HasUsableCreature())
        {
            EndBattle();
            return;
        }
        forcedSwitch = true;
        PartyUI.Instance.OpenForBattle();
    }

    public void SetMoveButtonsActive(bool active)
    {
        foreach (Button button in moveButtons)
        {
            button.gameObject.SetActive(active);
        }

        runButton.gameObject.SetActive(active);
        catchButton.gameObject.SetActive(active);
        switchButton.gameObject.SetActive(active);
    }

    void TryCatch()
    {
        SetMoveButtonsActive(false);

        float hpPercent = (float)enemyCreature.currentHP / enemyCreature.MaxHP;
        float catchChance = 1f - hpPercent;
        float roll = Random.Range(0f, 1f);
        PlayerCatch playerCatch = playerObject.GetComponent<PlayerCatch>();

        if(trainerBattle)
        {
            BattleDialogueUI.Instance.ShowMessage(
                "You feel terrible by the thought of stealing "
                + currentTrainer.trainer.trainerName + "\'s " + 
                enemyCreature.CreatureName + ".",
                EnemyFreeAttack
            );
        }
        playerCatch.StartCatch(enemyTransform, roll < catchChance);
    }

    public IEnumerator CatchAfterAnimation(bool caught)
    {
        if (caught)
        {
            yield return new WaitForSeconds(1f);
            BattleDialogueUI.Instance.ShowMessage(
                "Gotcha! " +
                enemyCreature.CreatureName +
                " was caught!",
                CatchSuccess
            );
        }
        else
        {
            yield return new WaitForSeconds(1f);
            enemyTransform.gameObject.SetActive(true);
            PlayerCatch.Instance.DestroyHat();
            BattleDialogueUI.Instance.ShowMessage(
                enemyCreature.CreatureName +
                " broke free!",
                EnemyFreeAttack
            );
        }
    }

    public void CatchSuccess()
    {
        if(PartyManager.Instance.AddCreature(enemyCreature))
        {
            DestroyEnemy();
            EndBattle();
        }
        else
        {
            //party is full message
        }
    }


    Vector3 GetGroundPosition(Vector3 position, CreatureInstance creature)
    {
        if (Physics.Raycast(
            position + Vector3.up * 50f,
            Vector3.down,
            out RaycastHit hit,
            200f, LayerMask.GetMask("Ground")))
        {
            return hit.point + Vector3.up * creature.species.groundOffset;
        }
        Debug.LogWarning("Ground raycast missed at " + position);
        return position;
    }

    Vector3 GetGroundPosition(Vector3 position, TrainerEncounter trainer)
    {
        if (Physics.Raycast(
            position + Vector3.up * 50f,
            Vector3.down,
            out RaycastHit hit,
            200f, LayerMask.GetMask("Ground")))
        {
            return hit.point + Vector3.up * trainer.trainer.groundOffset;
        }
        Debug.LogWarning("Ground raycast missed at " + position);
        return position;
    }

    void PositionBattleParticipants()
    {
        Vector3 forward = exploreCamera.transform.forward;
        Vector3 enemyPos =
            GetGroundPosition(
                BattlePositions.Instance.enemySpot.position,
                enemyCreature
            );

        Vector3 playerCreaturePos =
            GetGroundPosition(
                BattlePositions.Instance.playerCreatureSpot.position,
                playerCreature
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

        FaceHorizontally(
            playerTransform,
            playerCreatureTransform
        );

        if(trainerBattle)
        {
            currentTrainer.trainerTransform.position = GetGroundPosition(
                BattlePositions.Instance.enemyTrainerSpot.position, currentTrainer
            );
            FaceHorizontally(currentTrainer.trainerTransform,
            enemyTransform);
        }
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

    public bool IsForcedSwitch => forcedSwitch;
}