using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class VKB_Keyboard : MonoBehaviour
{
    [SerializeField] VKB_Key[] keys;
    private bool shifted = false;
    private string text;
    private InputField inputField;
    private int index = 0;
    void Start()
    {
        RedrawKeys();
        foreach (VKB_Key key in keys)
        {
            key.GetComponent<Button>().onClick.AddListener(delegate { ProcessKey(key); });
        }
    }

    public void SetupKeyboard(InputField inputField)
    {
        this.inputField = inputField;
        this.text = inputField.text;
        this.index = text.Length;
    }

    void ProcessKey(VKB_Key key)
    {
        switch (key.keyType)
        {
            case VKB_KeyType.Shift:
                shifted = !shifted;
                RedrawKeys();
                break;
            case VKB_KeyType.Enter:
                gameObject.SetActive(false);
                break;
            case VKB_KeyType.Backspace:
                if (index == 0) { break; }
                index -= 1;
                text = text.Remove(index, 1);
                break;
            case VKB_KeyType.Left:
                if (index != 0) index -= 1;
                break;
            case VKB_KeyType.Right:
                if (index != text.Length) index += 1;
                break;
            case VKB_KeyType.MaxLeft:
                index = 0;
                break;
            case VKB_KeyType.MaxRight:
                index = text.Length;
                break;
            default:
                text = text[..index] + (shifted ? key.printedShiftedText : key.printedText) + text[index..];
                index += 1;
                break;
        }
        this.inputField.text = this.text;
        this.inputField.caretPosition = index;
    }

    void RedrawKeys()
    {
        foreach (VKB_Key key in keys) { key.SwitchCase(shifted: shifted); }
    }
}
