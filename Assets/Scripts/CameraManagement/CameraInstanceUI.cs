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
    [SerializeField] private InputField spoutNameInputField;
    [SerializeField] private Slider fovSlider;
    [SerializeField] private GameObject propertiesWindow, actionsWindow, openMeButton;
    [SerializeField] private SpoutSender spoutSender;
    static private SpoutSender mainSpoutSender;

    void Start()
    {
        if (Settings.Animation.AnimationMode != Mode.PROPS_MANAGEMENT)
        {
            // onChange is called
            spoutNameInputField.text = cameraInstance.GetCameraData().Name;

            if (Settings.Animation.AnimationMode == Mode.ANIMATION_RUNTIME)
            {
                spoutNameRuntimeText.gameObject.SetActive(true);

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
            SpoutNameRandom();
        }
    }

    public void Open()
    {
        if (Settings.Animation.AnimationMode == Mode.PROPS_MANAGEMENT || Settings.Animation.AnimationMode == Mode.PROPS_MANAGEMENT_EDIT)
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
    public void SpoutNameRandom()
    {
        string newName = Settings.Camera.cameraNames[UnityEngine.Random.Range(0, Settings.Camera.cameraNames.Length)];
        spoutNameInputField.text = newName;
    }
    public void SpoutNameOnChange()
    {
        string newSpoutName = spoutNameInputField.text;
        cameraInstance.UpdateName(newSpoutName);
        spoutNameRuntimeText.text = newSpoutName;

        if (spoutSender != null)
        {
            spoutSender.spoutName = newSpoutName;
        }
    }
    public void SetAsMainSpout()
    {
        mainSpoutSender.sourceTexture = cameraInstance.GetTextureFromCamera();
    }
}
