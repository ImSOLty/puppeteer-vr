using UnityEngine;
using UnityEngine.UI;

public class ControlUISimulator : MonoBehaviour
{
    [SerializeField] private InputField pathInput, durationInput, fpsInput;
    [SerializeField] private Slider frameSlider;

    [SerializeField] private Transform startCanvas, timelineCanvas, controlCanvas;

    private Recorder _recorder;
    private ObjectTimeline _objectTimeline;

    private bool _randomActions;

    private void Awake()
    {
        _recorder = FindObjectOfType<Recorder>();
        _objectTimeline = FindObjectOfType<ObjectTimeline>();
    }

    public void SubmitButtonClicked()
    {
        SaveSettings();
        SwitchCanvases();
        Debug.Log(AnimationSettings.SettingsInfo());
    }

    public void SwitchCanvases()
    {
        startCanvas.gameObject.SetActive(false);
        timelineCanvas.gameObject.SetActive(true);
        controlCanvas.gameObject.SetActive(true);
    }

    public void SaveSettings()
    {
        AnimationSettings.Path = pathInput.text;
        AnimationSettings.Duration = int.Parse(durationInput.text);
        AnimationSettings.FPS = int.Parse(fpsInput.text);
    }

    public void ChangeFrame()
    {
        _objectTimeline.UpdateObjectsForFrame((int)frameSlider.value);
    }

    public void ExportAnimation()
    {
        _recorder.CaptureAndExport();
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