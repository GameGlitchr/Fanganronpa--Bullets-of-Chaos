using System.Collections.Generic;
using UnityEngine;

public class NPCDialog : MonoBehaviour
{
 
    [System.Serializable]
    public class DialogForQuestStage
    {
        public int questStage;
        public List<string> dialogLines;
    }

    public List<DialogForQuestStage> dialogForQuestStages;
    private Dictionary<int, List<string>> dialogLinesByQuestStage;
    private int currentLineIndex = 0;
    public bool isDialogExhausted;

    void Start()
    {
        dialogLinesByQuestStage = new Dictionary<int, List<string>>();
        foreach (var dialogForQuest in dialogForQuestStages)
        {
            dialogLinesByQuestStage[dialogForQuest.questStage] = dialogForQuest.dialogLines;
        }
    }

    public string GetNextDialogLine()
    {
        int questStage = QuestIDManager.instance.QuestID;

        if (!dialogLinesByQuestStage.ContainsKey(questStage))
        {
            Debug.LogError($"No dialog available for quest stage: {questStage}");
            return "No dialog available for this quest stage.";
        }

        List<string> dialogLines = dialogLinesByQuestStage[questStage];

        if (dialogLines == null || dialogLines.Count == 0)
        {
            Debug.LogError($"Dialog lines for quest stage: {questStage} are null or empty.");
            return "No dialog available for this quest stage.";
        }

        if (currentLineIndex < dialogLines.Count - 1)
        {
            string line = dialogLines[currentLineIndex];
            currentLineIndex++;
            return line;
        }
        else if (currentLineIndex == dialogLines.Count - 1)
        {
            // Notify the quest manager the first time the dialog is exhausted
            if (!isDialogExhausted)
            {
                isDialogExhausted = true;
                QuestIDManager.instance.NPCDialogExhausted(); // Notify Quest Manager
            }
            return dialogLines[currentLineIndex];
        }
        else
        {
            return dialogLines[dialogLines.Count - 1]; // Repeat the last line
        }
    }

    public void ResetDialog()
    {
        currentLineIndex = 0;
        isDialogExhausted = false;
    }
}
