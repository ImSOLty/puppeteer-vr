using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using UnityEngine.UI;

public class FileSelectionManager : MonoBehaviour
{
    private Canvas selfCanvas;
    private CanvasScaler selfCanvasScaler;
    private GraphicRaycaster selfGraphicRaycaster;
    public void GetFilePath()
    {
        FileBrowser.ShowLoadDialog((paths) => { Debug.Log("Selected: " + paths[0]); },
                                   () => { Debug.Log("Canceled"); },
                                   FileBrowser.PickMode.Files, false, null, null, "Select File", "Select");
    }

    public void SetupCanvasAfterInit()
    {
        FindObjectOfType<FileBrowser>().GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
    }
}
