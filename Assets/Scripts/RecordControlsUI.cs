using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RecordControlsUI : MonoBehaviour
{
    [SerializeField] private Slider frameSlider;
    [SerializeField] private RawImage frameImage, loadBarWindow;
    [SerializeField] private Image loadBarImage;
    [SerializeField] private Text loadBarText;
    [SerializeField] private Selectable[] uiElements;
    private AspectRatioFitter _frameAspectRatioFitter;
    private AnimationManager _animationManager;
    private CameraTimeline _cameraTimeline;

    private UnityAction updateSliderAction;

    private void Start()
    {
        _cameraTimeline = FindObjectOfType<CameraTimeline>();
        _animationManager = FindObjectOfType<AnimationManager>();
        _frameAspectRatioFitter = frameImage.GetComponent<AspectRatioFitter>();
        UpdateSliderFirstTime();
    }
    private void UpdateSliderFirstTime()
    {
        frameSlider.maxValue = Settings.Animation.TotalFrames();
        frameSlider.interactable = true;
        updateSliderAction += ChangeSliderValue;
        _cameraTimeline.RegisterForTimelineUpdated(updateSliderAction);
        ChangeSliderValue();
    }

    public void ChangeSliderValue()
    {
        _animationManager.SetupForFrame((int)frameSlider.value);
        UpdateFrameImage();
    }

    void UpdateFrameImage()
    {
        CameraLine _cameraLine = _cameraTimeline.GetCameraLineForFrame((int)frameSlider.value);
        if (!_cameraLine)
        {
            return;
        }
        CameraSection section = _cameraLine.GetSection();
        CameraInstance instance = section.GetCameraInstance();
        Texture tex = instance.GetTextureFromCamera();
        frameImage.texture = tex;
        _frameAspectRatioFitter.aspectRatio = (float)tex.width / tex.height;
    }

    public void Export()
    {
        StartCoroutine(ExportProcess());
    }

    IEnumerator ExportProcess()
    {
        // PreExport
        frameImage.transform.parent.gameObject.SetActive(false);
        loadBarWindow.transform.parent.gameObject.SetActive(true);
        loadBarText.text = "0%";
        loadBarImage.fillAmount = 0;
        foreach (Selectable selectable in uiElements)
        {
            selectable.interactable = false;
        }

        // Export
        ActualRecorder recorder = FindObjectOfType<ActualRecorder>();
        recorder.StartRecord();
        for (int frame = 0; frame < Settings.Animation.TotalFrames(); frame++)
        {
            float completed = (float)frame / Settings.Animation.TotalFrames();
            loadBarText.text = Mathf.FloorToInt(completed * 100).ToString() + "%";
            loadBarImage.fillAmount = completed;

            _animationManager.SetupForFrame(frame);
            CameraSection section = _cameraTimeline.GetCameraLineForFrame(frame).GetSection();
            yield return new WaitUntil(() => recorder.ReadyToAddFrame());
            recorder.AddFrame(section.GetCameraInstance().GetTextureFromCamera());
        }
        recorder.EndRecord();

        // PostExport
        frameImage.transform.parent.gameObject.SetActive(true);
        loadBarWindow.transform.parent.gameObject.SetActive(false);
        UpdateFrameImage();
        foreach (Selectable selectable in uiElements)
        {
            selectable.interactable = true;
        }
    }
}