using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BattleDialogueUI : MonoBehaviour
{
    public static BattleDialogueUI Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    public float typingSpeed = 0.03f;

    private bool isTyping;
    private string currentMessage;

    private Action onFinished;

    public bool IsOpen => dialoguePanel.activeSelf;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!IsOpen)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                CompleteText();
            }
            else
            {
                CloseDialogue();
            }
        }
    }

    public void ShowMessage(string message, Action finishedAction)
    {
        onFinished = finishedAction;

        dialoguePanel.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(TypeText(message));
    }

    IEnumerator TypeText(string message)
    {
        isTyping = true;

        currentMessage = message;
        dialogueText.text = "";

        foreach (char letter in message)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void CompleteText()
    {
        StopAllCoroutines();

        dialogueText.text = currentMessage;

        isTyping = false;
    }

    void CloseDialogue()
    {
        dialoguePanel.SetActive(false);

        Action action = onFinished;

        onFinished = null;

        action?.Invoke();
    }
}