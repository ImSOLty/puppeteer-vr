using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class CameraLine : MonoBehaviour
{
    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public UIHighlighter highlighter;

    private CameraLinesManager _cameraLinesManager;
    private Image _image;
    private CameraSection _cameraSection;


    private void Awake()
    {
        GetComponents();
    }

    void GetComponents()
    {
        rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        highlighter = gameObject.AddComponent<UIHighlighter>();
        _cameraLinesManager = FindObjectOfType<CameraLinesManager>();
    }

    public void SetSection(CameraSection section)
    {
        _cameraSection = section;
        _image.color = _cameraSection.GetCameraInstance().GetCameraData().CameraColor;
    }

    public void RepositionSelf()
    {
        CameraSectionDivider leftDivider = _cameraSection.GetLeftSectionDivider();
        CameraSectionDivider rightDivider = _cameraSection.GetRightSectionDivider();

        int totalFrames = Settings.Animation.TotalFrames();

        float leftFramePosition = leftDivider?.GetPosition() ?? 0;
        float rightFramePosition = rightDivider?.GetPosition() ?? totalFrames;

        rectTransform.anchorMin = new Vector2(leftFramePosition / totalFrames, 0);
        rectTransform.anchorMax = new Vector2(rightFramePosition / totalFrames, 1);
    }

    public void Redraw()
    {
        RepositionSelf();
        _image.color = _cameraSection.GetCameraInstance().GetCameraData().CameraColor;
    }

    public CameraSection GetSection()
    {
        return _cameraSection;
    }
}