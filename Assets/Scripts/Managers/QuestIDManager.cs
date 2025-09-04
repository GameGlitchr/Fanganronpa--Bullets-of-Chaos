using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class QuestStage
{
    public string description;
    public int targetValue; // How many actions to complete this stage (e.g., talk to 3 people)
    public int currentValue; // How much progress has been made
    public bool IsComplete => currentValue >= targetValue;

    public void IncrementProgress()
    {
        currentValue++;
    }

    public void ResetProgress()
    {
        currentValue = 0;
    }
}



[System.Serializable]
public class Quest
{
    public string questName;
    public string questID; // Can be "Main", "Akeno", "Uki", etc.
    public List<QuestStage> stages = new List<QuestStage>();
    public int currentStageIndex = 0;

    public QuestStage CurrentStage => currentStageIndex < stages.Count ? stages[currentStageIndex] : null;
    public bool IsComplete => currentStageIndex >= stages.Count;

    public void AdvanceStage()
    {
        if (!IsComplete)
        {
            currentStageIndex++;
        }
    }

    public void IncrementProgress()
    {
        if (!IsComplete)
        {
            CurrentStage.IncrementProgress();
            if (CurrentStage.IsComplete)
            {
                AdvanceStage();
            }
        }
    }

    public void ResetQuest()
    {
        currentStageIndex = 0;
        foreach (var stage in stages)
        {
            stage.ResetProgress();
        }
    }
}





public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [SerializeField] private List<Quest> quests = new List<Quest>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Quest GetQuestByID(string id)
    {
        return quests.Find(q => q.questID == id);
    }

    public void IncrementQuestProgress(string questID)
    {
        Quest quest = GetQuestByID(questID);
        if (quest != null)
        {
            quest.IncrementProgress();
            Debug.Log($"Progressed quest: {quest.questName}, Stage {quest.currentStageIndex + 1}");
        }
    }

    public void ResetQuest(string questID)
    {
        Quest quest = GetQuestByID(questID);
        if (quest != null)
        {
            quest.ResetQuest();
        }
    }

    public string GetQuestStageString(string questID)
    {
        Quest quest = GetQuestByID(questID);
        if (quest != null)
        {
            return $"Stage {quest.currentStageIndex + 1} / {quest.stages.Count}";
        }
        return "Quest not found";
    }
}


public class QuestIDManager : MonoBehaviour
{
    public static QuestIDManager instance;

    public int QuestID { get; private set; } // Make QuestID read-only outside of this class
    private int npcsInteractedWith; // Counter for NPCs interacted with

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        QuestID = 1; // Start at the first stage
        npcsInteractedWith = 0;
    }

    private void Update()
    {
        // Handle quest-specific logic here if needed
        if (QuestID == 1 && npcsInteractedWith >= 3)
        {
            AdvanceQuestStage();
        }
    }

    // Method to advance to the next quest stage
    public void AdvanceQuestStage()
    {
        if (QuestID < 10)
        {
            QuestID++;
            ResetAllNPCDialogs();
            
            npcsInteractedWith = 0; // Reset the counter for the next stage
            Debug.Log("Quest advanced to stage: " + QuestID.ToString("D2"));
        }
        else
        {
            Debug.Log("Quest is already at the final stage.");
        }
    }

    // Method to reset the quest to the first stage
    public void ResetQuest()
    {
        QuestID = 1;
        npcsInteractedWith = 0;
        Debug.Log("Quest has been reset to stage: " + QuestID.ToString("D2"));
    }

    // Method to get the current quest stage as a formatted string
    public string GetCurrentQuestStage()
    {
        return QuestID.ToString("D2"); // Format as two digits, e.g., "01", "02", etc.
    }

    // Method to track NPC interaction
    public void NPCDialogExhausted()
    {
        npcsInteractedWith++;
        Debug.Log("NPC dialog exhausted. Total NPCs interacted with: " + npcsInteractedWith);
    }

    // Method to reset all NPC dialogs
    private void ResetAllNPCDialogs()
    {
        NPCDialog[] allNPCs = FindObjectsByType<NPCDialog>(FindObjectsSortMode.None);
        foreach (NPCDialog npc in allNPCs)
        {
            npc.ResetDialog();
        }
        Debug.Log("All NPC dialogs have been reset.");
    }
}
