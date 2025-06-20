using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogUIManager : MonoBehaviour
{
    public GameObject dialogUI;
    public TextMeshProUGUI speakerName;
    public int speakerEmotion;
    public TextMeshProUGUI speakerDialog;

    public Button insultButton;
    public Button complimentButton;
    public Button giveGiftButton;
    public Button lieButton;

    private Character currentCharacter;

    void Start()
    {
        dialogUI.SetActive(false);

        insultButton.onClick.AddListener(Insult);
        complimentButton.onClick.AddListener(Compliment);
        giveGiftButton.onClick.AddListener(GiveGift);
        lieButton.onClick.AddListener(Lie);
    }

    public void ShowDialogUI(Character character, string dialogLine)
    {
        dialogUI.SetActive(true);
        currentCharacter = character;
        speakerName.text = character.charName;
        speakerDialog.text = dialogLine;
    }

    public void UpdateDialogUI(string dialogLine)
    {
        speakerDialog.text = dialogLine;
    }

    public void HideDialogUI()
    {
        dialogUI.SetActive(false);
        currentCharacter = null;
    }

    // Player interaction methods
    public void Insult()
    {
        if (currentCharacter != null)
        {
            currentCharacter.ChangeOpinion(0, -10f);
            currentCharacter.Mood -= 10f;
            UpdateDialogUI("How rude!");
        }
    }

    public void Compliment()
    {
        if (currentCharacter != null)
        {
            currentCharacter.ChangeOpinion(0, 10f);
            currentCharacter.Mood += 10f;
            UpdateDialogUI("Oh, thank you!");
        }
    }

    public void GiveGift()
    {
        if (currentCharacter != null)
        {
            bool positive = Random.value > 0.5f;

            if (positive)
            {
                currentCharacter.ChangeOpinion(0, 20f);
                currentCharacter.Mood += 20f;
                UpdateDialogUI("Wow, thanks for the gift!");
            }
            else
            {
                currentCharacter.ChangeOpinion(0, -20f);
                currentCharacter.Mood -= 20f;
                UpdateDialogUI("Why would I want this?");
            }
        }
    }

    public void Lie()
    {
        if (currentCharacter != null)
        {
            // Placeholder for special "lie" functionality, opens chaos menu
            UpdateDialogUI("Hmm, I'm not sure I believe you...");
            // Implement chaos menu logic here
        }
    }
}
