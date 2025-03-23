using System.Collections;
using System.Collections.Generic;
using System.IO;
using FFmpegOut;
using UnityEngine;

public class ActualRecorder : MonoBehaviour
{
    [SerializeField] public string _outputName = "output";
    [SerializeField] public FFmpegOut.FFmpegPreset _preset = FFmpegOut.FFmpegPreset.VP8Default;
    [SerializeField] public int _targetBitrateInMegabits = 60;
    [SerializeField] public int _compression = 18;


    public bool isOverwrite;
    public float overwriteNearClipFactor = 0.5f;

    public enum RecorderState
    {
        NotRecording,
        Recording,
        Pausing
    }


    public FFmpegOut.FFmpegSession _session;
    public RecorderState _state;

    int _frameCount;
    float _startTime;
    float _pauseTime = 0;
    int _frameDropCount;

    float FrameTime
    {
        get { return _startTime + _pauseTime + (_frameCount - 0.5f) / Settings.Animation.FPS; }
    }

    void WarnFrameDrop()
    {
        if (++_frameDropCount != 10) return;

        Debug.LogWarning(
            "Significant frame droppping was detected. This may introduce " +
            "time instability into output video. Decreasing the recording " +
            "frame rate is recommended."
        );
    }


    public void EndRecord()
    {
        _state = RecorderState.NotRecording;
        _pauseTime = 0;

        if (_session != null)
        {
            // Close and dispose the FFmpeg session.
            Debug.Log("Closing FFmpegSession after " + _frameCount + " frames.");

            _session.Close();
            _session.Dispose();
            _session = null;
        }

        // if (GetComponent<FFmpegOut.FrameRateController>() == null)
        // {
        //     Time.captureDeltaTime = oldCaptureDeltatime;
        //     oldCaptureDeltatime = 0f;
        // }
    }

    float oldCaptureDeltatime;

    IEnumerator Start()
    {
        // if (GetComponent<FFmpegOut.FrameRateController>() == null)
        // {
        //     oldCaptureDeltatime = Time.captureDeltaTime;
        //     Time.captureDeltaTime = 1.0f / _frameRate;
        // }

        for (var eof = new WaitForEndOfFrame(); ;)
        {
            yield return eof;
            if (_session != null)
                _session.CompletePushFrames();
        }
    }
    public void StartRecord()
    {
        // correct the extension if needed
        StartRecordWithPath(Path.ChangeExtension(_outputName, _preset.GetSuffix()));
    }

    public void StartRecordWithPath(string path)
    {
        if (_session != null)
        {
            _session.Dispose();
        }

        string fullpath = Path.GetFullPath(path);

        // Start an FFmpeg session.
        Debug.Log("creating FFmpeg session with size 1920x1080, will be saved at " + fullpath);

        string extraFfmpegOptions = "-b:v " + _targetBitrateInMegabits + "M";

        // #if !UNITY_EDITOR_OSX && !UNITY_STANDALONE_OSX
        //         if (preset == FFmpegOut.FFmpegPreset.H264Nvidia || preset == FFmpegOut.FFmpegPreset.HevcNvidia)
        //         {
        //             extraFfmpegOptions += " -cq:v " + _compression;
        //         }
        //         else
        //         {
        //             extraFfmpegOptions += " -crf " + _compression;
        //         }
        // #endif

        _session = FFmpegOut.FFmpegSession.CreateWithOutputPath(
            path,
           1920,
            1080,
            Settings.Animation.FPS, _preset
        );

        _startTime = Time.time;
        _frameCount = 0;
        _frameDropCount = 0;

        _state = RecorderState.Recording;
    }

    public void PauseRecord()
    {
        if (_state == RecorderState.Recording)
            _state = RecorderState.Pausing;
        else
            Debug.LogWarning("[Holoplay] Can't pause recording when it's not started");
    }

    public void ResumeRecord()
    {
        if (_state == RecorderState.Pausing)
        {
            _state = RecorderState.Recording;
        }
        else
            Debug.LogWarning("[Holoplay] Can't resume recording when it's not paused");
    }

    public void AddFrame(RenderTexture texture)
    {
        if (_state == RecorderState.NotRecording)
        {
            return;
        }
        _session.PushFrame(texture);
        _frameCount++;
        return;
    }
    public bool ReadyToAddFrame()
    {
        return _session.ReadyToQueue();
    }
}