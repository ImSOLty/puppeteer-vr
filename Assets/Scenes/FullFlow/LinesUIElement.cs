using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

public class LinesUIElement : UICustomReactiveElement
{
    [SerializeField] CameraLinesManager _cameraLinesManager;
    [SerializeField] CameraTimeline _cameraTimeline;
    [SerializeField] AnimationManager _animationManager;
    public override void OnPointerClick(PointerEventArgs eventData)
    {
        CameraLinesTool tool = _cameraLinesManager.GetCurrentTool();
        float percentX = (eventData.hit.point.x - eventData.hit.collider.bounds.min.x) / eventData.hit.collider.bounds.size.x;

        if (tool == CameraLinesTool.Cut)
        {
            // Define where cut was performed
            _cameraLinesManager.Cut(percentX);
        }

        if (tool == CameraLinesTool.Switch)
        {
            _cameraLinesManager.Switch(_cameraTimeline.GetCameraLineForPercent(_animationManager.TotalAnimationFrames, percentX));
        }
    }
}
