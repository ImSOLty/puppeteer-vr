using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class RecordManagementManager : MonoBehaviour
{
    public SteamVR_Behaviour_Pose handPose;
    public SteamVR_Action_Boolean yPressAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("YPress"); // Back to Menu
    [SerializeField] private Transform newPlayerPosition;
    [SerializeField] private GameObject recordManagementUICanvas;
    [SerializeField] private RecordControlsUI recordControlsUI;
    [SerializeField] private RecordSettingsUI recordSettingsUI;
    [SerializeField] private CameraTimeline cameraTimeline;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private CameraLinesManager cameraLinesManager;

    void Start()
    {
        handPose = FindObjectOfType<LaserInteractor>().GetComponent<SteamVR_Behaviour_Pose>();
        yPressAction.AddOnStateDownListener(BackToMainMenu(), handPose.inputSource);
    }

    public void SwitchToRecordManagement()
    {
        // Deactivate avatar
        CharacterManager characterManager = FindObjectOfType<CharacterManager>();
        characterManager.DetachCharacter();

        // Deactivate rb and interactable in all action objects
        ActionRecorder actionRecorder = FindObjectOfType<ActionRecorder>();
        actionRecorder.ManageRigidbodyAllActionObjects(false);
        actionRecorder.ManageInteractableAllActionObjects(false);

        // Deactivate UI, managers and action handlers
        FindObjectOfType<AnimationManager>().CurrentActionType = ActionType.PLAYING;
        FindObjectOfType<Teleport>().enabled = false;
        FindObjectOfType<ObjectsUI>().gameObject.SetActive(false);
        RecordingUI recordingUI = FindObjectOfType<RecordingUI>();
        recordingUI.OnDisable();
        recordingUI.gameObject.SetActive(false);

        // Activate RecordManagementUI and managers in exact order
        recordManagementUICanvas.SetActive(true);
        cameraLinesManager.Setup();
        cameraManager.Setup();
        cameraTimeline.Setup();
        recordSettingsUI.Setup();
        recordControlsUI.Setup();
        recordControlsUI.enabled = true;

        // Move player to a position
        Player player = FindObjectOfType<Player>();
        player.transform.SetPositionAndRotation(newPlayerPosition.position, Quaternion.identity);
    }

    private SteamVR_Action_Boolean.StateDownHandler BackToMainMenu()
    {
        return delegate { SceneManager.LoadScene(Settings.Scenes.MainMenuSceneName); };
    }
}
