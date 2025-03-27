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
    public Slider smoothnessSlider;
    public Slider overallPositionX, overallPositionY, overallPositionZ;
    public Button meshes;

    public void SetupPositionSlidersMaxValue(float maxValue)
    {
        overallPositionX.minValue = -maxValue; overallPositionX.maxValue = maxValue;
        overallPositionY.minValue = -maxValue; overallPositionY.maxValue = maxValue;
        overallPositionZ.minValue = -maxValue; overallPositionZ.maxValue = maxValue;
    }

    public void SetValuesWithoutNotify(Vector3 position, float smoothness)
    {
        overallPositionX.SetValueWithoutNotify(position.x);
        overallPositionY.SetValueWithoutNotify(position.y);
        overallPositionZ.SetValueWithoutNotify(position.z);
        smoothnessSlider.SetValueWithoutNotify(smoothness);
    }
}

[Serializable]
class BoneProperties : Properties
{
    public Button isUsed;
    public Slider bonePositionX, bonePositionY, bonePositionZ;
    public Slider boneRotationX, boneRotationY, boneRotationZ;
    public Text name;

    public void SetupPositionSlidersMaxValue(float maxValue)
    {
        bonePositionX.minValue = -maxValue; bonePositionX.maxValue = maxValue;
        bonePositionY.minValue = -maxValue; bonePositionY.maxValue = maxValue;
        bonePositionZ.minValue = -maxValue; bonePositionZ.maxValue = maxValue;
    }
    public void SetValuesWithoutNotify(Vector3 position, Vector3 rotation)
    {
        bonePositionX.SetValueWithoutNotify(position.x);
        bonePositionY.SetValueWithoutNotify(position.y);
        bonePositionZ.SetValueWithoutNotify(position.z);
        boneRotationX.SetValueWithoutNotify(rotation.x);
        boneRotationY.SetValueWithoutNotify(rotation.y);
        boneRotationZ.SetValueWithoutNotify(rotation.z);
    }
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
    private HandTrackingSolver handTrackingSolver; // Any of two
    private PuppeteerBone currentBone = PuppeteerBone.Any;
    private bool meshesShown = true;

    [SerializeField] Renderer[] renderersToHide;
    [SerializeField] TemplateTracker[] templateTrackers;

    [Header("UI Menus")]
    [SerializeField] OverallProperties overallProperties;
    [SerializeField] BoneProperties boneProperties;
    [SerializeField] GameObject rigTemplate, handTemplate;
    [SerializeField] float maxBodyOffset = 2, maxFingerOffset = 0.1f, maxBoneOffset = 1;
    private GameObject currentTemplate;


    public void PanelActivate()
    {
        ik = FindObjectOfType<IKTargetFollowVRRig>();
        handTrackingSolver = FindObjectOfType<HandTrackingSolver>();
        renderersToHide = FindObjectsOfType<SkinnedMeshRenderer>();

        currentTemplate = rigTemplate;

        SetupOverallProperties();
        UpdateTrackersOnTemplate();
    }


    private void SetupOverallProperties()
    {
        overallProperties.SetValuesWithoutNotify(position: ik.headBodyPositionOffset, smoothness: ik.turnSmoothness);
        overallProperties.SetupPositionSlidersMaxValue(maxBodyOffset);
        overallProperties.meshes.targetGraphic.color = meshesShown ? Color.gray : Color.white;

    }
    public void UpdateOverallProperties()
    {
        ik.turnSmoothness = overallProperties.smoothnessSlider.value;
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
        bool isUsed;
        Vector3 position, rotation;
        float maxValue;
        if (BonesMapping.IsFinger(currentBone))
        {
            FingerPartMap map = handTrackingSolver.GetFingerPartMap(currentBone);
            isUsed = (map != null) && handTrackingSolver.handMap.isUsed;
            position = map != null ? map.positionOffset : Vector3.zero;
            rotation = map != null ? map.rotationOffset : Vector3.zero;
            maxValue = maxFingerOffset;
        }
        else
        {
            VRMap map = ik.GetVRMapFromSource(currentBone);
            isUsed = (map != null) && map.isUsed;
            position = map != null ? map.trackingPositionOffset : Vector3.zero;
            rotation = map != null ? map.trackingRotationOffset : Vector3.zero;
            maxValue = maxBoneOffset;
        }

        boneProperties.name.text = currentBone.ToString();
        boneProperties.isUsed.targetGraphic.color = isUsed ? Color.green : Color.red;

        boneProperties.SetValuesWithoutNotify(position: position, rotation: rotation);
        boneProperties.SetupPositionSlidersMaxValue(maxValue);
    }

