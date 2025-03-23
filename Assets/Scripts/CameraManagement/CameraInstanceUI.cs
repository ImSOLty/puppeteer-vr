using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Klak.Spout;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CameraInstanceUI : MonoBehaviour
{
    [SerializeField] private CameraInstance cameraInstance;
    [SerializeField] private Text spoutNameRuntimeText;
    [SerializeField] private Text spoutNameText;
    [SerializeField] private Slider fovSlider;
    [SerializeField] private GameObject propertiesWindow, actionsWindow, openMeButton;
    [SerializeField] private SpoutSender spoutSender;
    static private SpoutSender mainSpoutSender;
    private int spoutNameIndex;
    private string spoutName;

    void Start()
    {
        if (Settings.Animation.AnimationMode != Mode.PROPS_MANAGEMENT)
        {
            if (Settings.Animation.AnimationMode == Mode.ANIMATION_RUNTIME)
            {
                spoutNameRuntimeText.gameObject.SetActive(true);
                UpdateSpoutName(cameraInstance.GetCameraData().Name);

                if (mainSpoutSender == null)
                {
                    mainSpoutSender = FindObjectOfType<AnimationManager>().GetOrAddComponent<SpoutSender>(); // Attach to managers
                    mainSpoutSender.spoutName = "Main";
                    mainSpoutSender.captureMethod = CaptureMethod.Texture;
                    mainSpoutSender.sourceTexture = cameraInstance.GetTextureFromCamera();
                    mainSpoutSender.enabled = true;
                }

                spoutSender.enabled = true;
                spoutSender.sourceTexture = cameraInstance.GetTextureFromCamera();
            }
        }
        else
        {
            spoutNameIndex = UnityEngine.Random.Range(0, Settings.Camera.cameraNames.Length);
            NewSpoutName();
        }
    }

    public void Open()
    {
        if (Settings.Animation.AnimationMode == Mode.PROPS_MANAGEMENT)
            propertiesWindow.SetActive(true);
        else if (Settings.Animation.AnimationMode == Mode.ANIMATION_RUNTIME)
            actionsWindow.SetActive(true);
        openMeButton.SetActive(false);
    }
    public void Close()
    {
        propertiesWindow.SetActive(false);
        actionsWindow.SetActive(false);
        openMeButton.SetActive(true);
    }
    public void TakeScreenshot()
    {
        Texture2D tex = new Texture2D(1920, 1080, TextureFormat.RGBA32, false);
        RenderTexture rTex = cameraInstance.GetTextureFromCamera();
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        File.WriteAllBytes(Path.Combine(Settings.Files.GetResultFolder(ResultType.IMAGE), DateTime.Now.ToString(Settings.Files.dateFormat) + ".png"), tex.EncodeToPNG());
        //TODO: FIX
    }

    public void UpdateFOV() { cameraInstance.UpdateFOV(fovSlider.value); }
    public void SpoutNameLeft() { NewSpoutName(right: false); }
    public void SpoutNameRight() { NewSpoutName(right: true); }
    private void NewSpoutName(bool right = true)
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
        UpdateSpoutName(spoutName);
    }
    private void UpdateSpoutName(string newSpoutName)
    {
        spoutName = newSpoutName;
        cameraInstance.UpdateName(newSpoutName);
        spoutNameRuntimeText.text = newSpoutName;
        spoutNameText.text = newSpoutName;

        if (spoutSender != null)
        {
            spoutSender.spoutName = newSpoutName;
        }
    }
    public void SetAsMainSpout()
    {
        mainSpoutSender.sourceTexture = cameraInstance.GetTextureFromCamera();
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
