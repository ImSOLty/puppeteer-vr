using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSimulator : MonoBehaviour
{
    private CameraManager _cameraManager;
    private List<GameObject> _cameraManipulators;
    private List<CameraInstance> _cameraInstances;

    private CameraTimeline _cameraTimeline;
    private Recorder _recorder;

    private bool _randomActions = false;

    private float _duration = 60;

    void Awake()
    {
        GetComponents();
    }

    private void Start()
    {
        _recorder.SetDuration(_duration);
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

        float sectionDuration = _duration / _cameraInstances.Count;

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
        _recorder.PerformRecording();
    }
}