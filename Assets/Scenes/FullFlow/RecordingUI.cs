using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class RecordingUI : MonoBehaviour
{
    public SteamVR_Action_Boolean recordAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("XPress");
    public SteamVR_Action_Boolean endAnimationAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("YPress");
    private AnimationManager animationManager;
    [SerializeField] private Text timerText;

    [SerializeField] private Vector3 recordManagementSubScenePosition;
    [SerializeField] private GameObject recordManagementSubScene;

    void Awake()
    {
        animationManager = FindObjectOfType<AnimationManager>();
        recordAction.AddOnStateDownListener(Record(), SteamVR_Input_Sources.Any);
        endAnimationAction.AddOnStateDownListener(StopAndManageRecording(), SteamVR_Input_Sources.Any);
    }

    private SteamVR_Action_Boolean.StateDownHandler Record()
    {
        return delegate
        {
            animationManager.StartRecording();
        };
    }

    private SteamVR_Action_Boolean.StateDownHandler StopAndManageRecording()
    {
        return delegate
        {
            Instantiate(recordManagementSubScene);
            recordManagementSubScene.transform.position = recordManagementSubScenePosition;
            FindObjectOfType<RecordManagementManager>().SwitchToRecordManagement();
        };
    }

    public void SetTime(int secondsRemaining)
    {
        timerText.text = secondsRemaining.ToString();
    }

    public void Action()
    {
        SetTime(animationManager.GetSecondsLeft());
    }

    private void OnDisable()
    {
        recordAction.RemoveAllListeners(SteamVR_Input_Sources.Any);
        endAnimationAction.RemoveAllListeners(SteamVR_Input_Sources.Any);
    }
}
