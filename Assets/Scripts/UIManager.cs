using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas timelineCanvas;
    [SerializeField] private float timelineCanvasDistance;
    [SerializeField] private Transform headTransform;

    private bool _timelineCanvasIsOpen = false;

    public void TimelineCanvasOpenClose(InputAction.CallbackContext ctx)
    {
        _timelineCanvasIsOpen = !_timelineCanvasIsOpen;

        timelineCanvas.enabled = _timelineCanvasIsOpen;

        if (_timelineCanvasIsOpen)
        {
            timelineCanvas.transform.parent.position =
                headTransform.position + headTransform.forward * timelineCanvasDistance;
            timelineCanvas.transform.parent.LookAt(headTransform);
        }
    }
}