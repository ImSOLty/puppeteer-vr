using System;
using Puppeteer;
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
    public PuppeteerBone source;
    public Button button;
}

public class CalibrationUI : MonoBehaviour
{
    private IKTargetFollowVRRig ik;
    private HandTrackingSolver[] handTrackingSolvers;
    private PuppeteerBone currentBone = PuppeteerBone.Any;
    private bool meshesShown = true;

    [SerializeField] Renderer[] renderersToHide;
    [SerializeField] TemplateTracker[] templateTrackers;

    [Header("UI Menus")]
    [SerializeField] OverallProperties overallProperties;
    [SerializeField] BoneProperties boneProperties;


    public void PanelActivate()
    {
        ik = FindObjectOfType<IKTargetFollowVRRig>();
        handTrackingSolvers = FindObjectsOfType<HandTrackingSolver>();
        renderersToHide = FindObjectsOfType<SkinnedMeshRenderer>();
        UpdateTrackersOnTemplate();
    }


    private void SetupOverallProperties()
    {
        overallProperties.smoothness.SetValueWithoutNotify(ik.turnSmoothness);
        overallProperties.overallPositionX.SetValueWithoutNotify(ik.headBodyPositionOffset.x);
        overallProperties.overallPositionY.SetValueWithoutNotify(ik.headBodyPositionOffset.y);
        overallProperties.overallPositionZ.SetValueWithoutNotify(ik.headBodyPositionOffset.z);
        overallProperties.meshes.targetGraphic.color = meshesShown ? Color.gray : Color.white;

    }
    public void UpdateOverallProperties()
    {
        ik.turnSmoothness = overallProperties.smoothness.value;
        ik.headBodyPositionOffset = new Vector3(
            overallProperties.overallPositionX.value,
            overallProperties.overallPositionY.value,
            overallProperties.overallPositionZ.value
        );
        ik.calibrationSettings.SetOverallSettings(ik.turnSmoothness, ik.headBodyPositionOffset);
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
    private void SetupBoneProperties(PuppeteerBone source)
    {
        currentBone = source;
        VRMap map = ik.GetVRMapFromSource(currentBone);

        boneProperties.name.text = currentBone.ToString();

        bool isUsed = (map != null) && map.isUsed;
        boneProperties.isUsed.targetGraphic.color = isUsed ? Color.green : Color.red;

        boneProperties.bonePositionX.SetValueWithoutNotify(map != null ? map.trackingPositionOffset.x : 0);
        boneProperties.bonePositionY.SetValueWithoutNotify(map != null ? map.trackingPositionOffset.y : 0);
        boneProperties.bonePositionZ.SetValueWithoutNotify(map != null ? map.trackingPositionOffset.z : 0);
        boneProperties.boneRotationX.SetValueWithoutNotify(map != null ? map.trackingRotationOffset.x : 0);
        boneProperties.boneRotationY.SetValueWithoutNotify(map != null ? map.trackingRotationOffset.y : 0);
        boneProperties.boneRotationZ.SetValueWithoutNotify(map != null ? map.trackingRotationOffset.z : 0);
    }
    public void UpdateBoneProperties()
    {
        VRMap map = ik.GetVRMapFromSource(currentBone);
        if (map != null)
        {
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
            ik.calibrationSettings.AddOrSetBoneSettings(currentBone, map.isUsed, map.trackingPositionOffset, map.trackingRotationOffset);
        }
        SetupBoneProperties(currentBone);
    }

    public void UpdateIsUsed()
    {
        VRMap map = ik.GetVRMapFromSource(currentBone);
        if (map == null)
        {
            return;
        }
        map.isUsed = !map.isUsed;
        UpdateBoneProperties();
        UpdateTrackersOnTemplate();
    }

    public void UpdateTrackersOnTemplate()
    {
        foreach (TemplateTracker templateTracker in templateTrackers)
        {
            VRMap map = ik.GetVRMapFromSource(templateTracker.source);
            bool isUsed = (map != null) && map.isUsed;
            templateTracker.button.targetGraphic.color = isUsed ? Color.green : Color.red;
        }
    }
    public void SaveCalibrationSettings()
    {
        Settings.Files.BodyCalibrationSettings.Write(JsonUtility.ToJson(ik.calibrationSettings));
    }
    public void LoadCalibrationSettings()
    {
        if (!Settings.Files.BodyCalibrationSettings.Exists())
        {
            return;
        }
        ik.LoadCalibrationSettings(JsonUtility.FromJson<BodyCalibrationSettings>(Settings.Files.BodyCalibrationSettings.Read()));
        SetupOverallProperties();

        if (currentBone != PuppeteerBone.Any)
        {
            SetupBoneProperties(currentBone);
        }
    }

    public void ShowOverallProperties()
    {
        boneProperties.propertiesWindow.SetActive(false);
        overallProperties.propertiesWindow.SetActive(true);
        SetupOverallProperties();
    }
    private void ShowBoneProperties(PuppeteerBone source)
    {
        overallProperties.propertiesWindow.SetActive(false);
        boneProperties.propertiesWindow.SetActive(true);
        SetupBoneProperties(source);
    }

    // Sadly OnClick handle in UI Button cannot take enum as an argument, thus multiple similar methods were created in order not to use strings:
    public void ShowBonePropertiesLeftHand() => ShowBoneProperties(PuppeteerBone.LeftHand);
    public void ShowBonePropertiesRightHand() => ShowBoneProperties(PuppeteerBone.RightHand);
    public void ShowBonePropertiesLeftFoot() => ShowBoneProperties(PuppeteerBone.LeftFoot);
    public void ShowBonePropertiesRightFoot() => ShowBoneProperties(PuppeteerBone.RightFoot);
    public void ShowBonePropertiesLeftShoulder() => ShowBoneProperties(PuppeteerBone.LeftShoulder);
    public void ShowBonePropertiesRightShoulder() => ShowBoneProperties(PuppeteerBone.RightShoulder);
    public void ShowBonePropertiesLeftElbow() => ShowBoneProperties(PuppeteerBone.LeftLowerArm);
    public void ShowBonePropertiesRightElbow() => ShowBoneProperties(PuppeteerBone.RightLowerArm);
    public void ShowBonePropertiesLeftKnee() => ShowBoneProperties(PuppeteerBone.LeftLowerLeg);
    public void ShowBonePropertiesRightKnee() => ShowBoneProperties(PuppeteerBone.RightLowerLeg);
    public void ShowBonePropertiesLeftAnkle() => ShowBoneProperties(PuppeteerBone.LeftAnkle);
    public void ShowBonePropertiesRightAnkle() => ShowBoneProperties(PuppeteerBone.RightAnkle);
    public void ShowBonePropertiesChest() => ShowBoneProperties(PuppeteerBone.Chest);
    public void ShowBonePropertiesWaist() => ShowBoneProperties(PuppeteerBone.Waist);
    public void ShowBonePropertiesHead() => ShowBoneProperties(PuppeteerBone.Head);
}