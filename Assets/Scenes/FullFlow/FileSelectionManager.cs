using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using UnityEngine.UI;
using System;

public class FileSelectionManager : MonoBehaviour
{
    private RectTransform selfRectTransform;
    void Awake()
    {
        selfRectTransform = GetComponent<RectTransform>();
    }

    public void SetFilters(FileBrowser.Filter filter)
    {
        FileBrowser.SetFilters(false, filter);
    }

    public void SetupCanvasAfterInit()
    {
        FileBrowser browser = FindObjectOfType<FileBrowser>();
        RectTransform browserRectTransform = browser.GetComponent<RectTransform>();

        browser.transform.SetParent(transform.parent);
        browserRectTransform.localPosition = selfRectTransform.localPosition;
        browserRectTransform.localRotation = selfRectTransform.localRotation;
        browserRectTransform.localScale = selfRectTransform.localScale;
    }
}
