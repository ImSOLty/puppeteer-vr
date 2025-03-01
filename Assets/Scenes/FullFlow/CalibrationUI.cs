using System;
using System.Collections;
using System.Collections.Generic;
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

}

public class CalibrationUI : MonoBehaviour
{
    private IKTargetFollowVRRig ik;
    private SteamVR_Input_Sources currentBone;
    private bool meshesShown = true;
    [SerializeField] OverallProperties overallProperties;
    [SerializeField] BoneProperties boneProperties;
    [SerializeField] Renderer[] listOfRenderersToHide;

    private void Awake()
    {
        ik = FindObjectOfType<IKTargetFollowVRRig>();
    }

    public void ShowBoneProperties(SteamVR_Input_Sources source)
    {
        overallProperties.propertiesWindow.SetActive(false);
        boneProperties.propertiesWindow.SetActive(false);
        if (source == SteamVR_Input_Sources.Any)
        {
            overallProperties.propertiesWindow.SetActive(true);
            SetupOverallProperties();
        }
        else
        {
            boneProperties.propertiesWindow.SetActive(true);
            SetupBoneProperties(source);
        }
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
    }
    public void UpdateMeshShow()
    {
        meshesShown = !meshesShown;
        SetupOverallProperties();
    }
    private void SetupBoneProperties(SteamVR_Input_Sources source)
    {
        currentBone = source;
    }
    public void UpdateBoneProperties()
    {
        SteamVR_Input_Sources source = currentBone;
    }

    public void UpdateTrackersOnTemplate()
    {
        
    }
}
