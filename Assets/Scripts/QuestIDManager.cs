using UnityEngine;

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