    public void UpdateBoneProperties()
    {
        Vector3 positionOffset = new(boneProperties.bonePositionX.value, boneProperties.bonePositionY.value, boneProperties.bonePositionZ.value);
        Vector3 rotationOffset = new(boneProperties.boneRotationX.value, boneProperties.boneRotationY.value, boneProperties.boneRotationZ.value);
        if (BonesMapping.IsFinger(currentBone))
        {
            FingerPartMap map = handTrackingSolver.GetFingerPartMap(currentBone);
            if (map == null) { return; }
            map.positionOffset = positionOffset;
            map.rotationOffset = rotationOffset;
            handTrackingSolver.CopyCalibrationToOtherHand();
        }
        else
        {
            VRMap map = ik.GetVRMapFromSource(currentBone);
            if (map == null) { return; }
            map.trackingPositionOffset = positionOffset;
            map.trackingRotationOffset = rotationOffset;
            ik.calibrationSettings.AddOrSetBoneSettings(currentBone, map.isUsed, map.trackingPositionOffset, map.trackingRotationOffset);
        }
        SetupBoneProperties(currentBone);
    }

    public void UpdateIsUsed()
    {
        if (BonesMapping.IsFinger(currentBone))
        {
            handTrackingSolver.handMap.isUsed = !handTrackingSolver.handMap.isUsed;
        }
        else
        {
            VRMap map = ik.GetVRMapFromSource(currentBone);
            if (map == null) { return; }
            map.isUsed = !map.isUsed;
        }
        UpdateBoneProperties();
        UpdateTrackersOnTemplate();
    }

    public void UpdateTrackersOnTemplate()
    {
        foreach (TemplateTracker templateTracker in templateTrackers)
        {
            bool isUsed;
            if (BonesMapping.IsFinger(templateTracker.source))
            {
                isUsed = handTrackingSolver.handMap.isUsed;
            }
            else
            {
                VRMap map = ik.GetVRMapFromSource(templateTracker.source);
                isUsed = (map != null) && map.isUsed;
            }
            templateTracker.button.targetGraphic.color = isUsed ? Color.green : Color.red;
        }
    }
    public void SaveCalibrationSettings()
    {
        Settings.Files.BodyCalibrationSettings.Write(JsonUtility.ToJson(ik.calibrationSettings, prettyPrint: true));
        Settings.Files.HandCalibrationSettings.Write(JsonUtility.ToJson(handTrackingSolver.handMap, prettyPrint: true));
    }
    public void LoadCalibrationSettings()
    {
        if (Settings.Files.BodyCalibrationSettings.Exists())
        {
            ik.LoadCalibrationSettings(JsonUtility.FromJson<BodyCalibrationSettings>(Settings.Files.BodyCalibrationSettings.Read()));
            SetupOverallProperties();
        }
        if (Settings.Files.HandCalibrationSettings.Exists())
        {
            handTrackingSolver.LoadCalibrationSettings(JsonUtility.FromJson<HandCalibrationSettings>(Settings.Files.HandCalibrationSettings.Read()));
            handTrackingSolver.CopyCalibrationToOtherHand();
        }

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
    public void SwitchTemplates()
    {
        currentTemplate = currentTemplate == rigTemplate ? handTemplate : rigTemplate;
        rigTemplate.SetActive(currentTemplate == rigTemplate);
        handTemplate.SetActive(currentTemplate == handTemplate);
        overallProperties.propertiesWindow.SetActive(false);
        boneProperties.propertiesWindow.SetActive(false);
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
    public void ShowBonePropertiesThumbProximal() => ShowBoneProperties(PuppeteerBone.AnyThumbProximal);
    public void ShowBonePropertiesThumbIntermediate() => ShowBoneProperties(PuppeteerBone.AnyThumbIntermediate);
    public void ShowBonePropertiesThumbDistal() => ShowBoneProperties(PuppeteerBone.AnyThumbDistal);
    public void ShowBonePropertiesIndexProximal() => ShowBoneProperties(PuppeteerBone.AnyIndexProximal);
    public void ShowBonePropertiesIndexIntermediate() => ShowBoneProperties(PuppeteerBone.AnyIndexIntermediate);
    public void ShowBonePropertiesIndexDistal() => ShowBoneProperties(PuppeteerBone.AnyIndexDistal);
    public void ShowBonePropertiesMiddleProximal() => ShowBoneProperties(PuppeteerBone.AnyMiddleProximal);
    public void ShowBonePropertiesMiddleIntermediate() => ShowBoneProperties(PuppeteerBone.AnyMiddleIntermediate);
    public void ShowBonePropertiesMiddleDistal() => ShowBoneProperties(PuppeteerBone.AnyMiddleDistal);
    public void ShowBonePropertiesRingProximal() => ShowBoneProperties(PuppeteerBone.AnyRingProximal);
    public void ShowBonePropertiesRingIntermediate() => ShowBoneProperties(PuppeteerBone.AnyRingIntermediate);
    public void ShowBonePropertiesRingDistal() => ShowBoneProperties(PuppeteerBone.AnyRingDistal);
    public void ShowBonePropertiesLittleProximal() => ShowBoneProperties(PuppeteerBone.AnyLittleProximal);
    public void ShowBonePropertiesLittleIntermediate() => ShowBoneProperties(PuppeteerBone.AnyLittleIntermediate);
    public void ShowBonePropertiesLittleDistal() => ShowBoneProperties(PuppeteerBone.AnyLittleDistal);
}