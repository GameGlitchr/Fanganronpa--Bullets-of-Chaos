using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUIManager : MonoBehaviour
{
    [Header("Portrait References")]
    public GameObject portraitPanel;
    public Transform portraitRegistrar;
    public GameObject portraitProfileScreen;
    public TextMeshProUGUI portraitNameAndTalentText;
    public TextMeshProUGUI portraitBioText;
    public Image portraitMoodImage;
    public Image portraitPhoto;
    public TextMeshProUGUI portraitBesties;
    public TextMeshProUGUI portraitEnemies;
    public TextMeshProUGUI portraitHangouts;

    [Header("Landscape References")]
    public GameObject landscapePanel;
    public Transform landscapeRegistrar;
    public GameObject landscapeProfileScreen;
    public TextMeshProUGUI landscapeNameAndTalentText;
    public TextMeshProUGUI landscapeBioText;
    public Image landscapeMoodImage;
    public Image landscapePhoto;
    public TextMeshProUGUI landscapeBesties;
    public TextMeshProUGUI landscapeEnemies;
    public TextMeshProUGUI landscapeHangouts;

    [System.Serializable]
    public struct ExtendedCharacterProfile
    {
        public string profileText;
        public Sprite studentPhoto;
    }

    public Dictionary<int, ExtendedCharacterProfile> extendedProfiles = new();
    private Dictionary<int, Character> charactersInScene = new();

    private void Start()
    {
        Character[] allCharacters = FindObjectsByType<Character>(FindObjectsSortMode.None);
        foreach (Character c in allCharacters)
        {
            if (!charactersInScene.ContainsKey(c.charID))
                charactersInScene.Add(c.charID, c);
        }
    }

   /* public void AssignPortraitButton(int charID)
    {
        if (!CharacterBase.allCharacterProfiles.ContainsKey(charID)) return;

        CharacterProfile profile = CharacterBase.allCharacterProfiles[charID];
        portraitNameAndTalentText.text = $"{profile.name}\n{profile.talent}";

        if (extendedProfiles.TryGetValue(charID, out ExtendedCharacterProfile exProfile))
        {
            portraitBioText.text = exProfile.profileText;
            portraitPhoto.sprite = exProfile.studentPhoto;
        }

        if (charactersInScene.TryGetValue(charID, out Character targetChar))
        {
            portraitMoodImage.sprite = targetChar.GetMoodSprite();
            portraitBesties.text = string.Join(", ", targetChar.GetBesties());
            portraitEnemies.text = string.Join(", ", targetChar.GetEnemies());
            portraitHangouts.text = string.Join(", ", targetChar.GetHangouts());
        }
    }

    public void AssignLandscapeButton(int charID)
    {
        if (!CharacterBase.allCharacterProfiles.ContainsKey(charID)) return;

        CharacterProfile profile = CharacterBase.allCharacterProfiles[charID];
        landscapeNameAndTalentText.text = $"{profile.name}\n{profile.talent}";

        if (extendedProfiles.TryGetValue(charID, out ExtendedCharacterProfile exProfile))
        {
            landscapeBioText.text = exProfile.profileText;
            landscapePhoto.sprite = exProfile.studentPhoto;
        }

        if (charactersInScene.TryGetValue(charID, out Character targetChar))
        {
            landscapeMoodImage.sprite = targetChar.GetMoodSprite();
            landscapeBesties.text = string.Join(", ", targetChar.GetBesties());
            landscapeEnemies.text = string.Join(", ", targetChar.GetEnemies());
            landscapeHangouts.text = string.Join(", ", targetChar.GetHangouts());
        }
    } */
}
