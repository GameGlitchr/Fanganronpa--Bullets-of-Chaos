using UnityEngine;
using UnityEngine.UI;

public class TabletUIManager : MonoBehaviour
{
    public enum TabletState { Hidden, View, Fullscreen }
    public TabletState currentState = TabletState.Hidden;

    public GameObject tabletObject;
    public Transform restPosition;
    public Transform viewPosition;
    public Transform fullscreenPosition;

    public Canvas portraitCanvas;
    public Canvas fullscreenCanvas;

    public GameObject evidencePanel;
    public GameObject argumentPanel;

    public PlayerManager playerManager;

    public float moveSpeed = 5f;

    private Transform currentTarget;

    void Start()
    {
        SetState(TabletState.Hidden, instant: true);
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
            tabletObject.transform.localPosition = Vector3.Lerp(tabletObject.transform.localPosition, currentTarget.localPosition, Time.deltaTime * moveSpeed);
            tabletObject.transform.localRotation = Quaternion.Lerp(tabletObject.transform.localRotation, currentTarget.localRotation, Time.deltaTime * moveSpeed);
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
                tabletObject.transform.SetParent(restPosition.parent); // Stay in local space
                currentTarget = restPosition;
                portraitCanvas.enabled = false;
                fullscreenCanvas.enabled = false;
                playerManager.ResumeGame();
                break;

            case TabletState.View:
                tabletObject.transform.SetParent(viewPosition.parent); // Typically the camera
                currentTarget = viewPosition;
                portraitCanvas.enabled = true;
                fullscreenCanvas.enabled = false;
                playerManager.ResumeGame();
                break;

            case TabletState.Fullscreen:
                tabletObject.transform.SetParent(fullscreenPosition.parent);
                currentTarget = fullscreenPosition;
                portraitCanvas.enabled = false;
                fullscreenCanvas.enabled = true;
                playerManager.PauseGame();
                break;
        }

        if (instant && currentTarget != null)
        {
            tabletObject.transform.localPosition = currentTarget.localPosition;
            tabletObject.transform.localRotation = currentTarget.localRotation;
        }
    }

    public void ShowEvidenceUI()
    {
        evidencePanel.SetActive(true);
        argumentPanel.SetActive(false);
    }

    public void ShowArgumentUI()
    {
        argumentPanel.SetActive(true);
        evidencePanel.SetActive(false);
    }

    public void GoHome()
    {
        evidencePanel.SetActive(false);
        argumentPanel.SetActive(false);
    }
}
