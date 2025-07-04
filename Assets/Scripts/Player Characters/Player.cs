using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float mouseSensitivity = 1000f;
    public Transform playerBody;
    private float xRotation = 0f;
    public float moveSpeed = 5f;

    public Character character;
    private ArgumentUIManager argumentUIManager;
    private EvidenceUIManager evidenceUIManager;
    private QuestIDManager questIDManager;
    private TabletUIManager tabletManager;

    public bool isTalking;

    public Evidence Evidence;

    public bool argueMenuOpen = false;
    //public bool evidenceMenuOpen = false;

    /*public int TabletUIManager.TabletState;
    public int tabletInRest = 0;
    public int tabletInView = 1;
    public int tabletInFullScreen = 2;*/

    public TabletUIManager.TabletState tabletMode;

    //public bool tabletInView = false;
    //public bool tabletInFullScreen = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        character = GameObject.FindGameObjectWithTag("Character").GetComponent<Character>();
        argumentUIManager = FindFirstObjectByType<ArgumentUIManager>();
        evidenceUIManager = FindFirstObjectByType<EvidenceUIManager>();
        questIDManager = FindFirstObjectByType<QuestIDManager>();
        tabletManager = FindFirstObjectByType<TabletUIManager>();

        // Ensure the UI components are hidden initially
        argumentUIManager.HideArgumentUI();
       // evidenceUIManager.HideEvidenceUI();

    }

    void Update()
    {
        if (PlayerManager.instance.isPaused)
            return;

        HandleMouseLook();
        HandleInputs();

        if (tabletManager == null)
        {
            //Debug.LogWarning("TabletUIManager not found! Disabling tablet logic.");
        }

    }

    /*public void enableFPS(bool enable)
    {
       // GetComponent<CharacterController>().enabled = enable;
       // mouseSensitivity = enable ? originalSensitivity : 0f;
        Cursor.lockState = enable ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enable;
    }*/

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        playerBody.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void HandleInputs()
    {
        // Handle movement input
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction += Camera.main.transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction -= Camera.main.transform.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction -= Camera.main.transform.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Camera.main.transform.right;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            moveSpeed *= 3f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed /= 3f;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            Debug.Log(questIDManager.GetCurrentQuestStage());
        }

        // Normalize direction to ensure consistent movement speed
        direction = direction.normalized;

        // Ensure the player doesn't move vertically
        Vector3 newPosition = transform.position + direction * Time.deltaTime * moveSpeed;
        newPosition.y = 1.6f; // Lock the Y position to 1.6

        // Move the player
        transform.position = newPosition;

        // Handle Argument Window
        if (Input.GetKeyDown(KeyCode.F))
        {
            argueMenuOpen = !argueMenuOpen;
            Debug.Log($"Argument Menu Open: {argueMenuOpen}");

            if (argueMenuOpen)
            {
                //evidenceMenuOpen = false;
                //evidenceUIManager.HideEvidenceUI();
                argumentUIManager.ShowArgumentUI();
            }
            else
            {
                argumentUIManager.HideArgumentUI();
            }

            //ToggleCursorAndMouseSensitivity(argueMenuOpen);
        }

        // Handle Evidence Window
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (tabletMode == TabletUIManager.TabletState.Hidden)
            {
                tabletMode = TabletUIManager.TabletState.View;
            }
            else if (tabletMode == TabletUIManager.TabletState.View)
            {
                tabletMode = TabletUIManager.TabletState.Hidden;
            }

            //Debug.Log("Tablet Mode: " + TabletUIManager.TabletState);
            
          
                ToggleCursorAndMouseSensitivity(tabletMode);
        }

        if (Input.GetKeyDown(KeyCode.E) /*&& evidenceMenuOpen*/)
        {
            //evidenceUIManager.ToggleButtons();
        }
    }

    public void CloseEvidenceMenu()
    {
        //evidenceMenuOpen = false;
        //evidenceUIManager.HideEvidenceUI();
        //ToggleCursorAndMouseSensitivity(evidenceMenuOpen);
    }

    public void ToggleCursorAndMouseSensitivity(TabletUIManager.TabletState tabletMode)
    {
        //Debug.Log($"Toggle Cursor and Mouse Sensitivity: Menu Open: {menuOpen}");
        if (tabletMode == TabletUIManager.TabletState.Hidden)
        {
            Cursor.lockState = CursorLockMode.Locked;
            mouseSensitivity = 1000f;

        }
        else if (tabletMode == TabletUIManager.TabletState.View)
        {
            Cursor.lockState = CursorLockMode.None;
            mouseSensitivity = 500f;
        }
        else if (tabletMode == TabletUIManager.TabletState.Fullscreen)
        {
            Cursor.lockState = CursorLockMode.None;
            mouseSensitivity = 0f;
        }
    }

    public void ToggleDialog(bool isTalking)
    {
        if (isTalking)
        {
            Cursor.lockState = CursorLockMode.None;
            mouseSensitivity = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            mouseSensitivity = 1000f;
        }
    }

    public void MakeArgument(float damage, float truth, float type)
    {
        // Call the character's Argument method with the provided values
        character.Argument(damage, truth, type);
        Debug.Log($"Made an argument with damage: {damage}, truth: {truth}, type: {type}");
    }
}
