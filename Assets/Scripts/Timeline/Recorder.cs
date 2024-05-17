using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UTJ.FrameCapturer;


public class Recorder : MonoBehaviour
{
    private UnityEvent<int> _finishedRecordingEvent = new UnityEvent<int>();

    private CameraTimeline _cameraTimeline;
    private ObjectTimeline _objectTimeline;
    private MovieRecorder _movieRecorder;

    private CameraInstance _currentCamera;

    private string _exportPath;

    private int frames;

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

        _movieRecorder.BeginRecording();
        StartCoroutine(_objectTimeline.MoveObjectsForXFrames(frames));
        foreach (CameraSection section in _cameraTimeline.GetCameraSections())
        {
            if (_currentCamera)
                _currentCamera.SetAsRecordingCamera(false);

            _currentCamera = section.GetCameraInstance();
            if (_currentCamera)
            {
                _currentCamera.SetAsRecordingCamera(true);
            }

            CameraSectionDivider left = section.GetLeftSectionDivider();
            CameraSectionDivider right = section.GetRightSectionDivider();
            float start = left?.GetPosition() ?? 0;
            float end = right?.GetPosition() ?? 1;

            yield return
                new WaitForSeconds((end - start) * AnimationSettings.Duration); //TODO: problem with convertance
        }

        Debug.Log("Finished");
        _movieRecorder.EndRecording();
        _movieRecorder.enabled = false;
        if (_currentCamera)
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

        (frames, dynamicData, staticData) = _objectTimeline.GetData();

        Debug.Log($"Static objects: {staticData.Length}/{dynamicData.Count + staticData.Length}," +
                  $" Frames: {frames}");
        _finishedRecordingEvent.Invoke(frames); // raise the event for all listeners
    }

    public void RegisterForRecordingFinished(UnityAction<int> action)
    {
        _finishedRecordingEvent.AddListener(action); // register action to receive the event callback
    }
}