using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraManager : MonoBehaviour
{
    private Dictionary<int, CameraInstance> _cameraInstances = new();
    [SerializeField] private GameObject _cameraPrefab;
    private LinkedList<CameraInstance> _instancesOrder = new();

    private void Awake()
    {
        // Find default cameras
        foreach (var cameraInstance in FindObjectsOfType<CameraInstance>())
        {
            _cameraInstances.Add(cameraInstance.gameObject.GetInstanceID(), cameraInstance);
            _instancesOrder.AddLast(cameraInstance);
        }
    }

    public CameraInstance CreateNewCamera(Vector3 place, Transform attachTo)
    {
        GameObject newCamera = Instantiate(_cameraPrefab, place, Quaternion.identity, attachTo);
        CameraInstance newInstance = newCamera.GetComponent<CameraInstance>();
        newInstance.UpdateResolution(CameraConstants.DefaultWidth, CameraConstants.DefaultHeight); // Should be changed
        newInstance.UpdateColor(Random.ColorHSV());
        _cameraInstances.Add(newCamera.GetInstanceID(), newInstance);
        _instancesOrder.AddLast(newInstance);
        return newInstance;
    }

    public LinkedList<CameraInstance> GetCameraInstances()
    {
        return _instancesOrder;
    }

    public void UpdateAllCamerasResolution(ExportResolution resolution)
    {
        foreach (var cameraInstance in _instancesOrder)
        {
            cameraInstance.UpdateResolution(width: resolution.width, height: resolution.height);
        }
    }
}