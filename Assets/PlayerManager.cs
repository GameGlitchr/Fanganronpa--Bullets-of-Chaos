using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //public LogicScript_Trevor logic;
    //public PortalBehvaior_Alex portal;
    public static PlayerManager instance;


    public GameObject player;

    public bool isPaused;


    private void Awake()
    {
        //logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript_Trevor>();
        //portal = GameObject.FindGameObjectWithTag("Portal").GetComponent<PortalBehvaior_Alex>();

        instance = this;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            TogglePaused();
        }   
    }

    private void TogglePaused()
    {
        if (!isPaused)
        {
            //logic.SkillTree();
            enableFPS(false);

        }
        else if (isPaused)
        {
           // logic.SkillTreeVanish();
            enableFPS(true);

        }
        isPaused = !isPaused;
    }

    public void enableFPS(bool enable)
    {
        //Characte characterController = GameObject.Find("Player");
        CharacterController characterController = GameObject.FindGameObjectWithTag("PlayerCharacter").GetComponent<CharacterController>();

        if (enable)
        {
            //Enable FPS script
            characterController.enabled = true;
        }
        else
        {
            //Disable FPS script
            characterController.enabled = false;
            //Unlock Mouse and make it visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
