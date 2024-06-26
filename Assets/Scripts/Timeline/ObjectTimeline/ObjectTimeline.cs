using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectTimeline : MonoBehaviour
{
    private float _previousFrameTime = 0;
    private int _frame = 0;
    private bool _recording = false;
    private ObjectToRecord[] _objects;
    private Dictionary<int, Dictionary<int, ObjectData>> _objectsData = new();
    private List<ObjectData> _staticObjects = new();
    private List<Transform> _dynamicObjects = new();

    public void StartRecordingObjects()
    {
        _objects = FindObjectsOfType<ObjectToRecord>();
        _recording = true;
        _staticObjects.Clear();
        _dynamicObjects.Clear();
        _objectsData.Clear();
        _frame = 0;

        foreach (ObjectToRecord record in _objects)
        {
            if (record.IsStatic())
            {
                _staticObjects.Add(record.GetStaticData());
            }
            else
            {
                _dynamicObjects.Add(record.transform);
                _objectsData[record.transform.GetInstanceID()] = new Dictionary<int, ObjectData>();
            }
        }
    }

    public void StopRecordingObjects()
    {
        _recording = false;
        _frame -= 1;
    }

    public Dictionary<int, ObjectData> DataForFrame(int frame)
    {
        return _objectsData.ContainsKey(frame) ? _objectsData[frame] : null;
    }

    public (int, Dictionary<int, Dictionary<int, ObjectData>>, ObjectData[]) GetData()
    {
        return (_frame, _objectsData, _staticObjects.ToArray());
    }

    void LateUpdate()
    {
        if (!_recording || Time.time - _previousFrameTime < 1 / AnimationSettings.FPS) return;
        _previousFrameTime = Time.time - (Time.time - _previousFrameTime) % (1 / AnimationSettings.FPS);
        foreach (var objTransform in _dynamicObjects)
        {
            _objectsData[objTransform.GetInstanceID()].Add(_frame, new ObjectData(objTransform));
        }

        _frame += 1;
    }

    public void UpdateObjectsForFrame(int frame)
    {
        foreach (Transform dynamicObjectTransform in _dynamicObjects)
        {
            ObjectData dataForFrame = _objectsData[dynamicObjectTransform.GetInstanceID()][frame];
            dynamicObjectTransform.localPosition = dataForFrame.Position;
            dynamicObjectTransform.localRotation = dataForFrame.Rotation;
        }
    }

    public IEnumerator MoveObjectsForXFrames(int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            UpdateObjectsForFrame(i);
            yield return new WaitForEndOfFrame();
        }
    }
}