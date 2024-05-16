using System;
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
    public CameraInstance _cameraInstance; //Musthave set default and single one in scene 
    [HideInInspector] public CameraLineDivider leftDivider, rightDivider;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        highlighter = gameObject.AddComponent<UIHighlighter>();
        _cameraLinesManager = FindObjectOfType<CameraLinesManager>();
    }

    private void Start()
    {
        SetCameraInstance(_cameraInstance);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_cameraLinesManager.GetCurrentTool() == CameraLinesTool.Cut)
        {
            CutCameraLine(eventData);
        }
    }

    void CutCameraLine(PointerEventData eventData)
    {
        Vector2 localPoint = new Vector2();
        RectTransform parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform,
            eventData.position,
            eventData.pressEventCamera, out localPoint);
        float anchorX = (localPoint.x + parentRectTransform.rect.width / 2) / parentRectTransform.rect.width;
        _cameraLinesManager.SplitLine(this, anchorX);
    }

    public void SetCameraInstance(CameraInstance cameraInstance)
    {
        _cameraInstance = cameraInstance;
        _image.color = _cameraInstance.GetCameraData().CameraColor;
    }
}