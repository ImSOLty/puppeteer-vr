using System;
using System.Collections.Generic;
using UnityEngine;


public class ObjectData
{
    public ObjectData(Transform objectTransform)
    {
        Position = objectTransform.position;
        Rotation = objectTransform.rotation;
        ObjectId = objectTransform.gameObject.GetInstanceID();
    }

    public int ObjectId { get; set; }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public override string ToString() => $"Position: {Position}, Rotation: {Rotation}";
}

public class ObjectToRecord : MonoBehaviour
{
    [SerializeField] private bool _isStatic = false;
    private ObjectTimeline _timeline;
    private bool _recording;

    private void Start()
    {
        _timeline = FindObjectOfType<ObjectTimeline>();
    }

    private void Update()
    {
        if (_recording && !_isStatic) _timeline.AddFrameData(new ObjectData(transform));
    }

    public ObjectData GetStaticData()
    {
        return new ObjectData(transform);
    }

    public void ChangeRecordingState(bool recording)
    {
        _recording = recording;
    }

    public bool IsStatic()
    {
        return _isStatic;
    }
}