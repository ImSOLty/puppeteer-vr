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

    void Awake()
    {
        animationManager = FindObjectOfType<AnimationManager>();
        recordAction.AddOnStateDownListener(Record(), SteamVR_Input_Sources.Any);
        endAnimationAction.AddOnStateDownListener(Record(), SteamVR_Input_Sources.Any);
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
            animationManager.StartRecording();
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
}
