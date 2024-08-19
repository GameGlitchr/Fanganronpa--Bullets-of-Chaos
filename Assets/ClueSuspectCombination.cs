using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueSuspectCombination : MonoBehaviour
{

    public int Clue1 { get; }
    public int Clue2 { get; }
    public int Suspect { get; }

    public string drawnConclusion;
    public int conclusionTruthfulness;
    public int emotionalLogical;
    public int relevanceToTopic;
    public int relevanceToMurder;


    public ClueSuspectCombination(int clue1, int clue2, int suspect)
    {
        Clue1 = Mathf.Min(clue1, clue2); // Ensure Clue1 is always the smaller value
        Clue2 = Mathf.Max(clue1, clue2); // Ensure Clue2 is always the larger value
        Suspect = suspect;
    }

    public override bool Equals(object obj)
    {
        if (obj is ClueSuspectCombination other)
        {
            return Clue1 == other.Clue1 && Clue2 == other.Clue2 && Suspect == other.Suspect;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Clue1.GetHashCode() ^ Clue2.GetHashCode() ^ Suspect.GetHashCode();
    }

}
