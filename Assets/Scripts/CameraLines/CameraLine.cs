using System;
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
    public CameraInstance _cameraInstance; //Musthave set default and single one in scene 
    [HideInInspector] public CameraLineDivider leftDivider, rightDivider;


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

    private void Start()
    {
        SetCameraInstance(_cameraInstance);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CameraLinesTool tool = _cameraLinesManager.GetCurrentTool();
        if (tool == CameraLinesTool.Cut)
        {
            CutCameraLine(eventData);
        }

        if (tool == CameraLinesTool.Switch)
        {
            LinkedList<CameraInstance> instances = _cameraLinesManager.cameraManager.GetCameraInstances();
            LinkedListNode<CameraInstance> current = instances.Find(_cameraInstance);
            SetCameraInstance((current != null) && (current.Next != null)
                ? current.Next.Value
                : instances.First.Value);
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