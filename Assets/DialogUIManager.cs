using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class DialogUIManager : MonoBehaviour
{
    public GameObject dialogUI;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI speakerDialog;

    // Start is called before the first frame update
    void Start()
    {
        dialogUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowDialogUI(string charName, string dialogLine)
    {
        dialogUI.SetActive(true);
        speakerName.text = charName;
        speakerDialog.text = dialogLine;
    }

    public void UpdateDialogUI(string dialogLine)
    {
        speakerDialog.text = dialogLine;
    }

    public void HideDialogUI()
    {
        dialogUI.SetActive(false);
    }
}
