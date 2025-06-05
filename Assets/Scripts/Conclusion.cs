using UnityEngine;

public class Conclusion
{
    public string Text { get; }
    public int RelevanceToTopic { get; }
    public int RelevanceToMurder { get; }
    public int Truthfulness { get; }
    public int ArgumentType { get; }

    public Conclusion(string text, int relevanceToTopic, int relevanceToMurder, int truthfulness, int argumentType)
    {
        Text = text;
        RelevanceToTopic = relevanceToTopic;
        RelevanceToMurder = relevanceToMurder;
        Truthfulness = truthfulness;
        ArgumentType = argumentType;
    }
}

