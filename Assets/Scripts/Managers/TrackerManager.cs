using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;


[Serializable]
public class Tracker
{
    public SteamVR_Input_Sources input_source;
    public Transform trackerTransform;
    public Transform target;
    public bool isUsed = true;
}

[Serializable]
public class HeadTracker : Tracker
{
    public HeadTracker(Player player)
    {
        input_source = SteamVR_Input_Sources.Head;
        trackerTransform = player.audioListener;
        target = trackerTransform.Find("Target").transform;
    }
}

[Serializable]
public class BodyTracker : Tracker
{
    public SteamVR_Behaviour_Pose behaviourPoseComponent;

    public BodyTracker(SteamVR_Behaviour_Pose behaviourPose)
    {
        behaviourPoseComponent = behaviourPose;
        trackerTransform = behaviourPose.transform;
        input_source = behaviourPoseComponent.inputSource;
        target = trackerTransform.Find("Target").transform;
    }
}

public class TrackerManager : MonoBehaviour
{
    private Player player;
    private HeadTracker headTracker;
    [SerializeField] private Dictionary<SteamVR_Input_Sources, BodyTracker> bodyTrackers;

    public void Awake()
    {
        DefineTrackers();
    }

    public void DefineTrackers()
    {
        player = FindObjectOfType<Player>();
        headTracker = new HeadTracker(player);
        bodyTrackers = new();
        foreach (SteamVR_Behaviour_Pose trackerComponent in FindObjectsOfType<SteamVR_Behaviour_Pose>())
        {
            bodyTrackers.Add(trackerComponent.inputSource, new BodyTracker(trackerComponent));
        }
    }


    public Tracker GetTracker(SteamVR_Input_Sources source)
    {
        if (source == SteamVR_Input_Sources.Head)
        {
            return headTracker;
        }
        if (!bodyTrackers.ContainsKey(source))
        {
            return null;
        }
        return bodyTrackers[source];
    }
}
