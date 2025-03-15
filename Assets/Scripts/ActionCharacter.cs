using System;
using UnityEngine;

public class ActionCharacter : ActionObject
{
    public bool wasRecorded = false;
    IKTargetFollowVRRig ikFollower;
    private Renderer[] renderers;
    void Awake()
    {
        isCharacter = true;
        ikFollower = GetComponent<IKTargetFollowVRRig>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void SetUsage(bool used = true)
    {
        ikFollower.enabled = used;
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = used || wasRecorded;
        }
    }
}
