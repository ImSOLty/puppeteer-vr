using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


class FrameData
{
    // ActionObject ID: ActionObjectData from ActionObject
    public Dictionary<int, ActionObjectData[]> actionObjectDatas;
    public int frame;

    public FrameData(int frame)
    {
        actionObjectDatas = new();
        this.frame = frame;
    }
    public void AddActionObjectData(ActionObject obj)
    {
        if (!actionObjectDatas.ContainsKey(obj.GetInstanceID()))
        {
            actionObjectDatas.Add(obj.GetInstanceID(), obj.GetActionData());
        }
        else
        {
            actionObjectDatas[obj.GetInstanceID()] = obj.GetActionData();
        }
    }
}
public class ActionRecorder : MonoBehaviour
{
    private Dictionary<int, FrameData> frameDataByFrame = new();
    private Dictionary<int, ActionObject> actionObjects = new();
    private AnimationManager animationManager;
    public CharacterManager characterManager;

    void Awake()
    {
        animationManager = FindObjectOfType<AnimationManager>();
        characterManager = FindObjectOfType<CharacterManager>();
    }

    void Start()
    {
        foreach (ActionObject actionObject in FindObjectsOfType<ActionObject>())
        {
            actionObjects.Add(actionObject.GetInstanceID(), actionObject);
        }
    }

    public void Action()
    {
        RecreateFromRecorded();
        if (animationManager.CurrentActionType == ActionType.RECORDING)
        {
            RecordFrame();
        }
    }

    private void RecreateFromRecorded()
    {
        // TODO: Add stop on final frame
        int frame = animationManager.CurrentFrame;
        if (!frameDataByFrame.ContainsKey(frame))
        {
            return;
        }
        FrameData frameData = frameDataByFrame[frame];
        foreach (KeyValuePair<int, ActionObjectData[]> entry in frameData.actionObjectDatas)
        {
            ActionObject actionObject = actionObjects[entry.Key];
            if (actionObject.IsActive())
            {
                continue;
            }
            actionObject.SetByActionData(entry.Value);
        }
    }

    private void RecordFrame()
    {
        int frame = animationManager.CurrentFrame;
        if (!frameDataByFrame.ContainsKey(frame))
        {
            frameDataByFrame.Add(frame, new FrameData(frame));
        }
        FrameData frameData = frameDataByFrame[frame];
        foreach (ActionObject actionObject in actionObjects.Values)
        {
            if (actionObject.IsActive())
            {
                frameData.AddActionObjectData(actionObject);
            }
        }
    }

    public void DeactivateAllActionObjects()
    {
        foreach (ActionObject actionObject in actionObjects.Values)
        {
            actionObject.SetActive(false);
        }
    }
    public void ManageRigidbodyAllActionObjects(bool active)
    {
        foreach (ActionObject actionObject in actionObjects.Values)
        {
            actionObject.RigidbodyActive(active);
        }
    }
}
