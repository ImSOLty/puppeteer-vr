using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraTimeline : MonoBehaviour
{
    private CameraSection _leftmostCameraSection;
    private UnityEvent _timelineUpdated = new UnityEvent();

    void Start()
    {
        _timelineUpdated.AddListener(RedrawTimeline);
    }

    public void RegisterForTimelineUpdated(UnityAction action)
    {
        _timelineUpdated.AddListener(action); // register action to receive the event callback
    }

    public void RaiseTimelineUpdated()
    {
        _timelineUpdated.Invoke(); // register action to receive the event callback
    }

    public CameraLine GetCameraLineForFrame(float endFrame)
    {
        CameraSection tmpCameraSection = _leftmostCameraSection;

        while (tmpCameraSection.GetRightSectionDivider() != null &&
               tmpCameraSection.GetRightSectionDivider().GetPosition() < endFrame)
        {
            tmpCameraSection = tmpCameraSection.GetRightSectionDivider().GetRightCameraSection();
        }

        return tmpCameraSection.GetLine();
    }
    public CameraLine GetCameraLineForPercent(int totalFrames, float percent)
    {
        float endValue = percent * totalFrames;
        return GetCameraLineForFrame(endValue);
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
    public void RedrawTimeline()
    {
        foreach (CameraSection section in GetCameraSections())
        {
            section.GetLine().Redraw();
        }
    }

    public void SetLeftmostCameraSection(CameraSection section)
    {
        _leftmostCameraSection = section;
    }
}