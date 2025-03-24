using System.Collections;
using System.Collections.Generic;
using System.IO;
using FFmpegOut;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.UI;

public class ExportResolution
{
    public int width, height;
    public ExportResolution(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
    // From <width>x<height> string
    public ExportResolution(string repr)
    {
        string[] values = repr.Split('x');
        int.TryParse(values[0], out width);
        int.TryParse(values[1], out height);
    }
    public override string ToString()
    {
        return width.ToString() + "x" + height.ToString();
    }
    public bool Equals(ExportResolution obj)
    {
        return this.width == obj.width && this.height == obj.height;
    }
}
public class ExportSettings
{
    public ExportResolution exportResolution = new ExportResolution(width: 1920, height: 1080);
    public FFmpegPreset ffmpegPreset = FFmpegPreset.H264Default;
    public string saveFolder = Settings.Files.GetResultFolder(ResultType.VIDEO);
    public string fileName = "output";
    public string FullPath()
    {
        return Path.Combine(saveFolder, fileName) + ffmpegPreset.GetSuffix();
    }
}
public class RecordSettingsUI : MonoBehaviour
{
    public ExportSettings exportSettings = new();
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private InputField filenameInputField;
    [SerializeField] private Text selectFolderButtonText;
    [SerializeField] private FileSelectionManager fileSelectionManager;
    [SerializeField] private Dropdown resolutionDropdown, presetDropdown;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private AspectRatioFitter[] aspectRatioFitters;

    void Start()
    {
        filenameInputField.text = exportSettings.fileName;
        selectFolderButtonText.text = exportSettings.saveFolder;
        resolutionDropdown.value = resolutionDropdown.options.FindIndex(
            (option) => option.text == exportSettings.exportResolution.ToString()
        );
        presetDropdown.value = presetDropdown.options.FindIndex(
            (option) => option.text == FFmpegPresetExtensions.GetDisplayName(exportSettings.ffmpegPreset)
        );
    }

    public void SettingsOpen()
    {
        settingsWindow.SetActive(true);
    }

    public void SettingsClose()
    {
        settingsWindow.SetActive(false);
    }

    public void SelectFolder()
    {
        FileBrowser.ShowLoadDialog((paths) =>
        {
            selectFolderButtonText.text = paths[0];
        }, () => { }, FileBrowser.PickMode.Folders, false, null, null, "Select Folder", "Select");

        fileSelectionManager.SetupCanvasAfterInit();
    }

    public void SettingsSave()
    {
        exportSettings.fileName = filenameInputField.text;
        exportSettings.saveFolder = selectFolderButtonText.text;


        ExportResolution newExportResolution = new ExportResolution(resolutionDropdown.options[resolutionDropdown.value].text);
        if (!exportSettings.exportResolution.Equals(newExportResolution))
        {
            exportSettings.exportResolution = newExportResolution;
            cameraManager.UpdateAllCamerasResolution(newExportResolution);
            foreach (AspectRatioFitter fitter in aspectRatioFitters)
            {
                fitter.aspectRatio = (float)newExportResolution.width / newExportResolution.height;
            }
        }
        exportSettings.ffmpegPreset = FFmpegPresetExtensions.GetPreset(presetDropdown.options[presetDropdown.value].text);

        SettingsClose();
    }
}
