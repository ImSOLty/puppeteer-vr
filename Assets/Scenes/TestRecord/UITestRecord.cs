using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITestRecord : MonoBehaviour
{
    bool started = false;
    ActualRecorder actualRecorder;

    void Start()
    {
        actualRecorder = FindObjectOfType<ActualRecorder>();
    }
    public void StartEndAnimation()
    {
        if (!started)
        {
            started = true;
            actualRecorder.StartRecord();
        }
        else
        {
            // End animation
            actualRecorder.EndRecord();
            Destroy(gameObject);
        }
    }
}
