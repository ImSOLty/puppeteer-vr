using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraTimeline : MonoBehaviour
{
    private CameraSection _leftmostCameraSection;
    private UnityEvent _timelineUpdated = new UnityEvent();

    public void RegisterForTimelineUpdated(UnityAction action)
    {
        _timelineUpdated.AddListener(action); // register action to receive the event callback
    }

    public void RaiseTimelineUpdated()
    {
        _timelineUpdated.Invoke(); // register action to receive the event callback
    }

    public CameraSection GetCameraLineForFrame(int totalFrames, int currentFrame)
    {
        float endValue = (float)currentFrame / totalFrames;
        CameraSection tmpCameraLine = _leftmostCameraSection;

        while (tmpCameraLine.GetRightSectionDivider() != null &&
               tmpCameraLine.GetRightSectionDivider().GetPosition() < endValue)
        {
            tmpCameraLine = tmpCameraLine.GetRightSectionDivider().GetRightCameraSection();
        }

        return tmpCameraLine;
    }

    public List<CameraSection> GetCameraSections()
    {
        CameraSection tmpCameraLine = _leftmostCameraSection;

        List<CameraSection> list = new List<CameraSection>() { tmpCameraLine };

        while (tmpCameraLine.GetRightSectionDivider() != null)
        {
            tmpCameraLine = tmpCameraLine.GetRightSectionDivider().GetRightCameraSection();
            list.Add(tmpCameraLine);
        }

        return list;
    }

    public void SetLeftmostCameraSection(CameraSection section)
    {
        _leftmostCameraSection = section;
    }
}