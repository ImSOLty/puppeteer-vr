using System;
using System.Collections.Generic;
using System.Linq;
using UniHumanoid;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using VRM;
using Valve.VR;

public class IKConstraint
{
    public Transform target;
    public virtual void constraintSet() { }
}

[Serializable]
public class IKSetupMultiParentConstraint : IKConstraint
{
    public MultiParentConstraint component;
    public Transform constrainedObject;

    public override void constraintSet()
    {
        component.data.constrainedObject = constrainedObject;
        target = component.data.sourceObjects[0].transform;
    }
    public void setConstrainedObjects(Transform constrainedObject)
    {
        this.constrainedObject = constrainedObject;
    }
}

[Serializable]
public class IKSetupTwoBoneConstraint : IKConstraint
{
    public TwoBoneIKConstraint component;
    public Transform root, mid, tip;
    public override void constraintSet()
    {
        component.data.root = root; component.data.mid = mid; component.data.tip = tip;
        target = component.data.target;
    }

    public void setConstrainedObjects(Transform root, Transform mid, Transform tip)
    {
        this.root = root; this.mid = mid; this.tip = tip;
    }
}


public class RigHelperSetup : MonoBehaviour
{

    [SerializeField] private IKSetupMultiParentConstraint waist, chest, head;
    [SerializeField] private IKSetupTwoBoneConstraint leftArm, rightArm, leftLeg, rightLeg;
    private Dictionary<HumanBodyBones, Transform> boneTransformMapping = new();
    private Transform vrmObject;
    private TrackerManager trackerManager;
    private BoneLimit[] bones;
    private bool sourceProvided = false;

    public void provideSources(Transform vrm, TrackerManager trackerManager)
    {
        vrmObject = vrm;
        bones = vrm.GetComponent<VRMHumanoidDescription>().Description.human;
        this.trackerManager = trackerManager;

        sourceProvided = true;
    }
    public bool Setup()
    {
        if (!sourceProvided)
        {
            return false;
        }

        prepareSetup();

        foreach (IKConstraint constraint in new List<IKConstraint>() { waist, chest, head, leftArm, rightArm, leftLeg, rightLeg })
        {
            constraint.constraintSet();
        }

        prepareIKFollowerSetup();

        return true;
    }
    private void prepareSetup()
    {
        boneTransformMapping = constructBoneTransformMapping();

        // MultiParentConstraints
        waist.setConstrainedObjects(boneTransformMapping[HumanBodyBones.Spine]);
        chest.setConstrainedObjects(boneTransformMapping[HumanBodyBones.Chest]);
        head.setConstrainedObjects(boneTransformMapping[HumanBodyBones.Head]);

        // TwoBoneConstraints (Arms)
        leftArm.setConstrainedObjects(
            root: boneTransformMapping[HumanBodyBones.LeftUpperArm],
            mid: boneTransformMapping[HumanBodyBones.LeftLowerArm],
            tip: boneTransformMapping[HumanBodyBones.LeftHand]
        );
        rightArm.setConstrainedObjects(
            root: boneTransformMapping[HumanBodyBones.RightUpperArm],
            mid: boneTransformMapping[HumanBodyBones.RightLowerArm],
            tip: boneTransformMapping[HumanBodyBones.RightHand]
        );

        // TwoBoneConstraints (Legs)
        leftLeg.setConstrainedObjects(
            root: boneTransformMapping[HumanBodyBones.LeftUpperLeg],
            mid: boneTransformMapping[HumanBodyBones.LeftLowerLeg],
            tip: boneTransformMapping[HumanBodyBones.LeftFoot]
        );
        rightLeg.setConstrainedObjects(
            root: boneTransformMapping[HumanBodyBones.RightUpperLeg],
            mid: boneTransformMapping[HumanBodyBones.RightLowerLeg],
            tip: boneTransformMapping[HumanBodyBones.RightFoot]
        );
    }

    private void prepareIKFollowerSetup()
    {
        IKTargetFollowVRRig ikFollower = vrmObject.GetOrAddComponent<IKTargetFollowVRRig>();
        ikFollower.others.Clear();
        foreach ((SteamVR_Input_Sources inputSource, IKConstraint constraint) in new (SteamVR_Input_Sources, IKConstraint)[] {
             (SteamVR_Input_Sources.Head, head),
             (SteamVR_Input_Sources.Chest, chest),
             (SteamVR_Input_Sources.Waist, waist),
             (SteamVR_Input_Sources.RightHand, rightArm),
             (SteamVR_Input_Sources.LeftHand, leftArm),
             (SteamVR_Input_Sources.RightFoot, rightLeg),
             (SteamVR_Input_Sources.LeftFoot, leftLeg)
        })
        {
            Tracker tracker = trackerManager.GetTracker(inputSource);
            if (tracker != null)
            {
                setIKFollowerMapping(ikFollower, vrTracker: tracker, ikConstraint: constraint);
            }
        }

    }
    private void setIKFollowerMapping(IKTargetFollowVRRig ikFollower, Tracker vrTracker, IKConstraint ikConstraint)
    {
        if (vrTracker.input_source == SteamVR_Input_Sources.Head)
        {
            ikFollower.head = new VRMap(ikTarget: ikConstraint.target, vrTarget: vrTracker.target, isUsed: vrTracker.isUsed);
        }
        else
        {
            ikFollower.others.Add(new VRMap(ikTarget: ikConstraint.target, vrTarget: vrTracker.target, isUsed: vrTracker.isUsed));
        }
    }
    private Dictionary<HumanBodyBones, Transform> constructBoneTransformMapping()
    {
        Dictionary<string, Transform> nameTransformMapping = constructNameTransformMapping(vrmObject);
        Dictionary<HumanBodyBones, Transform> boneTransformMapping = new();
        foreach (BoneLimit bone in bones)
        {
            boneTransformMapping.Add(bone.humanBone, nameTransformMapping[bone.boneName]);
        }
        return boneTransformMapping;
    }
    public Dictionary<string, Transform> constructNameTransformMapping(Transform obj)
    {
        Dictionary<string, Transform> nameTransformMapping = new();
        foreach (Transform child in obj.GetComponentsInChildren<Transform>())
        {
            nameTransformMapping.Add(child.name, child);
        }
        return nameTransformMapping;
    }
}
