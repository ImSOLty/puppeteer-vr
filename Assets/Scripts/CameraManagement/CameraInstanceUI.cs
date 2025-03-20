using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CameraInstanceUI : MonoBehaviour
{
    [SerializeField] private CameraInstance cameraInstance;
    [SerializeField] private Text spoutNameRuntimeText;
    [SerializeField] private Text spoutNameText;
    [SerializeField] private Slider fovSlider;
    [SerializeField] private GameObject propertiesWindow, screenshotWindow, openMeButton;
    private int spoutNameIndex;
    private string spoutName;

    void Start()
    {
        if (Settings.Animation.AnimationMode != Mode.PROPS_MANAGEMENT)
        {
            if (Settings.Animation.AnimationMode == Mode.ANIMATION_RUNTIME)
            {
                spoutNameRuntimeText.gameObject.SetActive(true);
                spoutNameRuntimeText.text = cameraInstance.GetCameraData().Name;
            }
        }
        else
        {
            spoutNameIndex = UnityEngine.Random.Range(0, Settings.Camera.cameraNames.Length);
            UpdateSpoutName();
        }
    }

    public void Open()
    {
        if (Settings.Animation.AnimationMode == Mode.PROPS_MANAGEMENT)
            propertiesWindow.SetActive(true);
        else
            screenshotWindow.SetActive(true);
        openMeButton.SetActive(false);
    }
    public void Close()
    {
        propertiesWindow.SetActive(false);
        screenshotWindow.SetActive(false);
        openMeButton.SetActive(true);
    }
    public void TakeScreenshot()
    {
        Texture2D tex = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        RenderTexture rTex = cameraInstance.GetTextureFromCamera();
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        File.WriteAllBytes(Settings.Files.GetResultFolder(ResultType.IMAGE) + DateTime.Now.ToString(Settings.Files.dateFormat) + ".png", tex.EncodeToPNG());
        //TODO: FIX
    }

    public void UpdateFOV() { cameraInstance.UpdateFOV(fovSlider.value); }

    public void SpoutNameLeft() { UpdateSpoutName(right: false); }
    public void SpoutNameRight() { UpdateSpoutName(right: true); }

    private void UpdateSpoutName(bool right = true)
    {
        int offset = right ? 1 : -1;
        spoutNameIndex += offset;
        spoutNameIndex %= Settings.Camera.cameraNames.Length;
        if (Settings.Camera.selectedNames.Contains(spoutName))
        {
            Settings.Camera.selectedNames.Remove(spoutName);
        }

        spoutName = GetNameByIndex(spoutNameIndex);

        spoutNameText.text = spoutName;
        cameraInstance.UpdateName(spoutName);
    }

    private string GetNameByIndex(int index)
    {
        string basicName = Settings.Camera.cameraNames[index];
        string actualName = basicName;

        int i = 1;
        while (Settings.Camera.selectedNames.Contains(spoutName))
        {
            actualName = basicName + i.ToString();
            i += 1;
        }
        Settings.Camera.selectedNames.Add(actualName);
        return actualName;
    }
}
