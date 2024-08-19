using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 5f; // Maximum distance for interaction
    public LayerMask interactableLayer; // Layer for interactable objects
    public Image crosshair; // Reference to the crosshair UI element

    private DialogUIManager dialogUIManager;
    private NPCDialog currentNPC;
    public bool dialogMenuOpen;

    public Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
        dialogUIManager = FindObjectOfType<DialogUIManager>();
    }

    void Update()
    {
        CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            // Change the crosshair color to indicate interactable object
            crosshair.color = Color.red;

            if (Input.GetKeyDown(KeyCode.E))
            {
                Evidence evidence = hit.collider.GetComponent<Evidence>();
                NPCDialog npcDialog = hit.collider.GetComponent<NPCDialog>();

                if (evidence != null)
                {
                    evidence.Interact();
                }
                else if (npcDialog != null)
                {
                    HandleNPCDialog(npcDialog);
                }
            }
        }
        else
        {
            // Revert the crosshair color to default
            crosshair.color = Color.black;
        }
    }

    private void HandleNPCDialog(NPCDialog npcDialog)
    {
        if (dialogMenuOpen)
        {
            if (currentNPC.isDialogExhausted)
            {
                dialogUIManager.HideDialogUI();
                dialogMenuOpen = false;
                player.ToggleCursorAndMouseSensitivity(dialogMenuOpen);
                player.moveSpeed = 5f; // Restore player movement
            }
            else
            {
                string dialogLine = currentNPC.GetNextDialogLine();
                dialogUIManager.UpdateDialogUI(dialogLine);

                if (currentNPC.isDialogExhausted)
                {
                    // Keep the dialog open to show the last line
                    dialogUIManager.UpdateDialogUI(dialogLine);
                }
            }
        }
        else
        {
            dialogMenuOpen = true;
            player.ToggleCursorAndMouseSensitivity(dialogMenuOpen);
            player.moveSpeed = 0; // Stop player movement
            currentNPC = npcDialog;
            string dialogLine = currentNPC.GetNextDialogLine();
            Character charAI = npcDialog.GetComponent<Character>();
            if (charAI != null)
            {
                charAI.bored = false; // Assuming you have logic for character behavior
            }
            dialogUIManager.ShowDialogUI(charAI.charName, dialogLine);
        }
    }
}
