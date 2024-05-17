using UnityEngine;
using UnityEngine.EventSystems;

public class CameraLineDivider : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private const float DividerDiameter = 0.002f;

    private CameraLinesManager _cameraLinesManager;
    private CameraSectionDivider _cameraSectionDivider;
    private UIHighlighter _highlighter;
    [HideInInspector] public RectTransform rectTransform;
    private CameraLine _leftCameraLine, _rightCameraLine;

    private bool _isMoving = false, _isJoining = false;

    private void Awake()
    {
        GetComponents();
    }

    void GetComponents()
    {
        rectTransform = GetComponent<RectTransform>();
        _highlighter = gameObject.AddComponent<UIHighlighter>();
        _cameraLinesManager = FindObjectOfType<CameraLinesManager>();
    }


    public void ChangeHighlightSelectedLine(float pos)
    {
        if (_rightCameraLine.highlighter != null)
            _rightCameraLine.highlighter.SetHighlight(pos > _cameraSectionDivider.GetPosition());
        if (_leftCameraLine.highlighter != null)
            _leftCameraLine.highlighter.SetHighlight(pos < _cameraSectionDivider.GetPosition());
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
        {
            _cameraSectionDivider.SetPosition(position);
            _cameraLinesManager.Reposition(this);
        }

        if (_isJoining)
            ChangeHighlightSelectedLine(position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isJoining)
        {
            float position = GetPositionByEventData(eventData);
            ChangeHighlightSelectedLine(position);
            _cameraLinesManager.JoinLines(this, position > _cameraSectionDivider.GetPosition());
        }

        _highlighter.SetHighlight(false);
        _isJoining = false;
        _isMoving = false;
    }

    public void RepositionSelf()
    {
        float position = _cameraSectionDivider.GetPosition();
        rectTransform.anchorMin = new Vector2(position, 0) - Vector2.one * DividerDiameter;
        rectTransform.anchorMax = new Vector2(position, 1) + Vector2.one * DividerDiameter;
    }

    public void SetSectionDivider(CameraSectionDivider divider)
    {
        _cameraSectionDivider = divider;
    }

    public CameraSectionDivider GetSectionDivider()
    {
        return _cameraSectionDivider;
    }

    public void SetLeftRightLines(CameraLine left, CameraLine right)
    {
        _leftCameraLine = left;
        _rightCameraLine = right;
    }

    public (CameraLine, CameraLine) GetLeftRightLines()
    {
        return (_cameraSectionDivider.GetLeftCameraSection().GetLine(),
            _cameraSectionDivider.GetRightCameraSection().GetLine());
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

        CameraSectionDivider prevSectionDivider = _cameraSectionDivider.GetLeftCameraSection().GetLeftSectionDivider();
        CameraSectionDivider nextSectionDivider =
            _cameraSectionDivider.GetRightCameraSection().GetRightSectionDivider();

        float prevDivider = prevSectionDivider?.GetPosition() ?? 0;
        float nextDivider = nextSectionDivider?.GetPosition() ?? 1;
        anchorX = Mathf.Clamp(anchorX, prevDivider + DividerDiameter, nextDivider - DividerDiameter);
        return anchorX;
    }
}