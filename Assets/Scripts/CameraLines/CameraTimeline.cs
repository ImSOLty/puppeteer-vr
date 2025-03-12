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

    public CameraLine GetCameraLineForFrame(int totalFrames, int currentFrame)
    {
        float endValue = (float)currentFrame / totalFrames;
        return GetCameraLineForPercent(endValue);
    }
    public CameraLine GetCameraLineForPercent(float percent)
    {
        float endValue = percent;
        CameraSection tmpCameraSection = _leftmostCameraSection;

        while (tmpCameraSection.GetRightSectionDivider() != null &&
               tmpCameraSection.GetRightSectionDivider().GetPosition() < endValue)
        {
            tmpCameraSection = tmpCameraSection.GetRightSectionDivider().GetRightCameraSection();
        }

        return tmpCameraSection.GetLine();
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