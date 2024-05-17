using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraLine : MonoBehaviour,
    IPointerClickHandler
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

    public void OnPointerClick(PointerEventData eventData)
    {
        CameraLinesTool tool = _cameraLinesManager.GetCurrentTool();
        if (tool == CameraLinesTool.Cut)
        {
            RectTransform parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform,
                eventData.position,
                eventData.pressEventCamera, out var localPoint);

            float anchorX = (localPoint.x + parentRectTransform.rect.width / 2) / parentRectTransform.rect.width;

            _cameraLinesManager.Cut(this, anchorX);
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
        _image.color = _cameraSection.GetCameraInstance().GetCameraData().CameraColor;
    }

    public void RepositionSelf()
    {
        CameraSectionDivider leftDivider = _cameraSection.GetLeftSectionDivider();
        CameraSectionDivider rightDivider = _cameraSection.GetRightSectionDivider();

        float left = leftDivider?.GetPosition() ?? 0;
        float right = rightDivider?.GetPosition() ?? 1;

        Debug.Log(left);
        Debug.Log(right);

        rectTransform.anchorMin = new Vector2(left, 0);
        rectTransform.anchorMax = new Vector2(right, 1);
    }

    public CameraSection GetSection()
    {
        return _cameraSection;
    }
}