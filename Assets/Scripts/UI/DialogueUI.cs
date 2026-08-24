using System.Collections;
using TMPro;
using UnityEngine;
using System;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    public GameObject namePanel;
    public TextMeshProUGUI nameText;

    public float typingSpeed = 0.03f;

    public static DialogueUI Instance;

    private bool isTyping = false;

    private string currentMessage;

    private string[] currentDialogue;
    private int dialogueIndex;

    public bool IsOpen => dialoguePanel.activeSelf;
    public bool IsTyping => isTyping;
    private CreatureInstance pendingBattleCreature;
    private Transform pendingEnemyTransform;

    private TrainerEncounter pendingTrainer;

    private Action onFinished;

    public void SetPendingBattle(CreatureInstance creature, Transform enemyTransform)
    {
        pendingBattleCreature = creature;
        pendingEnemyTransform = enemyTransform;
    }

    public void SetPendingTrainer(TrainerEncounter trainer)
    {
        pendingTrainer = trainer;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void SetOnFinished(Action finishedAction)
    {
        onFinished = finishedAction;
    }

    public void StartDialogue(string[] dialogueLines, string speakerName)
    {
        GameManager.Instance.SetState(GameState.Dialogue);
        currentDialogue = dialogueLines;
        dialogueIndex = 0;

        dialoguePanel.SetActive(true);
        if(speakerName != "")
        {
            namePanel.SetActive(true);
            nameText.text = speakerName;
        }

        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        currentMessage = currentDialogue[dialogueIndex];

        StopAllCoroutines();
        StartCoroutine(TypeText(currentMessage));
    }

    IEnumerator TypeText(string message)
    {
        isTyping = true;

        dialogueText.text = "";

        foreach (char letter in message)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void CompleteText()
    {
        StopAllCoroutines();

        dialogueText.text = currentMessage;

        isTyping = false;
    }

    public void NextLine()
    {
        if (isTyping)
        {
            CompleteText();
            return;
        }

        dialogueIndex++;

        if (dialogueIndex >= currentDialogue.Length)
        {
            HideDialogue();
            return;
        }

        ShowCurrentLine();
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);

        if(pendingTrainer != null)
        {
            BattleManager.Instance.StartTrainerBattle(pendingTrainer);
            pendingTrainer = null;
            return;
        }
        if (pendingBattleCreature != null)
        {
            BattleManager.Instance.StartWildBattle(pendingBattleCreature, pendingEnemyTransform);
            pendingBattleCreature = null;
            pendingEnemyTransform = null;
            return;
        }

        Action action = onFinished;

        onFinished = null;

        action?.Invoke();

        GameManager.Instance.SetState(GameState.Exploration);
    }
}