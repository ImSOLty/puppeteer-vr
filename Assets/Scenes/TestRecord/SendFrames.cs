using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendFrames : MonoBehaviour
{
    CameraInstance instance;
    ActualRecorder recorder;

    void Start()
    {
        instance = GetComponent<CameraInstance>();
        recorder = FindObjectOfType<ActualRecorder>();
    }
    void Update()
    {
        RenderTexture tex = instance.GetTextureFromCamera();
        recorder.AddFrame(tex);
    }
}
