using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArgumentUIManager : MonoBehaviour
{
    public GameObject argumentUI; // Reference to the UI panel
    public Button submitButton;

    public EvidenceUIManager evidenceUIManager;

    public TextMeshProUGUI previousConclusion;
    public TextMeshProUGUI selectedConclusion;
    public TextMeshProUGUI nextConclusion;

    public Conclusion chosenConclusion;

    private Player player; // Reference to the player script
    private List<Conclusion> drawnConclusions;
    private int currentIndex = 0;

    void Start()
    {
        argumentUI.SetActive(false);
        player = FindFirstObjectByType<Player>();
        submitButton.onClick.AddListener(OnSubmitArgument);
        evidenceUIManager = FindFirstObjectByType<EvidenceUIManager>();
        drawnConclusions = new List<Conclusion>();

        if (evidenceUIManager != null)
        {
            evidenceUIManager.OnConclusionsUpdated += UpdateDrawnConclusions;
            drawnConclusions = evidenceUIManager.GetDrawnConclusions();
        }

        UpdateConclusionUI();
    }

    void UpdateDrawnConclusions(List<Conclusion> conclusions)
    {
        drawnConclusions = conclusions;
        UpdateConclusionUI();
    }

    void Update()
    {
        // Check for mouse scroll input
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (drawnConclusions != null)
        {
            if (scroll > 0f)
            {
                // Scroll up
                currentIndex = (currentIndex > 0) ? currentIndex - 1 : drawnConclusions.Count - 1;
                UpdateConclusionUI();
            }
            else if (scroll < 0f)
            {
                // Scroll down
                currentIndex = (currentIndex < drawnConclusions.Count - 1) ? currentIndex + 1 : 0;
                UpdateConclusionUI();
            }
        }


        //I have no idea why, but without this little bit of redundant code right there the argument screen just doesn't fucking open the first time the player presses F. With it it works. IDK.
        if (player.argueMenuOpen)
        {
            ShowArgumentUI();
        }
        else if (!player.argueMenuOpen)
        {
            HideArgumentUI();
        }

        DamageCalc();

    }

    public void ShowArgumentUI()
    {
        argumentUI.SetActive(true);
        // Ensure conclusions are updated when the UI is shown
        UpdateConclusionUI();
    }

    public void HideArgumentUI()
    {
        argumentUI.SetActive(false);
    }

    private void UpdateConclusionUI()
    {
        // Debugging: Check if drawnConclusions is null or empty
        if (drawnConclusions == null || drawnConclusions.Count == 0)
        {
            //Debug.LogWarning("No conclusions have been drawn yet.");
            if (previousConclusion != null) previousConclusion.text = "";
            if (selectedConclusion != null) selectedConclusion.text = "No conclusions drawn.";
            if (nextConclusion != null) nextConclusion.text = "";
            return;
        }

        // Get the indices for previous, current, and next conclusions
        int previousIndex = (currentIndex > 0) ? currentIndex - 1 : drawnConclusions.Count - 1;
        int nextIndex = (currentIndex < drawnConclusions.Count - 1) ? currentIndex + 1 : 0;

        // Debugging: Log the current indices
        Debug.Log($"Previous index: {previousIndex}, Current index: {currentIndex}, Next index: {nextIndex}");

        // Update the text elements
        if (previousConclusion != null) previousConclusion.text = drawnConclusions[previousIndex].Text;
        if (selectedConclusion != null) selectedConclusion.text = drawnConclusions[currentIndex].Text;
        if (nextConclusion != null) nextConclusion.text = drawnConclusions[nextIndex].Text;

        chosenConclusion = drawnConclusions[currentIndex];
    }

    private void DamageCalc()
    {

    }

    private void OnSubmitArgument()
    {
        float tRelevance;
        float mRelevance;
        float truth;
        float type;
        float damage;

        if(chosenConclusion != null)
        {
            tRelevance = chosenConclusion.RelevanceToTopic;
            mRelevance = chosenConclusion.RelevanceToMurder;
            truth = chosenConclusion.Truthfulness;
            type = chosenConclusion.ArgumentType;
            damage = (tRelevance + mRelevance) / 2f;

            // Call the MakeArgument method on the player
            player.MakeArgument(damage, truth, type);
            player.argueMenuOpen = false;

            // Hide the UI after submitting the argument
            HideArgumentUI();
        }
        else
        {
            Debug.LogWarning("No Conclusions have been drawn, cannot make an argument");
            player.argueMenuOpen = false;
            HideArgumentUI();
        }



        
    }
}
