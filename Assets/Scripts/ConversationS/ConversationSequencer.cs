using System.Collections;
using UnityEngine;

public class ConversationSequencer : MonoBehaviour
{
    public Conversation conversation;
    private DialogUIManager dialogUIManager;
    private int currentLineIndex = 0;
    private bool isConversationActive = false;

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
            Character speakingCharacter = FindCharacterByName(currentLine.charName);

            if (speakingCharacter != null)
            {
                dialogUIManager.ShowDialogUI(speakingCharacter, currentLine.dialogLine);
            }
            else
            {
                Debug.LogError($"Character '{currentLine.charName}' not found in scene!");
                dialogUIManager.ShowDialogUI(null, currentLine.dialogLine); // Optional fallback
            }

            currentLineIndex++;
        }
        else
        {
            EndConversation();
        }
    }

    private Character FindCharacterByName(string charName)
    {
        Character[] allCharacters = FindObjectsByType<Character>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Character character in allCharacters)
        {
            if (character.charName == charName)
            {
                return character;
            }
        }

        return null;
    }

    private void EndConversation()
    {
        isConversationActive = false;
        dialogUIManager.HideDialogUI();
        Destroy(gameObject);
    }

    void Update()
    {
        if (isConversationActive && Input.GetKeyDown(KeyCode.E))
        {
            ShowNextLine();
        }
    }
}
