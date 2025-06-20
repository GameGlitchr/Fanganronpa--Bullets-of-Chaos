using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EvidenceUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject EvidenceScreen_Portrait;
    public GameObject EvidenceScreen_Landscape;
    public GameObject EvidencePanel_Portrait;
    public GameObject EvidencePanel_Landscape;
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

    private HashSet<int> discoveredEvidenceIDs = new HashSet<int>();

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
        if (discoveredEvidenceIDs.Contains(evidenceID))
            return;

        discoveredEvidenceIDs.Add(evidenceID);
        AddEvidenceToCanvas(contentParentPortrait, evidenceID, evidenceName, evidenceDescription);
        AddEvidenceToCanvas(contentParentLandscape, evidenceID, evidenceName, evidenceDescription);
    }

    private void AddEvidenceToCanvas(Transform parent, int evidenceID, string name, string desc)
    {
        GameObject buttonObj = Instantiate(evidenceButtonPrefab, parent);

        // Set button text
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
            buttonText.text = name;

        // Find actual button inside child named "ButtonImage"
        Button button = buttonObj.transform.Find("ButtonImage")?.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => ShowEvidenceDetails(name, desc));
            button.onClick.AddListener(() => SelectClue(evidenceID));
        }
        else
        {
            Debug.LogWarning("ButtonImage/Button not found on prefab for: " + name);
        }
    }

    private void ShowEvidenceDetails(string name, string desc)
    {
        EnsurePanelActive();

        evidenceDescriptionPanelPortrait.SetActive(true);
        evidenceTitleTextPortrait.text = name;
        evidenceDescriptionTextPortrait.text = desc;

        evidenceDescriptionPanelLandscape.SetActive(true);
        evidenceTitleTextLandscape.text = name;
        evidenceDescriptionTextLandscape.text = desc;
    }

    public void EnsurePanelActive()
    {
        if (EvidenceScreen_Portrait.activeSelf && !EvidencePanel_Portrait.activeSelf)
            EvidencePanel_Portrait.SetActive(true);
        if (EvidenceScreen_Landscape.activeSelf && !EvidencePanel_Landscape.activeSelf)
            EvidencePanel_Landscape.SetActive(true);
    }

    private void SelectClue(int evidenceID)
    {
        if (clueOne == -1)
        {
            clueOne = evidenceID;
        }
        else if (clueTwo == -1 && evidenceID != clueOne)
        {
            clueTwo = evidenceID;
        }
        else
        {
            clueOne = evidenceID;
            clueTwo = -1;
        }
    }

    public void SelectSuspect(int suspectID)
    {
        suspect = suspectID;
    }

    public void DrawConclusion()
    {
        if (clueOne == -1 || clueTwo == -1 || suspect == -1)
            return;

        var combo = new ClueSuspectCombination(clueOne, clueTwo, suspect);
        if (conclusionsDictionary.TryGetValue(combo, out Conclusion conclusion))
        {
            drawnConclusions.Add(conclusion);
        }
    }

    private void InitializeConclusions()
    {
        conclusionsDictionary = new Dictionary<ClueSuspectCombination, Conclusion>
        {
            { new ClueSuspectCombination(4, 0, 10), new Conclusion("The victim's tablet suffered heavy water damage...", 5, 9, 7, 3) },
            { new ClueSuspectCombination(11, 9, 8), new Conclusion("Soshu is a pervert, and was spying on the girls...", 8, 3, 6, 2) },
            { new ClueSuspectCombination(11, 9, 10), new Conclusion("Soshu is a pervert, so he must have killed Taira...", 7, 9, 4, 4) },
            { new ClueSuspectCombination(2, 1, 10), new Conclusion("Taira must have been strangled with the rope...", 6, 10, 8, 5) },
            { new ClueSuspectCombination(2, 14, 10), new Conclusion("Taira must have been strangled with fishing wire...", 4, 7, 9, 3) }
        };
    }

    public List<Conclusion> GetDrawnConclusions() => drawnConclusions;

    public void ResetSelection()
    {
        clueOne = -1;
        clueTwo = -1;
        suspect = -1;
        evidenceDescriptionPanelPortrait.SetActive(false);
        evidenceDescriptionPanelLandscape.SetActive(false);
    }
}
