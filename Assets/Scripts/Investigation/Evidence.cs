using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evidence : MonoBehaviour, IInteractable
{
    public int EvidenceID;
    public string EvidenceName;
    public string EvidenceDescription;
    public float relevanceToMurder;
    public float relevanceToTopic;

    private EvidenceUIManager evidenceUIManager;

    private void Start()
    {
        evidenceUIManager = FindFirstObjectByType<EvidenceUIManager>();
        if (evidenceUIManager == null)
        {
            Debug.LogError("EvidenceUIManager not found in the scene!");
        }
        FindID();
    }

    private void FindID()
    {
        if (EvidenceID == 0)
        {
            EvidenceName = "Monokuma File #1";
            EvidenceDescription = "The victim is Taira Hatake, the Ultimate Aviator. Time of death is estimated between 9:50pm and 1:30am. Cause of death is loss of oxygen due to strangulation or drowning";
        }
        else if (EvidenceID == 1)
        {
            EvidenceName = "Rope tied to Dock";
            EvidenceDescription = "There is a rope tied to the end of the docks.";
        }
        else if (EvidenceID == 2)
        {
            EvidenceName = "Red Line";
            EvidenceDescription = "There is a thin red line on the victim's neck.";
        }
        else if (EvidenceID == 3)
        {
            EvidenceName = "Wet Paper";
            EvidenceDescription = "Found in the victim's pockets. Reads 'I think I found something. Meet me at the flagpole after nighttime.'";
        }
        else if (EvidenceID == 4)
        {
            EvidenceName = "Victim's Tablet";
            EvidenceDescription = "Found on the victim. It doesnt turn on.";
        }
        else if (EvidenceID == 5)
        {
            EvidenceName = "Missing Hat";
            EvidenceDescription = "The victim was found without her signature aviator cap.";
        }
        else if (EvidenceID == 6)
        {
            EvidenceName = "Missing Kayak";
            EvidenceDescription = "The rack normally holds 6 canoes and 4 kayaks. 3 of the kayaks are on the shore, the ones used to retireve the body from the lake. The 4th kayak is gone completely.";
        }
        else if (EvidenceID == 7)
        {
            EvidenceName = "Victim's Belongings";
            EvidenceDescription = "Amongst which was found a faded noted with the only printed words being visible are 'fishing pole' 'tackle' and 'due', as well as a handwritten 'R' at the bottom.";
        }
        else if (EvidenceID == 8)
        {
            EvidenceName = "Genjo's Account";
            EvidenceDescription = "Soshu left the boys' cabin at 8:32pm the night of the murder. Akeno left at 10:37pm. He cannot account for either of them returning to the cabin.";
        }
        else if (EvidenceID == 9)
        {
            EvidenceName = "Ioko's Underwear";
            EvidenceDescription = "Found under Soshu's pillow. When the hell did he get these?!";
        }
        else if (EvidenceID == 10)
        {
            EvidenceName = "Rock?";
            EvidenceDescription = "Found under Akeno's pillow... for some reason.";
        }
        else if (EvidenceID == 11)
        {
            EvidenceName = "Soshu's Account";
            EvidenceDescription = "He refused to say where he was last night, but he did admit to seeing Taira leaving the girl's cabin.";
        }
        else if (EvidenceID == 12)
        {
            EvidenceName = "Footprints";
            EvidenceDescription = "Found around the flagpole. Were these made last night or this morning?";
        }
        else if (EvidenceID == 13)
        {
            EvidenceName = "Broken crank handle";
            EvidenceDescription = "The crank to raise or lower the flag has broken off, and is laying in the dirt.";
        }
        else if (EvidenceID == 14)
        {
            EvidenceName = "Monokuma's Account";
            EvidenceDescription = "Apparently, someone rented out fishing equipment, but hasnt returned them yet.";
        }
        else if (EvidenceID == 15)
        {
            EvidenceName = "Akeno's Account";
            EvidenceDescription = "He claims he was talking with Monokuma last night. He also saw Soshu outside the girls' cabin.";
        }
    }

    public void Interact()
    {
        // Interaction logic for the object
        Debug.Log("Interacted with object: " + EvidenceName);
        if (evidenceUIManager != null)
        {
            evidenceUIManager.AddEvidence(EvidenceID, EvidenceName, EvidenceDescription);
            gameObject.SetActive(false); // Optionally, deactivate the evidence object after interaction
        }
        else
        {
            Debug.LogError("EvidenceUIManager is null in Evidence.Interact");
        }
    }





}
