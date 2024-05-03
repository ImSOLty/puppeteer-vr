using UnityEngine;
using UnityEngine.UI;
using UTJ.FrameCapturer;

public class UIElements : MonoBehaviour
{
    public InputField input;
    private MovieRecorder _movieRecorder;

    // Called via button press
    public void StartSimulation()
    {
        FindObjectOfType<Timeline>().SetExportPath(input.text);
        FindObjectOfType<ActionSimulator>().RunSimulation();
    }
}