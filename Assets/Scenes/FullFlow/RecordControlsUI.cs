using UnityEngine;
using UnityEngine.UI;

public class RecordControlsUI : MonoBehaviour
{
    [SerializeField] private Slider frameSlider;
    [SerializeField] private RawImage frameImage;
    private AspectRatioFitter _frameAspectRatioFitter;
    private AnimationManager _animationManager;
    private CameraTimeline _cameraTimeline;

    private void Start()
    {
        _cameraTimeline = FindObjectOfType<CameraTimeline>();
        _animationManager = FindObjectOfType<AnimationManager>();
        _frameAspectRatioFitter = frameImage.GetComponent<AspectRatioFitter>();
        UpdateSlider();
    }
    void UpdateSlider()
    {
        frameSlider.maxValue = _animationManager.TotalAnimationFrames;
        frameSlider.interactable = true;
        ChangeSliderValue();
        _cameraTimeline.RegisterForTimelineUpdated(ChangeSliderValue);
    }

    public void ChangeSliderValue()
    {
        _animationManager.SetupForFrame((int)frameSlider.value);
        UpdateFrameImage();
    }

    void UpdateFrameImage()
    {
        CameraSection section = _cameraTimeline.GetCameraLineForFrame(
            _animationManager.TotalAnimationFrames, (int)frameSlider.value
        ).GetSection();
        CameraInstance instance = section.GetCameraInstance();
        Texture tex = instance.GetTextureFromCamera();
        frameImage.texture = tex;
        _frameAspectRatioFitter.aspectRatio = (float)tex.width / tex.height;
    }
}