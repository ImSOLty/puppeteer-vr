using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCharacter : ActionObject
{
    IKTargetFollowVRRig ikFollower;
    void Awake()
    {
        ikFollower = GetComponent<IKTargetFollowVRRig>();
    }

    public void SetUsage(bool used = true)
    {
        ikFollower.enabled = used;
    }
}
