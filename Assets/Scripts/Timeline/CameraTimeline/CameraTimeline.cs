using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CameraTimeline : MonoBehaviour
{
    private float _duration = 0;
    private LinkedList<CameraSection> _cameraSections = new();

    private void Start()
    {
        _cameraSections.AddFirst(new CameraSection(null, 0, 0));
    }

    public CameraSection AddSection(CameraInstance instance, float start, float end)
    {
        if (end > _duration || end == start || start < 0) return null;
        if (_cameraSections.Any(section =>
                (section.Start > start && section.Start < end || section.End > start && section.End < end)
                && section.CamInstance != null)
           ) return null;
        var node = _cameraSections.First;

        while (!(node.Value.Start <= start && end <= node.Value.End)) node = node.Next;

        if (node.Value.Start != start)
        {
            _cameraSections.AddBefore(node, new CameraSection(null, node.Value.Start, start));
        }

        if (node.Value.End != end)
        {
            _cameraSections.AddAfter(node, new CameraSection(null, end, node.Value.End));
        }

        node.Value = new CameraSection(instance, start, end);
        return node.Value;
    }

    public void RemoveSection(CameraSection section)
    {
        var node = _cameraSections.Find(section);
        if (node is null || section.CamInstance is null) return;

        node.Value = new CameraSection(null, section.Start, section.End);
        if (node.Previous != null && node.Previous.Value.CamInstance == node.Value.CamInstance)
        {
            node.Value.Start = node.Previous.Value.Start;
            _cameraSections.Remove(node.Previous);
        }

        if (node.Next != null && node.Next.Value.CamInstance == node.Value.CamInstance)
        {
            node.Value.End = node.Next.Value.End;
            _cameraSections.Remove(node.Next);
        }
    }

    public void MoveSection()
    {
    }

    public bool SetDuration(float seconds)
    {
        if (seconds == _duration) return true;
        if (_cameraSections.Any(section => section.End > seconds && section.CamInstance != null)) return false;

        _duration = seconds;
        if (_cameraSections.Last().CamInstance is null)
            _cameraSections.Last().End = _duration;
        else
            _cameraSections.AddLast(new CameraSection(null, _cameraSections.Last().End, _duration));

        return true;
    }

    public LinkedList<CameraSection> GetCameraSections()
    {
        return _cameraSections;
    }
    
    public override string ToString()
    {
        return String.Join(" | ", _cameraSections);
    }
}