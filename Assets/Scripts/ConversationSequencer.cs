using System.Collections;
using UnityEngine;

public class ConversationSequencer : MonoBehaviour
{
    public Conversation conversation; // The conversation data
    private DialogUIManager dialogUIManager; // Reference to the DialogUIManager
    private int currentLineIndex = 0; // Index of the current line in the conversation
    private bool isConversationActive = false; // Flag to check if the conversation is active

    void Start()
    {
        dialogUIManager = FindFirstObjectByType<DialogUIManager>();
    }

    public void StartConversation()
    {
        if (conversation == null || conversation.conversationLines.Count == 0)
        {
            Debug.LogError("Conversation data is missing or empty.");
            return;
        }

        isConversationActive = true;
        currentLineIndex = 0;
        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (currentLineIndex < conversation.conversationLines.Count)
        {
            ConversationLine currentLine = conversation.conversationLines[currentLineIndex];
            dialogUIManager.ShowDialogUI(currentLine.charName, currentLine.dialogLine);
            currentLineIndex++;
        }
        else
        {
            EndConversation();
        }
    }

    private void EndConversation()
    {
        isConversationActive = false;
        dialogUIManager.HideDialogUI();
        GameObject.Destroy(gameObject);
    }

    void Update()
    {
        if (isConversationActive && Input.GetKeyDown(KeyCode.E))
        {
            ShowNextLine();
        }
    }
}
