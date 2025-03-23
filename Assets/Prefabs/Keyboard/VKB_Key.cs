using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class VKB_Key : MonoBehaviour
{
    public VKB_KeyType keyType;
    public string displayedText;
    public string displayedShiftedText;
    public string printedText;
    public string printedShiftedText;
    public Text currentText, anotherText, singleText;

    public void SwitchCase(bool shifted = false)
    {
        if (displayedText == displayedShiftedText)
        {
            singleText.text = displayedText;
            return;
        }
        currentText.text = shifted ? displayedShiftedText : displayedText;
        anotherText.text = shifted ? displayedText : displayedShiftedText;
    }
}
