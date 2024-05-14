using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

        //TODO Change color setting
        newCameraLine.GetComponent<Image>().color = Random.ColorHSV();

        // Connect both camera lines via divider
        divider.leftCameraLine = cameraLine;
        divider.rightCameraLine = newCameraLine;

        newCameraLine.rightDivider = cameraLine.rightDivider;
        newCameraLine.leftDivider = divider;
        cameraLine.rightDivider = divider;

        // Set dividers position
        if (newCameraLine.rightDivider != null)
        {
            newCameraLine.rightDivider.leftCameraLine = newCameraLine;
            newCameraLine.rightDivider.RepositionDivider();
        }

        divider.RepositionDivider(anchorX);
    }


    public CameraLinesTool GetCurrentTool()
    {
        if (tools.selectedOption is null || !Enum.TryParse(tools.selectedOption.name, out CameraLinesTool selected))
        {
            return CameraLinesTool.NotExisting;
        }

        return selected;
    }

    public void JoinLines(CameraLineDivider divider, bool removeRight = true)
    {
        if (removeRight)
        {
            divider.leftCameraLine.rightDivider = divider.rightCameraLine.rightDivider;
            if (divider.leftCameraLine.rightDivider != null)
            {
                divider.leftCameraLine.rightDivider.leftCameraLine = divider.leftCameraLine;
                divider.leftCameraLine.rightDivider.RepositionDivider();
            }
            else
            {
                divider.leftCameraLine.rectTransform.anchorMax = Vector2.one;
            }

            Destroy(divider.rightCameraLine.gameObject);
        }
        else
        {
            divider.rightCameraLine.leftDivider = divider.leftCameraLine.leftDivider;
            if (divider.rightCameraLine.leftDivider != null)
            {
                divider.rightCameraLine.leftDivider.rightCameraLine = divider.rightCameraLine;
                divider.rightCameraLine.leftDivider.RepositionDivider();
            }
            else
            {
                divider.rightCameraLine.rectTransform.anchorMin = Vector2.zero;
            }

            Destroy(divider.leftCameraLine.gameObject);
        }

        Destroy(divider.gameObject);
    }
}