using System;
using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using UniHumanoid;
using UnityEngine;
using UnityEngine.Assertions;
using Valve.VR;

[Serializable]
public class CalibrationSettings
{
    [Serializable]
    public class BoneSettings
    {
        public SteamVR_Input_Sources source;
        public bool isUsed;
        public Vector3 positionOffset, rotationOffset;

        public BoneSettings(SteamVR_Input_Sources source, bool isUsed, Vector3 positionOffset, Vector3 rotationOffset)
        {
            this.source = source; this.isUsed = isUsed; this.positionOffset = positionOffset; this.rotationOffset = rotationOffset;
        }
    }
    [Serializable]
    public class OverallSettings
    {
        public float smoothness;
        public Vector3 positionOffset;
    }
    public Dictionary<SteamVR_Input_Sources, int> sourceIndex = new();
    public List<BoneSettings> boneSettings = new();
    public OverallSettings overallSettings = new();
    public void AddOrSetBoneSettings(SteamVR_Input_Sources source, bool isUsed, Vector3 positionOffset, Vector3 rotationOffset)
    {
        if (sourceIndex.ContainsKey(source))
        {
            boneSettings[sourceIndex[source]] = new BoneSettings(source, isUsed, positionOffset, rotationOffset);
        }
        else
        {
            sourceIndex.Add(source, boneSettings.Count);
            boneSettings.Add(new BoneSettings(source, isUsed, positionOffset, rotationOffset));
        }
    }

    public void SetOverallSettings(float smoothness, Vector3 positionOffset)
    {
        overallSettings.smoothness = smoothness;
        overallSettings.positionOffset = positionOffset;
    }
}

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform ikTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public bool isUsed = true;

    public VRMap(Transform vrTarget, Transform ikTarget, bool isUsed = true, Vector3 trackingPositionOffset = default, Vector3 trackingRotationOffset = default)
    {
        this.vrTarget = vrTarget;
        this.ikTarget = ikTarget;
        this.isUsed = isUsed;
        this.trackingPositionOffset = trackingPositionOffset;
        this.trackingRotationOffset = trackingRotationOffset;
    }

    public void Map()
    {
        if (isUsed)
        {
            ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
            ikTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
        }
        else
        {
            var ikSolver = ikTarget.GetComponent<IKSolver>();
            if (ikSolver != null)
            {
                ikSolver.Solve();
            }
        }
    }
}

[Serializable]
public class Mapping
{
    [Serializable]
    public class VRBoneMapping
    {
        public SteamVR_Input_Sources source;
        public VRMap vrMap;
        public VRBoneMapping(SteamVR_Input_Sources key, VRMap value)
        {
            source = key; vrMap = value;
        }
    }
    public List<VRBoneMapping> boneMap = new();
    public void SetFromDictionary(Dictionary<SteamVR_Input_Sources, VRMap> sourceVRMapMapping)
    {
        boneMap.Clear();
        foreach (SteamVR_Input_Sources key in sourceVRMapMapping.Keys)
        {
            VRMap value = sourceVRMapMapping[key];
            boneMap.Add(new VRBoneMapping(key, value));
        }
    }
}

public class IKTargetFollowVRRig : MonoBehaviour
{
    [Header("Overall properties")]
    [Range(0, 1)] public float turnSmoothness = 0.1f;
    private Dictionary<SteamVR_Input_Sources, VRMap> sourceVRMapMapping = new();
    public Vector3 headBodyPositionOffset;
    [SerializeField] private Mapping mapping = new();
    public CalibrationSettings calibrationSettings = new();


    // Update is called once per frame
    void LateUpdate()
    {
        VRMap head = GetVRMapFromSource(SteamVR_Input_Sources.Head);
        transform.position = head.ikTarget.position + headBodyPositionOffset;
        float yaw = head.vrTarget.eulerAngles.y;
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z), turnSmoothness);


        foreach (Mapping.VRBoneMapping map in mapping.boneMap)
        {
            map.vrMap.Map();
        }
    }

    public void AddOrSetSourceVRMapMapping(SteamVR_Input_Sources source, VRMap map)
    {
        if (sourceVRMapMapping.ContainsKey(source))
        {
            sourceVRMapMapping[source] = map;
        }
        else
        {
            sourceVRMapMapping.Add(source, map);
        }
        mapping.SetFromDictionary(sourceVRMapMapping);
    }
    public VRMap GetVRMapFromSource(SteamVR_Input_Sources source)
    {
        if (sourceVRMapMapping.ContainsKey(source))
        {
            return sourceVRMapMapping[source];
        }
        return null;
    }
    public void ResetMappings()
    {
        sourceVRMapMapping.Clear();
    }

    public void LoadCalibrationSettings(CalibrationSettings from)
    {
        calibrationSettings = from;
        turnSmoothness = calibrationSettings.overallSettings.smoothness;
        headBodyPositionOffset = calibrationSettings.overallSettings.positionOffset;

        foreach (CalibrationSettings.BoneSettings boneSetting in calibrationSettings.boneSettings)
        {
            VRMap map = GetVRMapFromSource(boneSetting.source);
            Assert.IsNotNull(map);

            map.trackingPositionOffset = boneSetting.positionOffset;
            map.trackingRotationOffset = boneSetting.rotationOffset;
            map.isUsed = boneSetting.isUsed;
        }
    }
}