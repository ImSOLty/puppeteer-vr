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
    private int currentFrame = 0;
    public int CurrentFrame { get { return currentFrame; } }
    private ActionType actionType = ActionType.UNKNOWN;
    public ActionType CurrentActionType { get { return actionType; } set { actionType = value; } }

    private ActionRecorder actionRecorder;
    private RecordingUI recordingUI;
    public bool isRecording = false;
    public bool hasRecorded = false;


    void Awake()
    {
        actionRecorder = FindObjectOfType<ActionRecorder>();
        recordingUI = FindObjectOfType<RecordingUI>();
        Time.fixedDeltaTime = 1.0f / Settings.Animation.FPS;
    }

    void FixedUpdate()
    {
        if (actionType != ActionType.RECORDING)
        {
            return;
        }
        preUpdate();
        actionRecorder.Action();
        recordingUI.Action();
        postUpdate();
    }

    void preUpdate()
    {
        if (currentFrame == Settings.Animation.TotalFrames() && actionType == ActionType.RECORDING)
        {
            StopRecording();
        }
    }
    void postUpdate()
    {
        currentFrame += 1;
    }

    public void StartRecording()
    {
        ActionCharacter currentCharacter = actionRecorder.characterManager.GetCurrentCharacter();
        if (currentCharacter == null)
        {
            return;
        }
        isRecording = true;
        currentCharacter.wasRecorded = true; // Set character as recorded
        currentCharacter.SetActive(true);
        actionType = ActionType.RECORDING;
        currentFrame = 0;
        actionRecorder.ManageRigidbodyAllActionObjects(false);
    }

    public void StopRecording()
    {
        isRecording = false;
        hasRecorded = true;
        actionType = ActionType.UNKNOWN;
        currentFrame = 0;
        //Just to reset
        recordingUI.Action();
        actionRecorder.Action();
        actionRecorder.DeactivateAllActionObjects();
        actionRecorder.ManageRigidbodyAllActionObjects(true);
    }

    public int GetSecondsLeft()
    {
        return Settings.Animation.TotalTimeInSeconds - (int)(currentFrame / Settings.Animation.FPS);
    }

    public bool SetupForFrame(int frame)
    {
        if (actionType != ActionType.PLAYING)
        {
            return false;
        }
        currentFrame = frame;
        actionRecorder.Action();
        return true;
    }
}
