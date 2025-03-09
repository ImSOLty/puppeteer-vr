using Unity.VisualScripting;
using UnityEngine;
public enum ActionType
{
    UNKNOWN,
    RECORDING,
    PLAYING,
}

public class AnimationManager : MonoBehaviour
{
    private int animationSeconds = 10;
    private int totalAnimationFrames;
    private int currentFrame = 0;
    public int CurrentFrame { get { return currentFrame; } }
    private ActionType actionType = ActionType.UNKNOWN;
    public ActionType CurrentActionType { get { return actionType; } }

    private ActionRecorder actionRecorder;
    private RecordingUI recordingUI;


    void Awake()
    {
        SetupAnimationSettings();
        actionRecorder = FindObjectOfType<ActionRecorder>();
        recordingUI = FindObjectOfType<RecordingUI>();
    }

    void FixedUpdate()
    {
        if (actionType == ActionType.UNKNOWN)
        {
            return;
        }
        preUpdateSetup();
        actionRecorder.Action();
        recordingUI.Action();
    }

    void preUpdateSetup()
    {
        currentFrame += 1;
        if (currentFrame == totalAnimationFrames && actionType == ActionType.RECORDING)
        {
            StopRecording();
        }
    }

    public void StartRecording()
    {
        actionType = ActionType.RECORDING;
        currentFrame = 0;
    }

    public void StopRecording()
    {
        actionType = ActionType.UNKNOWN;
        currentFrame = 0;
        recordingUI.Action();
    }

    private void SetupAnimationSettings()
    {
        totalAnimationFrames = (int)(animationSeconds / Time.fixedDeltaTime);
    }

    public int GetSecondsLeft()
    {
        return animationSeconds - (int)(currentFrame * Time.fixedDeltaTime);
    }
}
