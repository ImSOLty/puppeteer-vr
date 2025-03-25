using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class RecordingUI : MonoBehaviour
{
    public SteamVR_Behaviour_Pose handPose;
    private SteamVR_Behaviour_Pose otherHandPose;
    public SteamVR_Action_Boolean xPressAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("XPress"); // Start Recording
    public SteamVR_Action_Boolean yPressAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("YPress"); // End Recording / Back to Menu
    private AnimationManager animationManager;
    [SerializeField] private Text timerText;

    [SerializeField] private Vector3 recordManagementSubScenePosition;
    [SerializeField] private GameObject recordManagementSubScene;

    void Awake()
    {
        handPose = FindObjectOfType<LaserInteractor>().GetComponent<SteamVR_Behaviour_Pose>();
        otherHandPose = handPose.GetComponent<Hand>().otherHand.GetComponent<SteamVR_Behaviour_Pose>();

        if (Settings.Animation.AnimationMode == Mode.ANIMATION_RUNTIME)
        {
            Destroy(this.gameObject);
        }

        animationManager = FindObjectOfType<AnimationManager>();
        xPressAction.AddOnStateDownListener(Record(), handPose.inputSource);
        yPressAction.AddOnStateDownListener(StopAndManageRecording(), otherHandPose.inputSource);
        yPressAction.AddOnStateDownListener(BackToMainMenu(), handPose.inputSource);

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
            if (animationManager.isRecording)
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
            if (!animationManager.hasRecorded || animationManager.isRecording)
            {
                return;
            }
            Instantiate(recordManagementSubScene);
            recordManagementSubScene.transform.position = recordManagementSubScenePosition;
            FindObjectOfType<RecordManagementManager>().SwitchToRecordManagement();
        };
    }

    private SteamVR_Action_Boolean.StateDownHandler BackToMainMenu()
    {
        return delegate { SceneManager.LoadScene(Settings.Scenes.MainMenuSceneName); };
    }

    public void SetTime(int secondsRemaining)
    {
        timerText.text = secondsRemaining.ToString();
    }

    public void Action()
    {
        SetTime(animationManager.GetSecondsLeft());
    }

    public void OnDisable()
    {
        xPressAction.RemoveAllListeners(handPose.inputSource);
        yPressAction.RemoveAllListeners(handPose.inputSource);
        xPressAction.RemoveAllListeners(otherHandPose.inputSource);
        yPressAction.RemoveAllListeners(otherHandPose.inputSource);
    }
}
