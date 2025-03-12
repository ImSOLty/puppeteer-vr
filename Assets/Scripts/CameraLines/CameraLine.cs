using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class CameraLine : UICustomReactiveElement
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

    public override void OnPointerClick(PointerEventArgs eventData)
    {
        CameraLinesTool tool = _cameraLinesManager.GetCurrentTool();
        if (tool == CameraLinesTool.Cut)
        {
            // Define where cut was performed
            float percentX = (eventData.hit.point.x - eventData.hit.collider.bounds.min.x) / eventData.hit.collider.bounds.size.x;
            _cameraLinesManager.Cut(percentX);
        }

        if (tool == CameraLinesTool.Switch)
        {
            _cameraLinesManager.Switch(this);
            _image.color = _cameraSection.GetCameraInstance().GetCameraData().CameraColor;
        }
    }

    public void SetSection(CameraSection section)
    {
        _cameraSection = section;
        Debug.Log(_cameraSection);
        Debug.Log(_cameraSection.GetCameraInstance());
        Debug.Log(_cameraSection.GetCameraInstance().GetCameraData());
        _image.color = _cameraSection.GetCameraInstance().GetCameraData().CameraColor;
    }

    public void RepositionSelf()
    {
        CameraSectionDivider leftDivider = _cameraSection.GetLeftSectionDivider();
        CameraSectionDivider rightDivider = _cameraSection.GetRightSectionDivider();

        float left = leftDivider?.GetPosition() ?? 0;
        float right = rightDivider?.GetPosition() ?? 1;

        rectTransform.anchorMin = new Vector2(left, 0);
        rectTransform.anchorMax = new Vector2(right, 1);
    }

    public CameraSection GetSection()
    {
        return _cameraSection;
    }
}