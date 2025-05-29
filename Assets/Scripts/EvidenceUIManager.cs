using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvidenceUIManager : MonoBehaviour
{
    private Dictionary<ClueSuspectCombination, Conclusion> conclusionsDictionary;
    public List<Conclusion> drawnConclusions;

    public GameObject evidenceUI; // Reference to the UI panel
    public GameObject confirmScreen;
    public GameObject buttonCanvas;
    public List<Button> evidenceButtons; // List of evidence buttons
    public List<Sprite> evidenceSprites; // List to store sprites for evidence
    public List<Sprite> characterSprites; // List to store sprites for characters
    public Image[] evidenceImages; // Array to store references to evidence images
    public Image characterImage; // Reference to the image that will display the selected character
    public Slider characterSlider; // Reference to the slider for selecting a character
    public Button submitButton;
    public Button cancelButton;

    private int clueOne;
    private int clueTwo;
    private int suspect;

    private int currentImageIndex = 0;
    private Player player;

    private bool confirmDialog = false;

    void Start()
    {
        evidenceUI.SetActive(false);
        confirmScreen.SetActive(false);
        player = FindFirstObjectByType<Player>();
        drawnConclusions = new List<Conclusion>();
        InitializeConclusions();

        // Hide all evidence buttons initially
        foreach (var button in evidenceButtons)
        {
            button.gameObject.SetActive(false);
        }

        // Add listener to the slider to handle value changes
        characterSlider.onValueChanged.AddListener(OnCharacterSliderValueChanged);
        characterImage.sprite = characterSprites[0];
        submitButton.onClick.AddListener(DrawConclusion);

        // Add listener to the cancel button to close the evidence menu
        cancelButton.onClick.AddListener(() => player.CloseEvidenceMenu());
    }

    public void ShowEvidenceUI()
    {
        evidenceUI.SetActive(true);
    }

    public void HideEvidenceUI()
    {
        evidenceUI.SetActive(false);
    }

    public void ToggleButtons()
    {
        confirmDialog = !confirmDialog;
        confirmScreen.SetActive(confirmDialog);
        buttonCanvas.SetActive(!confirmDialog);
    }

    public void AddEvidence(int evidenceID, string evidenceName, string evidenceDescription)
    {
        if (evidenceID < 0 || evidenceID >= evidenceButtons.Count)
        {
            Debug.LogError($"Invalid evidence ID for button: {evidenceID}");
            return;
        }

        Button button = evidenceButtons[evidenceID];
        button.gameObject.SetActive(true);
        //button.GetComponentInChildren<Text>().text = evidenceName;

        // Add onClick listener to update images when button is clicked
        button.onClick.AddListener(() => UpdateEvidenceImages(evidenceID));
    }

    private void UpdateEvidenceImages(int evidenceID)
    {
        if (evidenceID < 0 || evidenceID >= evidenceSprites.Count)
        {
            Debug.LogError($"Invalid evidence ID for sprite: {evidenceID}");
            return;
        }

        if (currentImageIndex >= evidenceImages.Length)
        {
            currentImageIndex = 0;
        }

        evidenceImages[currentImageIndex].sprite = evidenceSprites[evidenceID];
        if (currentImageIndex == 0) { clueOne = evidenceID; }
        else if (currentImageIndex == 1) { clueTwo = evidenceID; }
        currentImageIndex++;
    }

    private void OnCharacterSliderValueChanged(float value)
    {
        int characterID = (int)value;
        if (characterID < 0 || characterID >= characterSprites.Count)
        {
            Debug.LogError($"Invalid character ID for sprite: {characterID}");
            return;
        }

        characterImage.sprite = characterSprites[characterID];
        suspect = characterID;
        Debug.Log($"Selected Character ID: {characterID}");
    }

    private void InitializeConclusions()
    {
        conclusionsDictionary = new Dictionary<ClueSuspectCombination, Conclusion>
        {
            { new ClueSuspectCombination(4, 0, 10), new Conclusion("The victim's tablet suffered heavy water damage, and she was found in the lake, therefore she must have been drowned!", 5, 9, 7, 3) },
            { new ClueSuspectCombination(11, 9, 8), new Conclusion("Soshu is a pervert, and was spying on the girls' cabin to steal our underwear when we weren't looking.", 8, 3, 6, 2) },
            { new ClueSuspectCombination(11, 9, 10), new Conclusion("Soshu is a pervert, so he must have killed Taira in a jealous rage after she rejected him!", 7, 9, 4, 4) },
            { new ClueSuspectCombination(2, 1, 10), new Conclusion("Taira must have been strangled with the rope by the docks!", 6, 10, 8, 5) },
            { new ClueSuspectCombination(2, 14, 10), new Conclusion("Taira must have been strangled with fishing wire!", 4, 7, 9, 3) }
        };
    }

    public event System.Action<List<Conclusion>> OnConclusionsUpdated; // Event to notify listeners

    public void DrawConclusion()
    {
        var combination = new ClueSuspectCombination(clueOne, clueTwo, suspect);
        if (conclusionsDictionary.TryGetValue(combination, out Conclusion conclusion))
        {
            drawnConclusions.Add(conclusion);
            Debug.Log($"Conclusion Drawn: {conclusion.Text}");
            OnConclusionsUpdated?.Invoke(drawnConclusions); // Notify listeners about updated conclusions
        }
        else
        {
            Debug.Log("No valid conclusion could be drawn with the given clues and suspect.");
        }
        HideEvidenceUI();
    }

    public List<Conclusion> GetDrawnConclusions()
    {
        return drawnConclusions;
    }
}

public class Conclusion
{
    public string Text { get; }
    public int RelevanceToTopic { get; }
    public int RelevanceToMurder { get; }
    public int Truthfulness { get; }
    public int ArgumentType { get; }

    public Conclusion(string text, int relevanceToTopic, int relevanceToMurder, int truthfulness, int argumentType)
    {
        Text = text;
        RelevanceToTopic = relevanceToTopic;
        RelevanceToMurder = relevanceToMurder;
        Truthfulness = truthfulness;
        ArgumentType = argumentType;
    }
}
