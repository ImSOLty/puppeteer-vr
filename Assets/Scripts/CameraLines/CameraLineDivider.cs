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

    private bool _isMoving = false, _isJoining = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        _cameraLinesManager = FindObjectOfType<CameraLinesManager>();
    }

    public void RepositionDivider()
    {
        //reposition lines
        leftCameraLine.rectTransform.anchorMax = new Vector2(_divisionPosition, 1);
        rightCameraLine.rectTransform.anchorMin = new Vector2(_divisionPosition, 0);

        //reposition divider
        rectTransform.anchorMin = new Vector2(_divisionPosition, 0) - Vector2.one * DividerDiameter;
        rectTransform.anchorMax = new Vector2(_divisionPosition, 1) + Vector2.one * DividerDiameter;
    }

    public void RepositionDivider(float pos)
    {
        _divisionPosition = pos;
        RepositionDivider();
    }

    public void ChangeHighlightSelectedLine(float pos, bool on)
    {
        UIHighlighter rightHighlighter = rightCameraLine ? rightCameraLine.GetComponent<UIHighlighter>() : null;
        UIHighlighter leftHighlighter = leftCameraLine ? leftCameraLine.GetComponent<UIHighlighter>() : null;
        if (rightHighlighter != null)
            rightHighlighter.SetHighlight(pos > _divisionPosition);
        if (leftHighlighter != null)
            leftHighlighter.SetHighlight(pos < _divisionPosition);
    }

    private float GetPositionByEventData(PointerEventData eventData)
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
        _isMoving = _cameraLinesManager.GetCurrentTool() == CameraLinesTool.Resize;
        _isJoining = _cameraLinesManager.GetCurrentTool() == CameraLinesTool.Join;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float position = GetPositionByEventData(eventData);
        if (_isMoving)
            RepositionDivider(position);
        if (_isJoining)
            ChangeHighlightSelectedLine(position, true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isJoining)
        {
            float position = GetPositionByEventData(eventData);
            ChangeHighlightSelectedLine(position, false);
            _cameraLinesManager.JoinLines(this, position > _divisionPosition);
        }

        _isMoving = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }
}