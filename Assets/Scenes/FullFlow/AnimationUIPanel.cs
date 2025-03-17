using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnimationUIPanel : MonoBehaviour
{
    [SerializeField] GameObject ScenesWindow, AnimationWindow;
    [SerializeField] Slider fpsSlider, secondsSlider;
    [SerializeField] Text fps, seconds;
    public void Back()
    {
        ScenesWindow.SetActive(true);
        AnimationWindow.SetActive(false);
    }

    public void AnimateRuntime()
    {
        // SceneProperties already set from registry
        Settings.Animation.AnimationMode = AnimationMode.RUNTIME;
        SceneManager.LoadScene(Settings.Scenes.ActionSceneName);
    }

    public void UpdateFPS()
    {
        Settings.Animation.FPS = (int)fpsSlider.value;
        fps.text = Settings.Animation.FPS.ToString();
    }
    public void UpdateSeconds()
    {
        Settings.Animation.TotalTimeInSeconds = (int)secondsSlider.value;
        int min = Settings.Animation.TotalTimeInSeconds / 60;
        int sec = Settings.Animation.TotalTimeInSeconds % 60;

        seconds.text = string.Format("{0}:{1:00}", min, sec);
    }

    public void RecordAnimation()
    {
        // SceneProperties already set from registry
        Settings.Animation.AnimationMode = AnimationMode.RECORDING;
        SceneManager.LoadScene(Settings.Scenes.ActionSceneName);
    }
}
