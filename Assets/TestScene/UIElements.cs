using UnityEngine;
using UnityEngine.UI;

public class UIElements : MonoBehaviour
{
    [SerializeField] private InputField _inputPath;
    [SerializeField] private Slider _frameSlider;

    private Recorder _recorder;
    private ObjectTimeline _objectTimeline;

    private void Awake()
    {
        _recorder = FindObjectOfType<Recorder>();
        _objectTimeline = FindObjectOfType<ObjectTimeline>();
    }

    public void UpdatePath()
    {
        _recorder.SetExportPath(_inputPath.text);
    }

    public void TurnOnSlider(int from, int to)
    {
        _frameSlider.interactable = true;
        _frameSlider.minValue = from;
        _frameSlider.maxValue = to;
    }

    public void TurnOffSlider()
    {
        _frameSlider.interactable = false;
    }

    public void ChangeFrame()
    {
        _objectTimeline.UpdateObjectsForFrame((int)_frameSlider.value);
    }

    public void ExportAnimation()
    {
        _recorder.CaptureAndExport();
    }
}