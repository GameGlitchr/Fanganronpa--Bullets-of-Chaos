using UnityEngine;
using UnityEngine.UI;

public class TabletUIManager : MonoBehaviour
{
    public enum TabletState { Hidden, View, Fullscreen }
    public enum TabletScreenType { Home, Evidence, Argument, Conclusions, Profiles, Rules, Settings, Camera, Notes, Messages }

    public TabletState currentState = TabletState.Hidden;
    public TabletScreenType currentScreen = TabletScreenType.Home;

    public GameObject tabletObject;
    public Transform restPosition;
    public Transform viewPosition;
    public Transform fullscreenPosition;

    public Transform UIContainer;
    public Transform UIScreenAnchor;

    public GameObject HomeScreen_Portrait;
    public GameObject HomeScreen_Landscape;
    public GameObject EvidenceScreen_Portrait;
    public GameObject EvidenceScreen_Landscape;
    public GameObject ArgumentScreen_Portrait;
    public GameObject ArgumentScreen_Landscape;

    public Button HomeButton;
    public Button FullScreenButton;
    public Button BackButton;

    public Button EvidenceButtonP;
    public Button EvidenceButtonL;
    public Button ArgumentButtonP;
    public Button ArgumentButtonL;
    public Button ConclusionButtonP;
    public Button ConclusionButtonL;
    public Button ProfilesButtonP;
    public Button ProfilesButtonL;
    public Button RulesButtonP;
    public Button RulesButtonL;
    public Button SettingsButtonP;
    public Button SettingsButtonL;
    public Button CameraButtonP;
    public Button CameraButtonL;
    public Button NotesButtonP;
    public Button NotesButtonL;
    public Button MessagesButtonP;
    public Button MessagesButtonL;


    public GameObject evidencePanel;
    public GameObject argumentPanel;

    public PlayerManager playerManager;
    public float moveSpeed = 5f;

    private Transform currentTarget;

