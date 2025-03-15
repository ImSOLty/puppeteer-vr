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
    private int animationSeconds = 2;
    private int totalAnimationFrames;
    public int TotalAnimationFrames { get { return totalAnimationFrames; } }
    private int currentFrame = 0;
    public int CurrentFrame { get { return currentFrame; } }
    private ActionType actionType = ActionType.UNKNOWN;
    public ActionType CurrentActionType { get { return actionType; } set { actionType = value; } }

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
        if (currentFrame == totalAnimationFrames && actionType == ActionType.RECORDING)
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
        currentCharacter.wasRecorded = true; // Set character as recorded
        currentCharacter.SetActive(true);
        actionType = ActionType.RECORDING;
        currentFrame = 0;
        actionRecorder.ManageRigidbodyAllActionObjects(false);
    }

    public void StopRecording()
    {
        actionType = ActionType.UNKNOWN;
        currentFrame = 0;
        //Just to reset
        recordingUI.Action();
        actionRecorder.Action();
        actionRecorder.DeactivateAllActionObjects();
        actionRecorder.ManageRigidbodyAllActionObjects(true);
    }

    private void SetupAnimationSettings()
    {
        totalAnimationFrames = (int)(animationSeconds / Time.fixedDeltaTime);
    }

    public int GetSecondsLeft()
    {
        return animationSeconds - (int)(currentFrame * Time.fixedDeltaTime);
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
