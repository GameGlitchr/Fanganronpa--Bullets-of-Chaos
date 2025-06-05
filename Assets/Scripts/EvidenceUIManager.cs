// === EvidenceUIManager.cs (REWRITTEN) ===
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EvidenceUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject EvidenceScreen_Portrait;
    public GameObject EvidenceScreen_Landscape;
    public Transform contentParentPortrait;
    public Transform contentParentLandscape;
    public Image evidenceImagePortrait;
    public Image evidenceImageLandscape;
    public GameObject evidenceButtonPrefab;
    public GameObject evidenceDescriptionPanelPortrait;
    public GameObject evidenceDescriptionPanelLandscape;
    public TextMeshProUGUI evidenceTitleTextPortrait;
    public TextMeshProUGUI evidenceTitleTextLandscape;
    public TextMeshProUGUI evidenceDescriptionTextPortrait;
    public TextMeshProUGUI evidenceDescriptionTextLandscape;

    [Header("Sprites (Optional)")]
    public List<Sprite> evidenceSprites;

    private Dictionary<ClueSuspectCombination, Conclusion> conclusionsDictionary;
    public List<Conclusion> drawnConclusions = new List<Conclusion>();
    private int clueOne = -1;
    private int clueTwo = -1;
    private int suspect = -1;

    private void Start()
    {
        InitializeConclusions();
        evidenceDescriptionPanelPortrait.SetActive(false);
        evidenceDescriptionPanelLandscape.SetActive(false);
    }

    public void AddEvidence(int evidenceID, string evidenceName, string evidenceDescription)
    {
        AddEvidenceToCanvas(contentParentPortrait, evidenceID, evidenceName, evidenceDescription);
        AddEvidenceToCanvas(contentParentLandscape, evidenceID, evidenceName, evidenceDescription);
    }

    private void AddEvidenceToCanvas(Transform parent, int evidenceID, string name, string desc)
    {
        Debug.Log("adding evidence: " + name + " to the canvas");
        GameObject buttonObj = Instantiate(evidenceButtonPrefab, parent);
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
            buttonText.text = name;

        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => ShowEvidenceDetails(name, desc));
            button.onClick.AddListener(() => SelectClue(evidenceID));
        }
    }

    private void ShowEvidenceDetails(string name, string desc)
    {
        evidenceDescriptionPanelPortrait.SetActive(true);
        evidenceTitleTextPortrait.text = name;
        evidenceDescriptionTextPortrait.text = desc;
        evidenceDescriptionPanelLandscape.SetActive(true);
        evidenceTitleTextLandscape.text = name;
        evidenceDescriptionTextLandscape.text = desc;
    }

    private void SelectClue(int evidenceID)
    {
        if (clueOne == -1)
        {
            clueOne = evidenceID;
            Debug.Log($"Clue One set to ID: {clueOne}");
        }
        else if (clueTwo == -1 && evidenceID != clueOne)
        {
            clueTwo = evidenceID;
            Debug.Log($"Clue Two set to ID: {clueTwo}");
        }
        else
        {
            Debug.LogWarning("Both clues already set. Resetting clues.");
            clueOne = evidenceID;
            clueTwo = -1;
        }
    }

    public void SelectSuspect(int suspectID)
    {
        suspect = suspectID;
        Debug.Log($"Suspect selected: {suspect}");
    }

    public void DrawConclusion()
    {
        if (clueOne == -1 || clueTwo == -1 || suspect == -1)
        {
            Debug.LogWarning("Missing a clue or suspect.");
            return;
        }

        var combo = new ClueSuspectCombination(clueOne, clueTwo, suspect);
        if (conclusionsDictionary.TryGetValue(combo, out Conclusion conclusion))
        {
            drawnConclusions.Add(conclusion);
            Debug.Log("Conclusion Drawn: " + conclusion.Text);
        }
        else
        {
            Debug.Log("No valid conclusion found for the selected combination.");
        }
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

    public List<Conclusion> GetDrawnConclusions()
    {
        return drawnConclusions;
    }

    public void ResetSelection()
    {
        clueOne = -1;
        clueTwo = -1;
        suspect = -1;
        evidenceDescriptionPanelPortrait.SetActive(false);
        evidenceDescriptionPanelLandscape.SetActive(false);
    }
}
