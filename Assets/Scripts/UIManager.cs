using System.Collections;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas _timelineCanvas;
    private Transform _timelineParent;

    [SerializeField] private Canvas _fileBrowserCanvas;
    private Transform _fileBrowserParent;

    [SerializeField] private float canvasDistance;
    [SerializeField] private Transform headTransform;

    private bool _timelineCanvasIsOpen = false;

    public void TimelineCanvasOpenClose(InputAction.CallbackContext ctx)
    {
        _timelineCanvasIsOpen = !_timelineCanvasIsOpen;

        _timelineCanvas.enabled = _timelineCanvasIsOpen;

        if (_timelineCanvasIsOpen)
        {
            _timelineCanvas.transform.parent.position =
                headTransform.position + headTransform.forward * canvasDistance;
            _timelineCanvas.transform.parent.LookAt(headTransform);
        }
    }

    public string FileBrowserOpen()
    {
        StartCoroutine(ShowChooseDirectoryDialog());
        return "";
    }

    IEnumerator ShowChooseDirectoryDialog()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Folders, false, null, null, "Choose Directory",
            "Choose");
        Debug.Log(FileBrowser.Success);
        Debug.Log(FileBrowser.Result[0]);
    }
}