using System.Collections.Generic;
using UnityEngine;

public class ObjectTimeline : MonoBehaviour
{
    private int _frame = 0;
    private bool _recording = false;
    private ObjectToRecord[] _objects;
    private Dictionary<int, ObjectData[]> _objectsData = new();
    private List<ObjectData> _frameData = new();
    private List<ObjectData> _staticObjects = new();

    public void StartRecordingObjects()
    {
        _objects = FindObjectsOfType<ObjectToRecord>();
        _recording = true;
        foreach (ObjectToRecord record in _objects)
        {
            if (record.IsStatic())
                _staticObjects.Add(record.GetStaticData());
            else
                record.ChangeRecordingState(true);
        }
    }

    public void AddFrameData(ObjectData data)
    {
        _frameData.Add(data);
    }

    public void StopRecordingObjects()
    {
        _recording = false;
        foreach (ObjectToRecord record in _objects)
        {
            record.ChangeRecordingState(false);
        }
    }

    public ObjectData[] DataForFrame(int frame)
    {
        return _objectsData.ContainsKey(frame) ? _objectsData[frame] : null;
    }

    public (Dictionary<int, ObjectData[]>, ObjectData[]) GetData()
    {
        return (_objectsData, _staticObjects.ToArray());
    }

    void LateUpdate()
    {
        if (!_recording) return;

        _objectsData.Add(_frame, _frameData.ToArray());
        _frameData.Clear();
        _frame += 1;
    }
}