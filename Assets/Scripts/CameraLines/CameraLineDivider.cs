using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraLineDivider : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private const float DividerDiameter = 0.002f;


    private CameraLinesManager _cameraLinesManager;
    private UIHighlighter _highlighter;
    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public CameraLine leftCameraLine, rightCameraLine;
    private float _divisionPosition;

    private bool _isMoving = false, _isJoining = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        _highlighter = gameObject.AddComponent<UIHighlighter>();
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

    public void ChangeHighlightSelectedLine(float pos)
    {
        if (rightCameraLine.highlighter != null)
            rightCameraLine.highlighter.SetHighlight(pos > _divisionPosition);
        if (leftCameraLine.highlighter != null)
            leftCameraLine.highlighter.SetHighlight(pos < _divisionPosition);
    }

    private float GetPositionByEventData(PointerEventData eventData)
    {
        Vector2 localPoint = new Vector2();
        RectTransform parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform,
            eventData.position,
            eventData.pressEventCamera, out localPoint);
        float anchorX = (localPoint.x + parentRectTransform.rect.width / 2) / parentRectTransform.rect.width;

        // Clamp Divider between the line
        float prevDivider = leftCameraLine.leftDivider ? leftCameraLine.leftDivider.GetDivisionPosition() : 0;
        float nextDivider = rightCameraLine.rightDivider ? rightCameraLine.rightDivider.GetDivisionPosition() : 1;
        anchorX = Mathf.Clamp(anchorX, prevDivider + DividerDiameter, nextDivider - DividerDiameter);
        return anchorX;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CameraLinesTool tool = _cameraLinesManager.GetCurrentTool();
        if (tool != CameraLinesTool.Resize && tool != CameraLinesTool.Join)
            return;
        _highlighter.SetHighlight(true);
        _isMoving = _cameraLinesManager.GetCurrentTool() == CameraLinesTool.Resize;
        _isJoining = _cameraLinesManager.GetCurrentTool() == CameraLinesTool.Join;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float position = GetPositionByEventData(eventData);
        if (_isMoving)
            RepositionDivider(position);
        if (_isJoining)
            ChangeHighlightSelectedLine(position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isJoining)
        {
            float position = GetPositionByEventData(eventData);
            ChangeHighlightSelectedLine(position);
            _cameraLinesManager.JoinLines(this, position > _divisionPosition);
        }

        _highlighter.SetHighlight(false);
        _isJoining = false;
        _isMoving = false;
    }

    public float GetDivisionPosition()
    {
        return _divisionPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }
}