using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

public class LinesUIElement : UICustomReactiveElement
{
    [SerializeField] CameraLinesManager _cameraLinesManager;
    [SerializeField] CameraTimeline _cameraTimeline;
    AnimationManager _animationManager;

    private CameraLineDivider _chosenDivider = null;

    void Awake()
    {
        _animationManager = FindObjectOfType<AnimationManager>();
    }
    public override void OnPointerClick(PointerEventArgs eventData)
    {
        CameraLinesTool tool = _cameraLinesManager.GetCurrentTool();
        float percentX = GetPercentXByHit(eventData.hit);

        if (tool == CameraLinesTool.Cut)
        {
            // Define where cut was performed
            _cameraLinesManager.Cut(percentX);
        }

        if (tool == CameraLinesTool.Switch)
        {
            _cameraLinesManager.Switch(_cameraTimeline.GetCameraLineForPercent(Settings.Animation.TotalFrames(), percentX));
        }
    }
    public override void OnPointerHold(PointerEventArgs eventData)
    {
        CameraLinesTool tool = _cameraLinesManager.GetCurrentTool();

        if (tool != CameraLinesTool.Join && tool != CameraLinesTool.Resize) { return; }

        float percentX = GetPercentXByHit(eventData.hit);
        if (_chosenDivider == null)
        {
            _chosenDivider = _cameraTimeline.GetNearestDividerForPercent(Settings.Animation.TotalFrames(), percentX);
        }
        if (_chosenDivider == null)
        {
            return;
        }

        int frame = (int)(Settings.Animation.TotalFrames() * percentX);
        _chosenDivider.Holding(frame, tool);
    }
    public override void OnPointerRelease(PointerEventArgs eventData)
    {
        if (_chosenDivider == null)
        {
            return;
        }
        CameraLinesTool tool = _cameraLinesManager.GetCurrentTool();

        if (tool != CameraLinesTool.Join && tool != CameraLinesTool.Resize) { return; }

        float percentX = GetPercentXByHit(eventData.hit);
        int frame = (int)(Settings.Animation.TotalFrames() * percentX);
        _chosenDivider.Releasing(frame, tool);
        _chosenDivider = null;
    }

    float GetPercentXByHit(RaycastHit hit)
    {
        return (hit.point.x - hit.collider.bounds.min.x) / hit.collider.bounds.size.x;
    }
}
