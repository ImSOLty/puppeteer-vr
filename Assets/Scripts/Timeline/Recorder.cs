using System.Collections;
using UnityEngine;
using UTJ.FrameCapturer;

public class Recorder : MonoBehaviour
{
    private CameraTimeline _cameraTimeline;
    private ObjectTimeline _objectTimeline;
    private MovieRecorder _movieRecorder;

    private CameraInstance _currentCamera;

    private string _exportPath;

    private void Start()
    {
        Setup();
    }

    public void PerformRecording()
    {
        Debug.Log("Started Recording");
        StartCoroutine(Capture());
    }

    void Setup()
    {
        _cameraTimeline = FindObjectOfType<CameraTimeline>();
        _objectTimeline = FindObjectOfType<ObjectTimeline>();
        _movieRecorder = FindObjectOfType<MovieRecorder>();
    }


    public void SetExportPath(string path)
    {
        _movieRecorder.outputDir = new DataPath(path);
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
}