using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Dictionary<int, CameraInstance> _cameraInstances = new();
    [SerializeField] private GameObject _cameraPrefab;

    public CameraInstance CreateNewCamera(Vector3 place, Transform attachTo)
    {
        GameObject newCamera = Instantiate(_cameraPrefab, place, Quaternion.identity, attachTo);
        CameraInstance newInstance = newCamera.GetComponent<CameraInstance>();
        _cameraInstances.Add(newCamera.GetInstanceID(), newInstance);
        return newInstance;
    }
}