using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UTJ.FrameCapturer;


public class Recorder : MonoBehaviour
{
    private UnityEvent<int> _tmpEvent = new UnityEvent<int>(); // TODO change this

    private CameraTimeline _cameraTimeline;
    private ObjectTimeline _objectTimeline;
    private MovieRecorder _movieRecorder;

    private CameraInstance _currentCamera;

    private string _exportPath;

    private void Awake()
    {
        _cameraTimeline = FindObjectOfType<CameraTimeline>();
        _objectTimeline = FindObjectOfType<ObjectTimeline>();
        _movieRecorder = FindObjectOfType<MovieRecorder>();
    }

    public void PerformRecording()
    {
        StartCoroutine(RecordActions());
    }

    public void CaptureAndExport()
    {
        StartCoroutine(Capture());
    }


    IEnumerator Capture()
    {
        _movieRecorder.outputDir = new DataPath(AnimationSettings.Path);
        _movieRecorder.enabled = true;
        foreach (CameraSection camera in _cameraTimeline.GetCameraSections())
        {
            if (_currentCamera != null)
                _currentCamera.SetAsRecordingCamera(false);
            _currentCamera = camera.CamInstance;
            if (camera.CamInstance)
            {
                camera.CamInstance.SetAsRecordingCamera(true);
            }

            yield return new WaitForSeconds(camera.End - camera.Start);
        }

        _movieRecorder.EndRecording();
        _movieRecorder.enabled = false;
        if (_currentCamera != null)
            _currentCamera.SetAsRecordingCamera(false);
    }

    IEnumerator RecordActions()
    {
        Debug.Log("Started Recording Actions");

        _objectTimeline.StartRecordingObjects();
        yield return new WaitForSeconds(AnimationSettings.Duration);
        _objectTimeline.StopRecordingObjects();

        Debug.Log($"Ended Recording Actions. Stats: ");

        Dictionary<int, Dictionary<int, ObjectData>> dynamicData;
        ObjectData[] staticData;
        int frames;

        (frames, dynamicData, staticData) = _objectTimeline.GetData();

        Debug.Log($"Static objects: {staticData.Length}/{dynamicData.Count + staticData.Length}," +
                  $" Frames: {frames}");
        RaiseEvent(frames);
    }

    public void RegisterForEvent(UnityAction<int> action)
    {
        _tmpEvent.AddListener(action); // register action to receive the event callback
    }

    private void RaiseEvent(int frames)
    {
        _tmpEvent.Invoke(frames); // raise the event for all listeners
    }
}