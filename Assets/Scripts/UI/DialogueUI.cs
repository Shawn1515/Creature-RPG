using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    public float typingSpeed = 0.03f;

    public static DialogueUI Instance;

    private bool isTyping = false;

    private string currentMessage;

    private string[] currentDialogue;
    private int dialogueIndex;

    public bool IsOpen => dialoguePanel.activeSelf;
    public bool IsTyping => isTyping;
    private CreatureData pendingBattleCreature;

    public void SetPendingBattle(CreatureData creature)
    {
        pendingBattleCreature = creature;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void StartDialogue(string[] dialogueLines)
    {
        currentDialogue = dialogueLines;
        dialogueIndex = 0;

        dialoguePanel.SetActive(true);

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

        if (pendingBattleCreature != null) {
            BattleManager.Instance.StartBattle(pendingBattleCreature);
            pendingBattleCreature = null;
        }
    }
}