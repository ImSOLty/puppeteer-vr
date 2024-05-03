using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSimulator : MonoBehaviour
{
    private CameraManager _cameraManager;
    private List<GameObject> _cameraManipulators;
    private List<CameraInstance> _cameraInstances;

    private CameraTimeline _timeline;

    void Start()
    {
        Setup();
        foreach (Vector3 place in new[]
                 {
                     new Vector3(-7.11f, 12.88f, -7.11f),
                     new Vector3(-7.11f, 12.88f, 7.11f),
                     new Vector3(7.11f, 12.88f, -7.11f),
                     new Vector3(7.11f, 12.88f, 7.11f),
                 })
        {
            GameObject manipulator = new GameObject("Manipulator");
            manipulator.transform.position = place;
            _cameraInstances.Add(_cameraManager.CreateNewCamera(place, manipulator.transform));
            _cameraManipulators.Add(manipulator);
            manipulator.transform.LookAt(Vector3.zero);
        }

        // Set timeline duration and add 4 sections
        Debug.Log(_timeline);
        _timeline.SetDuration(10);
        Debug.Log(_timeline);
        _timeline.AddSection(_cameraInstances[0], 2,3);
        Debug.Log(_timeline);
    }

    void Setup()
    {
        _cameraManipulators = new List<GameObject>();
        _cameraInstances = new List<CameraInstance>();
        _timeline = FindObjectOfType<CameraTimeline>();
        _cameraManager = FindObjectOfType<CameraManager>();
    }
}