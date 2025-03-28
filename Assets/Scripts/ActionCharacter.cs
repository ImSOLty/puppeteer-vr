using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActionCharacter : ActionObject
{
    public bool wasRecorded = false;
    RigBuilder rigBuilder;
    IKTargetFollowVRRig ikFollower;
    HandTrackingSolver[] solvers;
    private Renderer[] renderers;
    void Awake()
    {
        isCharacter = true;
        rigBuilder = GetComponent<RigBuilder>();
        ikFollower = GetComponent<IKTargetFollowVRRig>();
        solvers = GetComponentsInChildren<HandTrackingSolver>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void SetUsage(bool used = true)
    {
        rigBuilder.enabled = used;
        ikFollower.enabled = used;
        foreach (HandTrackingSolver solver in solvers)
        {
            if (used)
            {
                solver.ActivateSkeleton();
            }
            else
            {
                solver.DeactivateSkeleton();
            }

        }
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = used || wasRecorded;
        }
    }
}
