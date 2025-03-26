using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[Serializable]
public class FingerPartMap
{
    public Transform fingerPartTransform = null;
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

    public void PostSolve()
    {
        if (fingerPartTransform == null) { return; }

        Vector3 newLocalPosition = fingerPartTransform.localPosition + positionOffset;
        Quaternion newLocalRotation = Quaternion.Euler(fingerPartTransform.localRotation.eulerAngles + rotationOffset);

        fingerPartTransform.SetLocalPositionAndRotation(newLocalPosition, newLocalRotation);
    }
}
[Serializable]
public class FingersMap
{
    public FingerPartMap proximal, intermediate, distal;
    public void PostSolve()
    {
        foreach (FingerPartMap fingerPart in new[] { proximal, intermediate, distal })
        {
            fingerPart.PostSolve();
        }
    }
}
[Serializable]
public class HandCalibrationSettings
{
    public FingersMap thumb, index, middle, ring, pinky;
    public void PostSolve()
    {
        foreach (FingersMap finger in new[] { thumb, index, middle, ring, pinky })
        {
            finger.PostSolve();
        }
    }
}

// SHOULD BE PLACED NEAR THE HAND TRANSFORM
public class HandTrackingSolver : SteamVR_Behaviour_Skeleton
{
    public HandCalibrationSettings handMap = new();
    private Dictionary<PuppeteerBone, Transform> mapping;
    private bool isRightHand;
    void LateUpdate()
    {
        handMap.PostSolve();
    }
    public void LoadCalibrationSettings(HandCalibrationSettings from)
    //First
    {
        handMap = from;
    }

    public void Setup(bool rightHand, Dictionary<PuppeteerBone, Transform> mapping)
    //Second
    {
        isRightHand = rightHand;

        this.origin = mapping[isRightHand ? PuppeteerBone.RightHand : PuppeteerBone.LeftHand];
        this.onlySetRotations = true;
        this.inputSource = isRightHand ? SteamVR_Input_Sources.RightHand : SteamVR_Input_Sources.LeftHand;
        this.skeletonAction = SteamVR_Input.GetAction<SteamVR_Action_Skeleton>("Skeleton" + inputSource.ToString());

        this.mapping = mapping;

        SetupHandMap();
        SetupSkeleton();
    }
    public void SetupHandMap()
    {
        // Sadly, but it is needed here like so
        handMap.thumb.proximal.fingerPartTransform = mapping[isRightHand ? PuppeteerBone.RightThumbProximal : PuppeteerBone.LeftThumbProximal];
        handMap.thumb.intermediate.fingerPartTransform = mapping[isRightHand ? PuppeteerBone.RightThumbIntermediate : PuppeteerBone.LeftThumbIntermediate];
        handMap.thumb.distal.fingerPartTransform = mapping[isRightHand ? PuppeteerBone.RightThumbDistal : PuppeteerBone.LeftThumbDistal];
        handMap.index.proximal.fingerPartTransform = mapping[isRightHand ? PuppeteerBone.RightIndexProximal : PuppeteerBone.LeftIndexProximal];
        handMap.index.intermediate.fingerPartTransform = mapping[isRightHand ? PuppeteerBone.RightIndexIntermediate : PuppeteerBone.LeftIndexIntermediate];
        handMap.index.distal.fingerPartTransform = mapping[isRightHand ? PuppeteerBone.RightIndexDistal : PuppeteerBone.LeftIndexDistal];
        handMap.middle.proximal.fingerPartTransform = mapping[isRightHand ? PuppeteerBone.RightMiddleProximal : PuppeteerBone.LeftMiddleProximal];
        handMap.middle.intermediate.fingerPartTransform = mapping[isRightHand ? PuppeteerBone.RightMiddleIntermediate : PuppeteerBone.LeftMiddleIntermediate];
        handMap.middle.distal.fingerPartTransform = mapping[isRightHand ? PuppeteerBone.RightMiddleDistal : PuppeteerBone.LeftMiddleDistal];
        handMap.ring.proximal.fingerPartTransform = mapping[isRightHand ? PuppeteerBone.RightRingProximal : PuppeteerBone.LeftRingProximal];
        handMap.ring.intermediate.fingerPartTransform = mapping[isRightHand ? PuppeteerBone.RightRingIntermediate : PuppeteerBone.LeftRingIntermediate];
        handMap.ring.distal.fingerPartTransform = mapping[isRightHand ? PuppeteerBone.RightRingDistal : PuppeteerBone.LeftRingDistal];
        handMap.pinky.proximal.fingerPartTransform = mapping[isRightHand ? PuppeteerBone.RightLittleProximal : PuppeteerBone.LeftLittleProximal];
        handMap.pinky.intermediate.fingerPartTransform = mapping[isRightHand ? PuppeteerBone.RightLittleIntermediate : PuppeteerBone.LeftLittleIntermediate];
        handMap.pinky.distal.fingerPartTransform = mapping[isRightHand ? PuppeteerBone.RightLittleDistal : PuppeteerBone.LeftLittleDistal];
    }
    public void SetupSkeleton()
    {
        bones[SteamVR_Skeleton_JointIndexes.thumbProximal] = handMap.thumb.proximal.fingerPartTransform;
        bones[SteamVR_Skeleton_JointIndexes.thumbMiddle] = handMap.thumb.intermediate.fingerPartTransform;
        bones[SteamVR_Skeleton_JointIndexes.thumbDistal] = handMap.thumb.distal.fingerPartTransform;
        bones[SteamVR_Skeleton_JointIndexes.indexProximal] = handMap.index.proximal.fingerPartTransform;
        bones[SteamVR_Skeleton_JointIndexes.indexMiddle] = handMap.index.intermediate.fingerPartTransform;
        bones[SteamVR_Skeleton_JointIndexes.indexDistal] = handMap.index.distal.fingerPartTransform;
        bones[SteamVR_Skeleton_JointIndexes.middleProximal] = handMap.middle.proximal.fingerPartTransform;
        bones[SteamVR_Skeleton_JointIndexes.middleMiddle] = handMap.middle.intermediate.fingerPartTransform;
        bones[SteamVR_Skeleton_JointIndexes.middleDistal] = handMap.middle.distal.fingerPartTransform;
        bones[SteamVR_Skeleton_JointIndexes.ringProximal] = handMap.ring.proximal.fingerPartTransform;
        bones[SteamVR_Skeleton_JointIndexes.ringMiddle] = handMap.ring.intermediate.fingerPartTransform;
        bones[SteamVR_Skeleton_JointIndexes.ringDistal] = handMap.ring.distal.fingerPartTransform;
        bones[SteamVR_Skeleton_JointIndexes.pinkyProximal] = handMap.pinky.proximal.fingerPartTransform;
        bones[SteamVR_Skeleton_JointIndexes.pinkyMiddle] = handMap.pinky.intermediate.fingerPartTransform;
        bones[SteamVR_Skeleton_JointIndexes.pinkyDistal] = handMap.pinky.distal.fingerPartTransform;
    }
    protected override void AssignBonesArray()
    {
        bones = new Transform[31];
        // skip since we are setting bones in setup
    }
}
