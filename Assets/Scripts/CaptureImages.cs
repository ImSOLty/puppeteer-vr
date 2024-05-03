using UnityEngine;
using UnityEngine.UI;
using UTJ.FrameCapturer;

public class CaptureImages : MonoBehaviour
{
    public Button Start, End;
    public InputField input;
    private MovieRecorder _movieRecorder;

    public void StartCapturing()
    {
        SwapActive();
        _movieRecorder = GameObject.FindWithTag("MainCamera").GetComponent<MovieRecorder>();
        _movieRecorder.outputDir = new DataPath(input.text);
        _movieRecorder.BeginRecording();
    }

    public void EndCapturing()
    {
        SwapActive();
        _movieRecorder.EndRecording();
    }

    void SwapActive()
    {
        bool active = Start.interactable;
        Start.interactable = !active;
        End.interactable = active;
    }
}