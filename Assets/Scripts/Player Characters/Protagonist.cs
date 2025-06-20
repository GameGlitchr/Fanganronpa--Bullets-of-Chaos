using UnityEngine;

public class Protagonist : CharacterBase
{
    protected override void Start()
    {
        base.Start();
        // Any additional player-specific initialization
    }

    void Update()
    {
        IncrementDespair();
        UpdateMood();
        ControlMinMax();
    }

    private void IncrementDespair()
    {
        Despair += Time.deltaTime / 10;
    }

    /*protected override void GetIdentification()
    {
        // Player-specific identification
        charID = 0;
        charName = "Ioko Kabuto";
        gender = 1;
        sex = "female";
    }*/
}
