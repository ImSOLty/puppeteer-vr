using System.Collections;
using System.Collections.Generic;
using System.IO;
using FFmpegOut;
using UnityEngine;

public class ActualRecorder : MonoBehaviour
{
    [SerializeField] public RecordSettingsUI recordSettingsUI;

    public enum RecorderState { NotRecording, Recording, Pausing }

    public FFmpegSession _session;
    public RecorderState _state;

    int _frameCount;

    public void EndRecord()
    {
        _state = RecorderState.NotRecording;

        if (_session != null)
        {
            // Close and dispose the FFmpeg session.
            Debug.Log("Closing FFmpegSession after " + _frameCount + " frames.");

            _session.Close();
            _session.Dispose();
            _session = null;
        }
    }

    IEnumerator Start()
    {
        for (var eof = new WaitForEndOfFrame(); ;)
        {
            yield return eof;
            if (_session != null)
                _session.CompletePushFrames();
        }
    }
    public void StartRecord()
    {
        StartRecordWithPath(recordSettingsUI.exportSettings.FullPath());
    }

    public void StartRecordWithPath(string path)
    {
        if (_session != null)
        {
            _session.Dispose();
        }
        string fullpath = Path.GetFullPath(path);
        ExportResolution exportResolution = recordSettingsUI.exportSettings.exportResolution;
        // Start an FFmpeg session.
        Debug.Log("creating FFmpeg session with size " + exportResolution.ToString() + ", will be saved at " + fullpath);

        _session = FFmpegSession.CreateWithOutputPath(
            outputPath: fullpath,
            width: exportResolution.width,
            height: exportResolution.height,
            frameRate: Settings.Animation.FPS,
            preset: recordSettingsUI.exportSettings.ffmpegPreset
        );

        _frameCount = 0;
        _state = RecorderState.Recording;
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