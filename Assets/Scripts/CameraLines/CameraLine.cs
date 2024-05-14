using UnityEngine;
using UnityEngine.EventSystems;

public class CameraLine : MonoBehaviour,
    IPointerClickHandler
{
    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public UIHighlighter highlighter;

    private CameraLinesManager _cameraLinesManager;
    [HideInInspector] public CameraLineDivider leftDivider, rightDivider;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        highlighter = gameObject.AddComponent<UIHighlighter>();
        _cameraLinesManager = FindObjectOfType<CameraLinesManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_cameraLinesManager.GetCurrentTool() == CameraLinesTool.Cut)
        {
            CutCameraLine(eventData);
        }
    }

    void CutCameraLine(PointerEventData eventData)
    {
        Vector2 localPoint = new Vector2();
        RectTransform parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform,
            eventData.position,
            eventData.pressEventCamera, out localPoint);
        float anchorX = (localPoint.x + parentRectTransform.rect.width / 2) / parentRectTransform.rect.width;
        _cameraLinesManager.SplitLine(this, anchorX);
    }
}