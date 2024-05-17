using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ControlUISimulator : MonoBehaviour
{
    [SerializeField] private InputField pathInput, durationInput, fpsInput;
    [SerializeField] private Slider frameSlider;
    [SerializeField] private RawImage frameImage;
    private AspectRatioFitter _frameAspectRatioFitter;

    [SerializeField] private Transform startCanvas, timelineCanvas;

    private Recorder _recorder;
    private ObjectTimeline _objectTimeline;
    private CameraTimeline _cameraTimeline;

    private bool _randomActions;

    private void Awake()
    {
        _recorder = FindObjectOfType<Recorder>();
        _cameraTimeline = FindObjectOfType<CameraTimeline>();
        _objectTimeline = FindObjectOfType<ObjectTimeline>();
        _frameAspectRatioFitter = frameImage.GetComponent<AspectRatioFitter>();
    }

    private void Start()
    {
        _recorder.RegisterForRecordingFinished(UpdateSlider);
    }

    public void SubmitButtonClicked()
    {
        SaveSettings();
        SwitchCanvases();
        Debug.Log(AnimationSettings.SettingsInfo());
    }

    private void SwitchCanvases()
    {
        startCanvas.gameObject.SetActive(false);
        timelineCanvas.gameObject.SetActive(true);
    }

    private void SaveSettings()
    {
        AnimationSettings.Path = pathInput.text;
        AnimationSettings.Duration = int.Parse(durationInput.text);
        AnimationSettings.FPS = int.Parse(fpsInput.text);
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

    void UpdateSlider(int framesCount)
    {
        frameSlider.maxValue = framesCount;
        frameSlider.interactable = true;
        ChangeSliderValue();
        _cameraTimeline.RegisterForTimelineUpdated(ChangeSliderValue);
    }

    public void ChangeSliderValue()
    {
        _objectTimeline.UpdateObjectsForFrame((int)frameSlider.value);
        UpdateFrameImage();
    }

    void UpdateFrameImage()
    {
        CameraSection section = _cameraTimeline.GetCameraLineForFrame(
            (int)frameSlider.maxValue, (int)frameSlider.value
        );
        CameraInstance instance = section.GetCameraInstance();
        Texture tex = instance.GetTextureFromCamera();
        frameImage.texture = tex;
        _frameAspectRatioFitter.aspectRatio = (float)tex.width / tex.height;
    }
}