using System;
using System.Collections.Generic;
using System.Linq;
using UniHumanoid;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using VRM;

public class IKConstraint
{
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
    }
}

[Serializable]
public class IKSetupTwoBoneConstraint : IKConstraint
{
    public TwoBoneIKConstraint component;
    public Transform root, mid, tip;
    public override void constraintSet()
    {
        component.data.root = root;
        component.data.mid = mid;
        component.data.tip = tip;
    }
}


public class RigHelperSetup : MonoBehaviour
{

    [SerializeField] private IKSetupMultiParentConstraint waist, chest, head;
    [SerializeField] private IKSetupTwoBoneConstraint leftArm, rightArm, leftLeg, rightLeg;
    private Dictionary<HumanBodyBones, Transform> boneTransformMapping = new();
    private Transform vrmObject;
    private BoneLimit[] bones;
    private bool sourceProvided = false;

    public void provideSources(Transform vrm)
    {
        vrmObject = vrm;
        bones = vrm.GetComponent<VRMHumanoidDescription>().Description.human;

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

        return true;
    }
    private void prepareSetup()
    {
        boneTransformMapping = constructBoneTransformMapping();

        // MultiParentConstraints
        waist.constrainedObject = boneTransformMapping[HumanBodyBones.Spine];
        chest.constrainedObject = boneTransformMapping[HumanBodyBones.Chest];
        head.constrainedObject = boneTransformMapping[HumanBodyBones.Head];

        // TwoBoneConstraints (Arms)
        leftArm.root = boneTransformMapping[HumanBodyBones.LeftUpperArm];
        leftArm.mid = boneTransformMapping[HumanBodyBones.LeftLowerArm];
        leftArm.tip = boneTransformMapping[HumanBodyBones.LeftHand];
        rightArm.root = boneTransformMapping[HumanBodyBones.RightUpperArm];
        rightArm.mid = boneTransformMapping[HumanBodyBones.RightLowerArm];
        rightArm.tip = boneTransformMapping[HumanBodyBones.RightHand];

        // TwoBoneConstraints (Legs)
        leftLeg.root = boneTransformMapping[HumanBodyBones.LeftUpperLeg];
        leftLeg.mid = boneTransformMapping[HumanBodyBones.LeftLowerLeg];
        leftLeg.tip = boneTransformMapping[HumanBodyBones.LeftFoot];
        rightLeg.root = boneTransformMapping[HumanBodyBones.RightUpperLeg];
        rightLeg.mid = boneTransformMapping[HumanBodyBones.RightLowerLeg];
        rightLeg.tip = boneTransformMapping[HumanBodyBones.RightFoot];

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
