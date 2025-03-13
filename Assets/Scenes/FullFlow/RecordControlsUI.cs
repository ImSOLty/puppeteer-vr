using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RecordControlsUI : MonoBehaviour
{
    [SerializeField] private Slider frameSlider;
    [SerializeField] private RawImage frameImage;
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
        frameSlider.maxValue = _animationManager.TotalAnimationFrames;
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
        CameraSection section = _cameraTimeline.GetCameraLineForFrame((int)frameSlider.value).GetSection();
        CameraInstance instance = section.GetCameraInstance();
        Texture tex = instance.GetTextureFromCamera();
        frameImage.texture = tex;
        _frameAspectRatioFitter.aspectRatio = (float)tex.width / tex.height;
    }
}