    void Start()
    {
        SetState(TabletState.Hidden, instant: true);
        FullScreenButton.onClick.AddListener(ToggleFullscreen);
        HomeButton.onClick.AddListener(GoHome);
        //BackButton.onClick.AddListener();

        EvidenceButtonP.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Evidence));
        EvidenceButtonL.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Evidence));
        ArgumentButtonP.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Argument));
        ArgumentButtonL.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Argument));
        ConclusionButtonP.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Conclusions));
        ConclusionButtonL.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Conclusions));
        ProfilesButtonP.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Profiles));
        ProfilesButtonL.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Profiles));
        RulesButtonP.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Rules));
        RulesButtonL.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Rules));
        SettingsButtonP.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Settings));
        SettingsButtonL.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Settings));
        CameraButtonP.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Camera));
        CameraButtonL.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Camera));
        NotesButtonP.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Notes));
        NotesButtonL.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Notes));
        MessagesButtonP.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Messages));
        MessagesButtonL.onClick.AddListener(() => SetActiveScreen(TabletScreenType.Messages));

        


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (currentState == TabletState.Hidden)
                SetState(TabletState.View);
            else if (currentState == TabletState.View)
                SetState(TabletState.Hidden);
        }

        // Smooth transition in local space
        if (currentTarget != null)
        {
            tabletObject.transform.localPosition = Vector3.Lerp(tabletObject.transform.localPosition, currentTarget.localPosition, Time.unscaledDeltaTime * moveSpeed);
            tabletObject.transform.localRotation = Quaternion.Lerp(tabletObject.transform.localRotation, currentTarget.localRotation, Time.unscaledDeltaTime * moveSpeed);

            if (Vector3.Distance(tabletObject.transform.localPosition, currentTarget.localPosition) < 0.01f)
            {
                tabletObject.transform.localPosition = currentTarget.localPosition;
                tabletObject.transform.localRotation = currentTarget.localRotation;
            }
        }
        AlignUIToScreen();
    }

    private void AlignUIToScreen()
    {
        if (UIContainer != null && UIScreenAnchor != null)
        {
            UIContainer.position = UIScreenAnchor.position;
            UIContainer.rotation = UIScreenAnchor.rotation;
            UIContainer.localScale = Vector3.one;
        }
    }




    public void ToggleFullscreen()
    {
        if (currentState == TabletState.View)
            SetState(TabletState.Fullscreen);
        else if (currentState == TabletState.Fullscreen)
            SetState(TabletState.View);
    }

    public void SetState(TabletState newState, bool instant = false)
    {
        currentState = newState;

        switch (newState)
        {
            case TabletState.Hidden:
                tabletObject.transform.SetParent(restPosition.parent);
                currentTarget = restPosition;
                playerManager.ResumeGame();
                break;

            case TabletState.View:
                tabletObject.transform.SetParent(viewPosition.parent);
                currentTarget = viewPosition;
                playerManager.ResumeGame();
                break;

            case TabletState.Fullscreen:
                tabletObject.transform.SetParent(fullscreenPosition.parent);
                currentTarget = fullscreenPosition;
                playerManager.PauseGame();
                break;
        }

        if (instant && currentTarget != null)
        {
            tabletObject.transform.localPosition = currentTarget.localPosition;
            tabletObject.transform.localRotation = currentTarget.localRotation;
        }

        SetActiveScreen(currentScreen);  // ✅ Always reapply active screen
    }

    public void SetActiveScreen(TabletScreenType screen)
    {
        currentScreen = screen;

        // Disable all canvases
        HomeScreen_Portrait.SetActive(false);
        HomeScreen_Landscape.SetActive(false);
        EvidenceScreen_Portrait.SetActive(false);
        EvidenceScreen_Landscape.SetActive(false);
        ArgumentScreen_Portrait.SetActive(false);
        ArgumentScreen_Landscape.SetActive(false);

        // Also disable panels tied to these screens
        evidencePanel.SetActive(false);
        //argumentPanel.SetActive(false);

        bool isFullscreen = currentState == TabletState.Fullscreen;

        switch (screen)
        {
            case TabletScreenType.Home:
                if (isFullscreen) HomeScreen_Landscape.SetActive(true);
                else HomeScreen_Portrait.SetActive(true);
                break;

            case TabletScreenType.Evidence:
                if (isFullscreen) EvidenceScreen_Landscape.SetActive(true);
                else EvidenceScreen_Portrait.SetActive(true);
                evidencePanel.SetActive(true); // ✅ make sure the panel comes back!
                break;

            case TabletScreenType.Argument:
                if (isFullscreen) ArgumentScreen_Landscape.SetActive(true);
                else ArgumentScreen_Portrait.SetActive(true);
                //argumentPanel.SetActive(true);
                break;

            case TabletScreenType.Conclusions:
                Debug.Log("Conclusion Button Clicked.");
                break;
            case TabletScreenType.Profiles:
                Debug.Log("Profile Button Clicked.");
                break;
            case TabletScreenType.Rules:
                Debug.Log("Rules Button Clicked.");
                break;
            case TabletScreenType.Settings:
                Debug.Log("Settings Button Clicked.");
                break;
            case TabletScreenType.Camera:
                Debug.Log("Camera Button Clicked.");
                break;
            case TabletScreenType.Notes:
                Debug.Log("Notes Button Clicked.");
                break;
            case TabletScreenType.Messages:
                Debug.Log("Messages Button Clicked.");
                break;
        }
    }


    public void ShowEvidenceUI()
    {
        SetActiveScreen(TabletScreenType.Evidence);
        evidencePanel.SetActive(true);
        FindFirstObjectByType<EvidenceUIManager>().EnsurePanelActive();
        //argumentPanel.SetActive(false);
    }

    public void ShowArgumentUI()
    {
        SetActiveScreen(TabletScreenType.Argument);
        argumentPanel.SetActive(true);
        //evidencePanel.SetActive(false);
    }

    public void GoHome()
    {
        SetActiveScreen(TabletScreenType.Home);
        evidencePanel.SetActive(false);
        //argumentPanel.SetActive(false);
    }
}
