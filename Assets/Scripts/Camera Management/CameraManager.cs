using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Dictionary<int, CameraInstance> _cameraInstances;
    private GameObject _cameraPrefab;

    private void Start()
    {
        _cameraInstances = new Dictionary<int, CameraInstance>();
    }

    void CreateNewCamera()
    {
        GameObject newCamera = Instantiate(_cameraPrefab);
        CameraInstance newInstance = newCamera.GetComponent<CameraInstance>();
        _cameraInstances.Add(newCamera.GetInstanceID(), newInstance);
    }
}