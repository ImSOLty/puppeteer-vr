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

    public ObjectData GetStaticData()
    {
        return new ObjectData(transform);
    }

    public bool IsStatic()
    {
        return _isStatic;
    }
}