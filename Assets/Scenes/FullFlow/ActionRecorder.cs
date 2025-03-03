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
        actionObjectDatas.Add(obj.GetInstanceID(), obj.GetActionData());
    }
}
public class ActionRecorder : MonoBehaviour
{
    Dictionary<int, FrameData> frameDataByFrame = new();
    ActionObject[] actionObjects;
    int frame = 0;
    int totalRecorded = 0;
    public void Start()
    {
        actionObjects = FindObjectsOfType<ActionObject>();
    }

    public void FixedUpdate()
    {
        FrameData frameData;
        if (!frameDataByFrame.ContainsKey(frame))
        {
            frameDataByFrame.Add(frame, new FrameData(frame));
        }
        frameData = frameDataByFrame[frame];
        foreach (ActionObject actionObject in actionObjects)
        {
            frameData.AddActionObjectData(actionObject);
            if (frame > 10)
                actionObject.SetByActionData(frameData.actionObjectDatas[actionObject.GetInstanceID()]);
            totalRecorded += actionObject.GetActionData().Length;
        }
        Debug.Log(totalRecorded);
        frame += 1;
    }

    public void StartRecording()
    {

    }

    public void EndRecording()
    {

    }
}
