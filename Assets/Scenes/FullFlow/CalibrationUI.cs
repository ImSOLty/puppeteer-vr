using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

[Serializable]
public class Properties
{
    public GameObject propertiesWindow;

}
[Serializable]
class OverallProperties : Properties
{
    public Slider smoothness;
    public Slider overallPositionX, overallPositionY, overallPositionZ;
    public Button meshes;
}

[Serializable]
class BoneProperties : Properties
{
    public Button isUsed;
    public Slider bonePositionX, bonePositionY, bonePositionZ;
    public Slider boneRotationX, boneRotationY, boneRotationZ;
    public Text name;
}

[Serializable]
class TemplateTracker
{
    public SteamVR_Input_Sources source;
    public Button button;
}

public class CalibrationUI : MonoBehaviour
{
    private IKTargetFollowVRRig ik;
    private SteamVR_Input_Sources currentBone;
    private bool meshesShown = true;
    [SerializeField] Renderer[] renderersToHide;
    [SerializeField] TemplateTracker[] templateTrackers;

    [Header("UI Menus")]
    [SerializeField] OverallProperties overallProperties;
    [SerializeField] BoneProperties boneProperties;


    private void Start()
    {
        ik = FindObjectOfType<IKTargetFollowVRRig>();
        renderersToHide = FindObjectsOfType<SkinnedMeshRenderer>();
    }



    private void SetupOverallProperties()
    {
        overallProperties.smoothness.value = ik.turnSmoothness;
        overallProperties.overallPositionX.value = ik.headBodyPositionOffset.x;
        overallProperties.overallPositionY.value = ik.headBodyPositionOffset.y;
        overallProperties.overallPositionZ.value = ik.headBodyPositionOffset.z;
        overallProperties.meshes.targetGraphic.color = meshesShown ? Color.black : Color.white;
    }
    public void UpdateOverallProperties()
    {
        ik.turnSmoothness = overallProperties.smoothness.value;
        ik.headBodyPositionOffset = new Vector3(
            overallProperties.overallPositionX.value,
            overallProperties.overallPositionY.value,
            overallProperties.overallPositionZ.value
        );
        SetupOverallProperties();
    }
    public void UpdateMeshShow()
    {
        meshesShown = !meshesShown;
        SetupOverallProperties();
        foreach (Renderer renderer in renderersToHide)
        {
            renderer.enabled = meshesShown;
        }
    }
    private void SetupBoneProperties(SteamVR_Input_Sources source)
    {
        currentBone = source;
        VRMap map = ik.GetVRMapFromSource(currentBone);

        boneProperties.bonePositionX.value = map.trackingPositionOffset.x;
        boneProperties.bonePositionY.value = map.trackingPositionOffset.y;
        boneProperties.bonePositionZ.value = map.trackingPositionOffset.z;
        boneProperties.boneRotationX.value = map.trackingRotationOffset.x;
        boneProperties.boneRotationY.value = map.trackingRotationOffset.y;
        boneProperties.boneRotationZ.value = map.trackingRotationOffset.z;

        boneProperties.name.text = currentBone.ToString();
        boneProperties.isUsed.targetGraphic.color = map.isUsed ? Color.green : Color.red;
    }
    public void UpdateBoneProperties()
    {
        VRMap map = ik.GetVRMapFromSource(currentBone);
        map.trackingPositionOffset = new Vector3(
            boneProperties.bonePositionX.value,
            boneProperties.bonePositionY.value,
            boneProperties.bonePositionZ.value
        );
        map.trackingRotationOffset = new Vector3(
            boneProperties.boneRotationX.value,
            boneProperties.boneRotationY.value,
            boneProperties.boneRotationZ.value
        );
        SetupBoneProperties(currentBone);
    }

    public void UpdateIsUsed()
    {
        VRMap map = ik.GetVRMapFromSource(currentBone);
        map.isUsed = !map.isUsed;
        SetupBoneProperties(currentBone);
        UpdateTrackersOnTemplate();
    }

    public void UpdateTrackersOnTemplate()
    {
        foreach (TemplateTracker templateTracker in templateTrackers)
        {
            VRMap map = ik.GetVRMapFromSource(templateTracker.source);
            templateTracker.button.targetGraphic.color = map.isUsed ? Color.green : Color.red;
        }
    }

    public void ShowOverallProperties()
    {
        boneProperties.propertiesWindow.SetActive(false);
        overallProperties.propertiesWindow.SetActive(true);
        SetupOverallProperties();
    }
    private void ShowBoneProperties(SteamVR_Input_Sources source)
    {
        overallProperties.propertiesWindow.SetActive(false);
        boneProperties.propertiesWindow.SetActive(true);
        SetupBoneProperties(source);
    }

    // Sadly OnClick handle in UI Button cannot take enum as an argument, thus multiple similar methods were created in order not to use strings:
    public void ShowBonePropertiesLeftHand() => ShowBoneProperties(SteamVR_Input_Sources.LeftHand);
    public void ShowBonePropertiesRightHand() => ShowBoneProperties(SteamVR_Input_Sources.RightHand);
    public void ShowBonePropertiesLeftFoot() => ShowBoneProperties(SteamVR_Input_Sources.LeftFoot);
    public void ShowBonePropertiesRightFoot() => ShowBoneProperties(SteamVR_Input_Sources.RightFoot);
    public void ShowBonePropertiesLeftShoulder() => ShowBoneProperties(SteamVR_Input_Sources.LeftShoulder);
    public void ShowBonePropertiesRightShoulder() => ShowBoneProperties(SteamVR_Input_Sources.RightShoulder);
    public void ShowBonePropertiesLeftElbow() => ShowBoneProperties(SteamVR_Input_Sources.LeftElbow);
    public void ShowBonePropertiesRightElbow() => ShowBoneProperties(SteamVR_Input_Sources.RightElbow);
    public void ShowBonePropertiesLeftKnee() => ShowBoneProperties(SteamVR_Input_Sources.LeftKnee);
    public void ShowBonePropertiesRightKnee() => ShowBoneProperties(SteamVR_Input_Sources.RightKnee);
    public void ShowBonePropertiesLeftAnkle() => ShowBoneProperties(SteamVR_Input_Sources.LeftAnkle);
    public void ShowBonePropertiesRightAnkle() => ShowBoneProperties(SteamVR_Input_Sources.RightAnkle);
    public void ShowBonePropertiesChest() => ShowBoneProperties(SteamVR_Input_Sources.Chest);
    public void ShowBonePropertiesWaist() => ShowBoneProperties(SteamVR_Input_Sources.Waist);
    public void ShowBonePropertiesHead() => ShowBoneProperties(SteamVR_Input_Sources.Head);
}