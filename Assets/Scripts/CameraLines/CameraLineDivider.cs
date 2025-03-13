using UnityEngine;
using UnityEngine.EventSystems;

public class CameraLineDivider : MonoBehaviour
{
    private const float DividerDiameter = 0.002f;

    private CameraLinesManager _cameraLinesManager;
    private CameraSectionDivider _cameraSectionDivider;
    private UIHighlighter _highlighter;
    [HideInInspector] public RectTransform rectTransform;
    private CameraLine _leftCameraLine, _rightCameraLine;
    private AnimationManager _animationManager;

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
        _animationManager = FindObjectOfType<AnimationManager>();
    }


    public void ChangeHighlightSelectedLine(float pos)
    {
        if (_rightCameraLine.highlighter != null)
            _rightCameraLine.highlighter.SetHighlight(pos > _cameraSectionDivider.GetPosition());
        if (_leftCameraLine.highlighter != null)
            _leftCameraLine.highlighter.SetHighlight(pos < _cameraSectionDivider.GetPosition());
    }

    public void Holding(int holdFramePosition, CameraLinesTool tool)
    {
        _highlighter.SetHighlight(true);

        int newFramePosition = ClampPosition(holdFramePosition);

        if (tool == CameraLinesTool.Resize)
        {
            _cameraSectionDivider.SetPosition(newFramePosition);
            _cameraLinesManager.Reposition(this);
        }

        if (tool == CameraLinesTool.Join)
            ChangeHighlightSelectedLine(newFramePosition);
    }

    public void Releasing(int holdFramePosition, CameraLinesTool tool)
    {
        if (tool == CameraLinesTool.Join)
        {
            float position = ClampPosition(holdFramePosition);
            ChangeHighlightSelectedLine(position);
            _cameraLinesManager.JoinLines(this, position > _cameraSectionDivider.GetPosition());
        }

        _highlighter.SetHighlight(false);
        _isJoining = false;
        _isMoving = false;
    }

    public void RepositionSelf()
    {
        float framePosition = _cameraSectionDivider.GetPosition();
        int totalFrames = _cameraLinesManager.animationManager.TotalAnimationFrames;

        rectTransform.anchorMin = new Vector2(framePosition / totalFrames, 0) - Vector2.one * DividerDiameter;
        rectTransform.anchorMax = new Vector2(framePosition / totalFrames, 1) + Vector2.one * DividerDiameter;
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

    private int ClampPosition(int position)
    {
        CameraSectionDivider prevSectionDivider = _cameraSectionDivider.GetLeftCameraSection().GetLeftSectionDivider();
        CameraSectionDivider nextSectionDivider = _cameraSectionDivider.GetRightCameraSection().GetRightSectionDivider();

        float prevDivider = prevSectionDivider?.GetPosition() ?? 0;
        float nextDivider = nextSectionDivider?.GetPosition() ?? _animationManager.TotalAnimationFrames;
        position = (int)Mathf.Clamp(position, prevDivider, nextDivider);
        return position;
    }
}