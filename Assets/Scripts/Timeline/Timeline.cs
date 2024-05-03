using UnityEngine;
using UTJ.FrameCapturer;

public class Timeline : MonoBehaviour
{
    private CameraTimeline _cameraTimeline;
    private ObjectTimeline _objectTimeline;
    private MovieRecorder _movieRecorder;

    private string _exportPath;

    private void Start()
    {
        Setup();
    }

    public void PerformRecording()
    {
        Debug.Log("Started Recording");
    }

    void Setup()
    {
        _cameraTimeline = FindObjectOfType<CameraTimeline>();
        _objectTimeline = FindObjectOfType<ObjectTimeline>();
        _movieRecorder = FindObjectOfType<MovieRecorder>();
    }

    public void SetExportPath(string path)
    {
        Debug.Log(path);
        // _movieRecorder.outputDir = new DataPath(path);
    }
}