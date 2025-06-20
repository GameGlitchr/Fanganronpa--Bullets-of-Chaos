using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConversationLine
{
    public string charName; // Reference to the NPC who will speak
    public string charEmotion;
    public string dialogLine; // The dialog line to be spoken
}

[CreateAssetMenu(fileName = "New Conversation", menuName = "Conversation")]
public class Conversation : ScriptableObject
{
    public List<ConversationLine> conversationLines; // List of conversation lines
}
