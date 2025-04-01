using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHint : MonoBehaviour
{
    [SerializeField] Text text;

    public void Setup(HintAbout hintAbout = HintAbout.UNKNOWN)
    {
        if (hintAbout == HintAbout.UNKNOWN)
        {
            text.text = Settings.Hints.GetCurrentHintMessage();
        }
        else
        {
            text.text = Settings.Hints.GetHintMessage(hintAbout);
        }
    }
}
