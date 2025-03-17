using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraInstanceUI : MonoBehaviour
{
    [SerializeField] private CameraInstance cameraInstance;
    [SerializeField] private Text spoutNameText;
    [SerializeField] private Slider fovSlider;
    [SerializeField] private GameObject propertiesWindow, openMeButton;
    private int spoutNameIndex;
    private string spoutName;

    void Start()
    {
        if (FindObjectOfType<PropsManagement>() == null) // For now check if propsManagementScene
            Destroy(this);
        spoutNameIndex = Random.Range(0, Settings.Camera.cameraNames.Length);
        UpdateSpoutName();
    }

    public void Open() { propertiesWindow.SetActive(true); openMeButton.SetActive(false); }
    public void Close() { propertiesWindow.SetActive(false); openMeButton.SetActive(true); }

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
