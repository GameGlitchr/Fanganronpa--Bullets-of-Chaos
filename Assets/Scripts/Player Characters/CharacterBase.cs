using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public struct CharacterProfile
{
    public int id;
    public string name;
    public string gender;
    public string talent;
    public string realTalent;
    public int studentID;
    public bool isIntrovert;
    public bool isExtrovert;
}


public class CharacterBase : MonoBehaviour
{

    /// <summary>
    /// this block defines two booleans, isIntrovert and isExtrovert, and ensures that the two are always opposite, such that a character
    /// cannot be both introverted and extroverted at any given time.
    /// </summary>

    private bool _isIntrovert;
    public bool isIntrovert
    {
        get => _isIntrovert;
        set
        {
            _isIntrovert = value;
            _isExtrovert = !value;
        }
    }

    private bool _isExtrovert;
    public bool isExtrovert
    {
        get => _isExtrovert;
        set
        {
            _isExtrovert = value;
            _isIntrovert = !value;
        }
    }


    public static readonly Dictionary<int, CharacterProfile> allCharacterProfiles = new()
{
    { 0, new CharacterProfile { id = 0, name = "Ioko Kabuto", gender = "female", talent = "The Ultimate Huntress", realTalent = "", studentID = 381151906, isIntrovert = false, isExtrovert = false }},
    { 1, new CharacterProfile { id = 1, name = "Tomeo Hatano", gender = "male", talent = "The Ultimate Lawyer", realTalent = "The Ultimate Liar", studentID = 381151904, isIntrovert = true, isExtrovert = false }},
    { 2, new CharacterProfile { id = 2, name = "Akeno Sakai", gender = "male", talent = "The Ultimate Novelist", realTalent = "The Ultimate Chaos", studentID = 381151911, isIntrovert = true, isExtrovert = false }},
    { 3, new CharacterProfile { id = 3, name = "Genjo Takenaka", gender = "male", talent = "The Ultimate Eidetiker", realTalent = "", studentID = 381151913, isIntrovert = true, isExtrovert = false }},
    { 4, new CharacterProfile { id = 4, name = "Dayu Enatsu", gender = "male", talent = "The Ultimate Actor", realTalent = "The Ultimate Despair", studentID = 381151901, isIntrovert = false, isExtrovert = true }},
    { 5, new CharacterProfile { id = 5, name = "Risako Kitani", gender = "female", talent = "The Ultimate Thief", realTalent = "", studentID = 381151908, isIntrovert = false, isExtrovert = true }},
    { 6, new CharacterProfile { id = 6, name = "Masaki Tamura", gender = "male", talent = "The Ultimate ???", realTalent = "The Ultimate Spy", studentID = 381151914, isIntrovert = true, isExtrovert = false }},
    { 7, new CharacterProfile { id = 7, name = "Akira Kawashima", gender = "male", talent = "The Ultimate ???", realTalent = "The Ultimate Secret Agent", studentID = 381151907, isIntrovert = false, isExtrovert = true }},
    { 8, new CharacterProfile { id = 8, name = "Soshu Nura", gender = "male", talent = "The Ultimate Simp", realTalent = "The Ultimate Politician", studentID = 381151909, isIntrovert = false, isExtrovert = true }},
    { 9, new CharacterProfile { id = 9, name = "Miki Sugiyama", gender = "female", talent = "The Ultimate Sailor", realTalent = "", studentID = 381151912, isIntrovert = false, isExtrovert = true }},
    { 10, new CharacterProfile { id = 10, name = "Taira Hatake", gender = "female", talent = "The Ultimate Aviator", realTalent = "", studentID = 381151903, isIntrovert = false, isExtrovert = true }},
    { 11, new CharacterProfile { id = 11, name = "Han Saeki", gender = "female", talent = "The Ultimate Archer", realTalent = "", studentID = 381151910, isIntrovert = true, isExtrovert = false }},
    { 12, new CharacterProfile { id = 12, name = "Masago Haga", gender = "female", talent = "The Ultimate F1 Racer", realTalent = "", studentID = 381151902, isIntrovert = true, isExtrovert = false }},
    { 13, new CharacterProfile { id = 13, name = "Uki Umezaki", gender = "female", talent = "The Ultimate Journalist", realTalent = "", studentID = 381151916, isIntrovert = false, isExtrovert = true }},
    { 14, new CharacterProfile { id = 14, name = "Umeka Tanifuji", gender = "female", talent = "The Ultimate Demolitionist", realTalent = "", studentID = 381151915, isIntrovert = true, isExtrovert = false }},
    { 15, new CharacterProfile { id = 15, name = "Tetsuo Ishimoto", gender = "male", talent = "The Ultimate Pirate", realTalent = "", studentID = 381151905, isIntrovert = false, isExtrovert = true }},
};





    public int charID;
    public int studentID_Number;
    public string charName;
    public string charTalent;
    public string charRealTalent;
    public int gender = -1;
    public int male = 0;
    public int female = 1;
    public string sex;

    public float emotionalIntelligence = 75;
    public float logicalIntelligence = 25;
    public float trustTowardPlayer = 20;
    public float Despair = 5;
    public float Hope = 5;
    public float Confidence = 15;

    public float Mood = 500; 

    public bool guiltyConscience;


    protected virtual void Start()
    {
        GetIdentification();
        ApplyCharacterOverrides();
        UpdateMood();
    }

    protected void UpdateMood()
    {
        Mood = Mathf.Clamp(100 - Despair, 0, 1000);
    }

    protected void ControlMinMax()
    {
        emotionalIntelligence = Mathf.Clamp(emotionalIntelligence, 10, 90);
        logicalIntelligence = Mathf.Clamp(logicalIntelligence, 10, 90);
        trustTowardPlayer = Mathf.Clamp(trustTowardPlayer, 0, 100);
        Despair = Mathf.Clamp(Despair, 5, 100);
        Hope = Mathf.Clamp(Hope, 0, 100);
        Confidence = Mathf.Clamp(Confidence, 5, 100);
    }

    public void GetIdentification()
    {
        if (CharacterBase.allCharacterProfiles.TryGetValue(charID, out CharacterProfile profile))
        {
            charName = profile.name;
            sex = profile.gender;
            gender = profile.gender == "female" ? female : male;
            charTalent = profile.talent;
            charRealTalent = profile.realTalent;
            studentID_Number = profile.studentID;
            isIntrovert = profile.isIntrovert;
            isExtrovert = profile.isExtrovert;
        }
        else
        {
            Debug.LogWarning($"No profile found for charID {charID}");
        }
    }


    public List<int> inseparableBesties = new();
    public List<int> swornEnemies = new();

    protected virtual void ApplyCharacterOverrides()
    {
        switch (charID)
        {
            case 2: // Akeno Sakai
            case 4: // Dayu Enatsu
                Despair = 100;
                break;

            case 3: // Genjo Takenaka
                    // No field needed, just don’t decay his memories later
                break;

            case 13: // Uki Umezaki
                inseparableBesties.Add(14); // Umeka
                break;

            case 14: // Umeka Tanifuji
                inseparableBesties.Add(13); // Uki
                break;

            case 6: // Masaki Tamura
                swornEnemies.Add(7); // Akira
                break;

            case 7: // Akira Kawashima
                swornEnemies.Add(6); // Masaki
                break;
        }
    }



}
