using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Valve.VR.InteractionSystem;
using VRM;

public class ActionCharacter : ActionObject
{
    public float neededScaleForThisModel = 1;
    public bool wasRecorded = false;
    RigBuilder rigBuilder;
    IKTargetFollowVRRig ikFollower;
    Blinker blinker;
    HandTrackingSolver[] solvers;
    private Renderer[] renderers;
    void Awake()
    {
        isCharacter = true;
        rigBuilder = GetComponent<RigBuilder>();
        ikFollower = GetComponent<IKTargetFollowVRRig>();
        solvers = GetComponentsInChildren<HandTrackingSolver>();
        renderers = GetComponentsInChildren<Renderer>();
        blinker = transform.AddComponent<Blinker>();
    }

    public void SetUsage(bool used = true)
    {
        if (used)
        {
            FindObjectOfType<Player>().transform.localScale = Vector3.one * neededScaleForThisModel;
        }
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
