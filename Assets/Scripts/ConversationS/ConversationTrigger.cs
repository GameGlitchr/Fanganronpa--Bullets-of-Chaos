using UnityEngine;

public class ConversationTrigger : MonoBehaviour
{
    public ConversationSequencer sequencer; // Reference to the ConversationSequencer

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sequencer.StartConversation();
        }
    }
}
