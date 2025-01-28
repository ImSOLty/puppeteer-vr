using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterSetManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private IKTargetFollowVRRig ikTarget;
    [SerializeField] private Transform RigHips;

    private void Start()
    {
        AddObjectControl(RigHips);
    }

    void AddObjectControl(Transform tr)
    {
        foreach (Transform newTr in tr.GetComponentsInChildren<Transform>())
        {
            newTr.AddComponent<ObjectToRecord>();
        }
    }

    public void SetRecording(bool on)
    {
        animator.enabled = on;
        ikTarget.enabled = on;
    }
}