using UnityEngine;

public class MenuPanelsUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel, calibrationPanel, assetsPanel, scenesPanel;
    [SerializeField] private CalibrationUI calibrationUI;
    public void OpenCalibration()
    {
        foreach (GameObject panel in new GameObject[] { assetsPanel, mainMenuPanel, scenesPanel }) { panel.SetActive(false); }
        calibrationPanel.SetActive(true);
        calibrationUI.PanelActivate();
        Settings.Hints.currentHintAbout = HintAbout.SETTINGS;
    }
    public void OpenAssets()
    {
        foreach (GameObject panel in new GameObject[] { calibrationPanel, mainMenuPanel, scenesPanel }) { panel.SetActive(false); }
        assetsPanel.SetActive(true);
        Settings.Hints.currentHintAbout = HintAbout.ASSETS;
    }
    public void OpenScenes()
    {
        foreach (GameObject panel in new GameObject[] { calibrationPanel, mainMenuPanel, assetsPanel }) { panel.SetActive(false); }
        scenesPanel.SetActive(true);
        Settings.Hints.currentHintAbout = HintAbout.SCENES;
    }
    public void BackToMainMenu()
    {
        foreach (GameObject panel in new GameObject[] { calibrationPanel, assetsPanel, scenesPanel }) { panel.SetActive(false); }
        mainMenuPanel.SetActive(true);
        Settings.Hints.currentHintAbout = HintAbout.MAIN_MENU;
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
