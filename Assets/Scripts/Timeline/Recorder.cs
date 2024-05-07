using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTJ.FrameCapturer;

public class Recorder : MonoBehaviour
{
    private float _duration = 0;
    private CameraTimeline _cameraTimeline;
    private ObjectTimeline _objectTimeline;
    [SerializeField] MovieRecorder _movieRecorder;

    private CameraInstance _currentCamera;

    private string _exportPath;

    private void Start()
    {
        _cameraTimeline = FindObjectOfType<CameraTimeline>();
        _objectTimeline = FindObjectOfType<ObjectTimeline>();
        _movieRecorder = FindObjectOfType<MovieRecorder>();
    }

    public void PerformRecording()
    {
        StartCoroutine(RecordActions());
    }


    public void SetExportPath(string path)
    {
        _movieRecorder.outputDir = new DataPath(path);
    }

    public void CaptureAndExport()
    {
        StartCoroutine(Capture());
    }

    public void SetDuration(float seconds)
    {
        _duration = seconds;
        _cameraTimeline.SetDuration(_duration);
    }


    IEnumerator Capture()
    {
        _movieRecorder.enabled = true;
        _movieRecorder.BeginRecording();
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
        yield return new WaitForSeconds(_duration);
        _objectTimeline.StopRecordingObjects();
        Debug.Log($"Ended Recording Actions. Stats: ");
        Dictionary<int, ObjectData[]> dynamicData;
        ObjectData[] staticData;
        (dynamicData, staticData) = _objectTimeline.GetData();
        Debug.Log($"Static objects: {staticData.Length}/{dynamicData[1].Length}, Frames: {dynamicData.Count}");
    }
}