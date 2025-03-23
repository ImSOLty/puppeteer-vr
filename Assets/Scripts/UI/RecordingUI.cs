using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using Valve.VR;

public class RecordingUI : MonoBehaviour
{
    public SteamVR_Behaviour_Pose handPose;
    public SteamVR_Action_Boolean recordAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("XPress");
    public SteamVR_Action_Boolean endAnimationAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("YPress");
    private AnimationManager animationManager;
    [SerializeField] private Text timerText;

    [SerializeField] private Vector3 recordManagementSubScenePosition;
    [SerializeField] private GameObject recordManagementSubScene;

    void Awake()
    {
        handPose = FindObjectOfType<LaserInteractor>().GetComponent<SteamVR_Behaviour_Pose>();
        if (Settings.Animation.AnimationMode == Mode.ANIMATION_RUNTIME)
        {
            Destroy(this.gameObject);
        }

        animationManager = FindObjectOfType<AnimationManager>();
        recordAction.AddOnStateDownListener(Record(), handPose.inputSource);
        endAnimationAction.AddOnStateDownListener(StopAndManageRecording(), handPose.inputSource);

        //Setup Constraint
        ParentConstraint constraint = GetComponent<ParentConstraint>();
        constraint.AddSource(new ConstraintSource()
        {
            weight = 1.0f,
            sourceTransform = handPose.transform,
        });
        constraint.locked = true;

        SetTime(Settings.Animation.TotalTimeInSeconds);
    }

    private SteamVR_Action_Boolean.StateDownHandler Record()
    {
        return delegate
        {
            if (Settings.Animation.AnimationMode != Mode.ANIMATION_RECORDING)
            {
                return;
            }
            animationManager.StartRecording();
        };
    }

    private SteamVR_Action_Boolean.StateDownHandler StopAndManageRecording()
    {
        return delegate
        {
            if (Settings.Animation.AnimationMode != Mode.ANIMATION_RECORDING)
            {
                return;
            }
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
        recordAction.RemoveAllListeners(handPose.inputSource);
        endAnimationAction.RemoveAllListeners(handPose.inputSource);
    }
}
