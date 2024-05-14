using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraLineDivider : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private const float DividerDiameter = 0.002f;

    [HideInInspector] public RectTransform rectTransform;
    private CameraLinesManager _cameraLinesManager;
    [HideInInspector] public CameraLine leftCameraLine, rightCameraLine;
    private float _divisionPosition;

    private bool _isDragged = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        _cameraLinesManager = FindObjectOfType<CameraLinesManager>();
    }


    public void RepositionDivider(float pos)
    {
        _divisionPosition = pos;

        //reposition lines
        leftCameraLine.rectTransform.anchorMax = new Vector2(_divisionPosition, 1);
        rightCameraLine.rectTransform.anchorMin = new Vector2(_divisionPosition, 0);

        //reposition divider
        rectTransform.anchorMin = new Vector2(_divisionPosition, 0) - Vector2.one * DividerDiameter;
        rectTransform.anchorMax = new Vector2(_divisionPosition, 1) + Vector2.one * DividerDiameter;
    }

    private float getPositionByEventData(PointerEventData eventData)
    {
        Vector2 localPoint = new Vector2();
        RectTransform parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform,
            eventData.position,
            eventData.pressEventCamera, out localPoint);
        float anchorX = (localPoint.x + parentRectTransform.rect.width / 2) / parentRectTransform.rect.width;
        return anchorX;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragged = _cameraLinesManager.GetCurrentTool() == CameraLinesTool.Resize;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragged)
            RepositionDivider(getPositionByEventData(eventData));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragged = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}