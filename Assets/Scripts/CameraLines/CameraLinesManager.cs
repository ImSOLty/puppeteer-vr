using System;
using System.Collections.Generic;
using UnityEngine;

public enum CameraLinesTool
{
    NotExisting,
    Cut,
    Switch,
    Resize,
    Join
}

public class CameraLinesManager : MonoBehaviour
{
    [SerializeField] private GameObject cameraLinePrefab, lineDividerPrefab;
    [SerializeField] private Transform linesContainer, dividersContainer;
    [SerializeField] private RadioSelector tools;

    private CameraTimeline _cameraTimeline;
    private CameraManager _cameraManager;

    [SerializeField] private CameraLine firstCameraLine;
    private CameraInstance firstCameraInstance;

    private List<CameraLine> _cameraLines;
    private List<CameraLineDivider> _cameraLineDividers = new();

    public AnimationManager animationManager;

    private void Awake()
    {
        _cameraTimeline = FindObjectOfType<CameraTimeline>();
        _cameraManager = FindObjectOfType<CameraManager>();
        animationManager = FindObjectOfType<AnimationManager>();
    }

    private void Start()
    {
        firstCameraInstance = FindObjectOfType<CameraInstance>(); // Select random camera as first

        _cameraLines = new List<CameraLine> { firstCameraLine };

        firstCameraLine.SetSection(new CameraSection(firstCameraInstance));
        firstCameraLine.GetSection().SetLine(firstCameraLine);
        _cameraTimeline.SetLeftmostCameraSection(firstCameraLine.GetSection());
    }

    public void Cut(float cutPercentsFromStart)
    {
        int frameOnCut = (int)(Settings.Animation.TotalFrames() * cutPercentsFromStart);

        CameraLine cuttedCameraLine = _cameraTimeline.GetCameraLineForFrame(frameOnCut);
        CameraSection nextSection;
        CameraSectionDivider nextDivider;

        (nextSection, nextDivider) = cuttedCameraLine.GetSection().SplitSectionAndReturn(frameOnCut);

        CameraLine newCameraLine = Instantiate(cameraLinePrefab, parent: linesContainer).GetComponent<CameraLine>();
        newCameraLine.SetSection(nextSection);
        nextSection.SetLine(newCameraLine);

        CameraLineDivider newDivider =
            Instantiate(lineDividerPrefab, parent: dividersContainer).GetComponent<CameraLineDivider>();
        nextDivider.SetLineDivider(newDivider);

        _cameraLines.Add(newCameraLine);
        _cameraLineDividers.Add(newDivider);

        newDivider.SetSectionDivider(nextDivider);
        newDivider.SetLeftRightLines(cuttedCameraLine, newCameraLine);

        Reposition(newDivider);
    }

    public void Switch(CameraLine cameraLine)
    {
        CameraSection sectionToSwitch = cameraLine.GetSection();
        LinkedList<CameraInstance> instances = _cameraManager.GetCameraInstances();
        LinkedListNode<CameraInstance> current = instances.Find(sectionToSwitch.GetCameraInstance());
        sectionToSwitch.SetCameraInstance((current != null) && (current.Next != null)
            ? current.Next.Value
            : instances.First.Value);
        _cameraTimeline.RaiseTimelineUpdated();
    }


    public CameraLinesTool GetCurrentTool()
    {
        if (tools.selectedOption is null || !Enum.TryParse(tools.selectedOption.name, out CameraLinesTool selected))
        {
            return CameraLinesTool.NotExisting;
        }

        return selected;
    }

    public void Reposition(CameraLineDivider divider)
    {
        CameraSectionDivider sectionDivider = divider.GetSectionDivider();

        sectionDivider.GetLineDivider().RepositionSelf();
        sectionDivider.GetLeftCameraSection().GetLine().RepositionSelf();
        sectionDivider.GetRightCameraSection().GetLine().RepositionSelf();

        _cameraTimeline.RaiseTimelineUpdated();
    }

    public void JoinLines(CameraLineDivider divider, bool removeRight = true)
    {
        CameraLine leftCameraLine, rightCameraLine;
        (leftCameraLine, rightCameraLine) = divider.GetLeftRightLines();

        CameraSectionDivider sectionDivider = divider.GetSectionDivider();

        CameraSection leftSection = sectionDivider.GetLeftCameraSection();
        CameraSection rightSection = sectionDivider.GetRightCameraSection();
        if (removeRight)
        {
            leftSection.SetRightSectionDivider(rightSection.GetRightSectionDivider());
            if (leftSection.GetRightSectionDivider() != null)
            {
                leftSection.GetRightSectionDivider().SetLeftCameraSection(leftSection);
            }
            else
            {
                leftSection.SetRightSectionDivider(null);
            }

            _cameraLines.Remove(rightCameraLine);
            Destroy(rightCameraLine.gameObject);
        }
        else
        {
            rightSection.SetLeftSectionDivider(leftSection.GetLeftSectionDivider());
            if (rightSection.GetLeftSectionDivider() != null)
            {
                rightSection.GetLeftSectionDivider().SetRightCameraSection(rightSection);
            }
            else
            {
                _cameraTimeline.SetLeftmostCameraSection(rightSection);
                rightSection.SetLeftSectionDivider(null);
            }

            _cameraLines.Remove(leftCameraLine);
            Destroy(leftCameraLine.gameObject);
        }

        _cameraLineDividers.Remove(divider);
        Destroy(divider.gameObject);

        _cameraLines.ForEach(line => line.RepositionSelf());
        _cameraLineDividers.ForEach(div => div.RepositionSelf());
        _cameraTimeline.RaiseTimelineUpdated();
    }
}