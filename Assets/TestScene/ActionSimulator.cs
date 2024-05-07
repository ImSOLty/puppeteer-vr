using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSimulator : MonoBehaviour
{
    private CameraManager _cameraManager;
    private List<GameObject> _cameraManipulators;
    private List<CameraInstance> _cameraInstances;

    private CameraTimeline _cameraTimeline;
    private Recorder _recorder;

    private bool _randomActions = false;

    void Start()
    {
        GetComponents();
        SetupCameras();
        SetupCameraSections();
    }

    void GetComponents()
    {
        _cameraManipulators = new List<GameObject>();
        _cameraInstances = new List<CameraInstance>();
        _cameraTimeline = FindObjectOfType<CameraTimeline>();
        _cameraManager = FindObjectOfType<CameraManager>();
        _recorder = FindObjectOfType<Recorder>();
    }

    void SetupCameras()
    {
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
    }

    void SetupCameraSections()
    {
        // Set timeline duration and add 4 sections
        float duration = 10;
        float sectionDuration = duration / _cameraInstances.Count;
        _cameraTimeline.SetDuration(duration);
        for (int i = 0; i < _cameraInstances.Count; i++)
        {
            _cameraTimeline.AddSection(_cameraInstances[i], i * sectionDuration, (i + 1) * sectionDuration);
            Debug.Log(_cameraTimeline);
        }
    }

    public void ChangeRandomActionsAllowance()
    {
        _randomActions = !_randomActions;
        foreach (SimpleRotation cube in FindObjectsOfType<SimpleRotation>())
        {
            cube.Allow(_randomActions);
        }
    }

    public void StartRecordingActions()
    {
        
    }
}