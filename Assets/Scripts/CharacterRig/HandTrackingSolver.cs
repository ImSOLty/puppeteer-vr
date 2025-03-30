using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[Serializable]
public class FingerPartMap
{
    private Transform fingerPartTransform = null;
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

    public void PostSolve(bool mirror)
    {
        if (fingerPartTransform == null) { return; }
        Vector3 positionOffsetActual = positionOffset;
        Vector3 rotationOffsetActual = rotationOffset;
        if (mirror)
        {
            positionOffsetActual.x *= -1;
            rotationOffsetActual.y *= -1;
            rotationOffsetActual.z *= -1;
        }

        Vector3 newLocalPosition = fingerPartTransform.localPosition + positionOffsetActual;
        Quaternion newLocalRotation = Quaternion.Euler(fingerPartTransform.localRotation.eulerAngles + rotationOffsetActual);

        fingerPartTransform.SetLocalPositionAndRotation(newLocalPosition, newLocalRotation);
    }

    public Transform GetTransform() { return fingerPartTransform; }
    public void SetTransform(Transform transform) { fingerPartTransform = transform; }
    public void CopyCalibration(FingerPartMap other)
    {
        positionOffset = other.positionOffset;
        rotationOffset = other.rotationOffset;
    }
}
[Serializable]
public class FingersMap
{
    public FingerPartMap proximal = new(), intermediate = new(), distal = new();
    public void PostSolve(bool mirror)
    {
        foreach (FingerPartMap fingerPart in new[] { proximal, intermediate, distal })
        {
            fingerPart.PostSolve(mirror);
        }
    }

    public void CopyCalibration(FingersMap other)
    {
        proximal.CopyCalibration(other.proximal);
        intermediate.CopyCalibration(other.intermediate);
        distal.CopyCalibration(other.distal);
    }
}
[Serializable]
public class HandCalibrationSettings
{
    public FingersMap thumb = new(), index = new(), middle = new(), ring = new(), pinky = new();
    public bool isUsed = true;
    public void PostSolve(bool mirror = false)
    {
        foreach (FingersMap finger in new[] { thumb, index, middle, ring, pinky })
        {
            finger.PostSolve(mirror);
        }
    }

    public void CopyCalibration(HandCalibrationSettings other)
    {
        thumb.CopyCalibration(other.thumb);
        index.CopyCalibration(other.index);
        middle.CopyCalibration(other.middle);
        ring.CopyCalibration(other.ring);
        pinky.CopyCalibration(other.pinky);

        isUsed = other.isUsed;
    }
}

public class HandTrackingSolver : SteamVR_Behaviour_Skeleton
{
    public HandTrackingSolver otherSolver;
    public HandCalibrationSettings handMap = new();
    private Dictionary<PuppeteerBone, FingerPartMap> fingerPartMapping = new();
    private Dictionary<PuppeteerBone, Transform> transformMapping;
    [SerializeField] private bool isRightHand;
    private bool activated = true;

    protected override void UpdateSkeleton()
    {
        base.UpdateSkeleton();
        if (!handMap.isUsed) return;
        handMap.PostSolve(mirror: !isRightHand);
    }

    public void LoadCalibrationSettings(HandCalibrationSettings from)
    //First
    {
        handMap.CopyCalibration(from);
    }
    public void DeactivateSkeleton() { if (!activated) return; activated = false; this.OnDisable(); }
    public void ActivateSkeleton() { if (activated) return; activated = true; this.OnEnable(); }

    public void Setup(Dictionary<PuppeteerBone, Transform> mapping)
    //Second
    {
        PuppeteerBone handBone = isRightHand ? PuppeteerBone.RightHand : PuppeteerBone.LeftHand;
        this.inputSource = Puppeteer.BonesMapping.SteamVR_Input(handBone);
        this.onlySetRotations = true;
        this.skeletonAction = SteamVR_Input.GetAction<SteamVR_Action_Skeleton>("Skeleton" + inputSource.ToString());
        this.mirroring = isRightHand ? MirrorType.None : MirrorType.LeftToRight;
        this.origin = mapping[handBone];

        this.transformMapping = mapping;

        SetupHandMap();
    }
    public void SetupHandMap()
    {
        fingerPartMapping.Clear();

        foreach ((PuppeteerBone, FingerPartMap) bone in new[] {
            (PuppeteerBone.AnyThumbProximal, handMap.thumb.proximal),
            (PuppeteerBone.AnyThumbIntermediate, handMap.thumb.intermediate),
            (PuppeteerBone.AnyThumbDistal, handMap.thumb.distal),
            (PuppeteerBone.AnyIndexProximal, handMap.index.proximal),
            (PuppeteerBone.AnyIndexIntermediate, handMap.index.intermediate),
            (PuppeteerBone.AnyIndexDistal, handMap.index.distal),
            (PuppeteerBone.AnyMiddleProximal, handMap.middle.proximal),
            (PuppeteerBone.AnyMiddleIntermediate, handMap.middle.intermediate),
            (PuppeteerBone.AnyMiddleDistal, handMap.middle.distal),
            (PuppeteerBone.AnyRingProximal, handMap.ring.proximal),
            (PuppeteerBone.AnyRingIntermediate, handMap.ring.intermediate),
            (PuppeteerBone.AnyRingDistal, handMap.ring.distal),
            (PuppeteerBone.AnyLittleProximal, handMap.pinky.proximal),
            (PuppeteerBone.AnyLittleIntermediate, handMap.pinky.intermediate),
            (PuppeteerBone.AnyLittleDistal, handMap.pinky.distal),
        })
        {
            SteamVR_Skeleton_JointIndexEnum jointIndex = Puppeteer.BonesMapping.SteamVR_Skeleton(bone.Item1);
            PuppeteerBone actualBone = Puppeteer.BonesMapping.DefineSideFingerBone(isRightHand, bone.Item1);
            FingerPartMap fingerPartMap = bone.Item2;
            fingerPartMap.SetTransform(transformMapping[actualBone]);
            bones[(int)jointIndex] = fingerPartMap.GetTransform();
            fingerPartMapping[bone.Item1] = fingerPartMap;
        }
    }
    public FingerPartMap GetFingerPartMap(PuppeteerBone bone)
    {
        return fingerPartMapping[bone];
    }
    protected override void AssignBonesArray()
    {
        bones = new Transform[31];
        // skip since bones are set in setup
    }

    public void CopyCalibrationToOtherHand()
    {
        otherSolver.handMap.CopyCalibration(this.handMap);
    }
}
