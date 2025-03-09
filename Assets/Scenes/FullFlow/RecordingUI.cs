using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordingUI : MonoBehaviour
{
    private AnimationManager animationManager;
    [SerializeField] private Text timerText;

    void Awake()
    {
        animationManager = FindObjectOfType<AnimationManager>();
    }

    public void Record()
    {
        animationManager.StartRecording();
    }

    public void SetTime(int secondsRemaining)
    {
        timerText.text = secondsRemaining.ToString();
    }

    public void Action()
    {
        SetTime(animationManager.GetSecondsLeft());
    }
}
