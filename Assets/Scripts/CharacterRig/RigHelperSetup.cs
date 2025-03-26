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
    private Dictionary<PuppeteerBone, Transform> boneTransformMapping = new();
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

        prepareHandTrackingSetup();

        return true;
    }
    private void prepareSetup()
    {
        boneTransformMapping = constructBoneTransformMapping();

        // MultiParentConstraints
        waist.setConstrainedObjects(boneTransformMapping[PuppeteerBone.Spine]);
        chest.setConstrainedObjects(boneTransformMapping[PuppeteerBone.Chest]);
        head.setConstrainedObjects(boneTransformMapping[PuppeteerBone.Head]);

        // TwoBoneConstraints (Arms)
        leftArm.setConstrainedObjects(
            root: boneTransformMapping[PuppeteerBone.LeftUpperArm],
            mid: boneTransformMapping[PuppeteerBone.LeftLowerArm],
            tip: boneTransformMapping[PuppeteerBone.LeftHand]
        );
        rightArm.setConstrainedObjects(
            root: boneTransformMapping[PuppeteerBone.RightUpperArm],
            mid: boneTransformMapping[PuppeteerBone.RightLowerArm],
            tip: boneTransformMapping[PuppeteerBone.RightHand]
        );

        // TwoBoneConstraints (Legs)
        leftLeg.setConstrainedObjects(
            root: boneTransformMapping[PuppeteerBone.LeftUpperLeg],
            mid: boneTransformMapping[PuppeteerBone.LeftLowerLeg],
            tip: boneTransformMapping[PuppeteerBone.LeftFoot]
        );
        rightLeg.setConstrainedObjects(
            root: boneTransformMapping[PuppeteerBone.RightUpperLeg],
            mid: boneTransformMapping[PuppeteerBone.RightLowerLeg],
            tip: boneTransformMapping[PuppeteerBone.RightFoot]
        );
    }

    private void prepareIKFollowerSetup()
    {
        IKTargetFollowVRRig ikFollower = vrmObject.GetOrAddComponent<IKTargetFollowVRRig>();
        ikFollower.ResetMappings();
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
        if (!Settings.Files.BodyCalibrationSettings.Exists())
        {
            Settings.Files.BodyCalibrationSettings.Write(JsonUtility.ToJson(ikFollower.calibrationSettings));
        }
        ikFollower.LoadCalibrationSettings(JsonUtility.FromJson<BodyCalibrationSettings>(Settings.Files.BodyCalibrationSettings.Read()));
    }

    private void prepareHandTrackingSetup()
    {
        GameObject handSolversObject = new("HandSolvers");
        foreach (PuppeteerBone handBone in new[] { PuppeteerBone.LeftHand, PuppeteerBone.RightHand })
        {
            // Define if is righthand
            bool isRightHand = handBone == PuppeteerBone.RightHand;

            // Create HandSolvers and place as separate objects, so they won't be recorded
            GameObject handSolver = new((isRightHand ? "Right" : "Left") + "HandSolver");
            handSolver.transform.SetParent(handSolversObject.transform);
            HandTrackingSolver solver = handSolver.AddComponent<HandTrackingSolver>();

            if (!Settings.Files.HandCalibrationSettings.Exists())
            {
                Settings.Files.HandCalibrationSettings.Write(JsonUtility.ToJson(solver.handMap));
            }
            solver.LoadCalibrationSettings(JsonUtility.FromJson<HandCalibrationSettings>(Settings.Files.HandCalibrationSettings.Read()));
            solver.Setup(rightHand: isRightHand, mapping: boneTransformMapping);
        }
    }

    private void setIKFollowerMapping(IKTargetFollowVRRig ikFollower, Tracker vrTracker, IKConstraint ikConstraint)
    {
        ikFollower.AddOrSetSourceVRMapMapping(
            source: Puppeteer.BonesMapping.FromSteamVR(vrTracker.input_source),
            map: new VRMap(ikTarget: ikConstraint.target, vrTarget: vrTracker.target, isUsed: vrTracker.isUsed)
        );
    }
    private Dictionary<PuppeteerBone, Transform> constructBoneTransformMapping()
    {
        Dictionary<string, Transform> nameTransformMapping = constructNameTransformMapping(vrmObject);
        Dictionary<PuppeteerBone, Transform> boneTransformMapping = new();
        foreach (BoneLimit bone in bones)
        {
            boneTransformMapping.Add(Puppeteer.BonesMapping.FromHumanBodyBone(bone.humanBone), nameTransformMapping[bone.boneName]);
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
