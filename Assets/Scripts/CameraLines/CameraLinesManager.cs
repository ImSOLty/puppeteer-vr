using System;
using UnityEngine;

public enum CameraLinesTool
{
    NotExisting,
    Cut,
    Switch,
    Resize,
    Join,
    Select
}

public class CameraLinesManager : MonoBehaviour
{
    [SerializeField] private GameObject cameraLinePrefab;
    [SerializeField] private GameObject lineDividerPrefab;
    [SerializeField] private Transform linesContainer, dividersContainer;

    [SerializeField] private RadioSelector tools;

    public void SplitLine(CameraLine cameraLine, float anchorX)
    {
        // Create new camera line and divider
        CameraLineDivider divider =
            Instantiate(lineDividerPrefab, parent: dividersContainer).GetComponent<CameraLineDivider>();
        CameraLine newCameraLine = Instantiate(cameraLinePrefab, parent: linesContainer).GetComponent<CameraLine>();

        // Connect both camera lines via divider
        divider.leftCameraLine = cameraLine;
        divider.rightCameraLine = newCameraLine;
        cameraLine.leftDivider = divider;
        cameraLine.rightDivider = divider;

        // Set divider position
        divider.RepositionDivider(anchorX);
    }
    
    public void JoinLines(CameraLineDivider divider)
    {
    }
    
    

    public CameraLinesTool GetCurrentTool()
    {
        if (tools.selectedOption is null || !Enum.TryParse(tools.selectedOption.name, out CameraLinesTool selected))
        {
            return CameraLinesTool.NotExisting;
        }

        return selected;
    }
}