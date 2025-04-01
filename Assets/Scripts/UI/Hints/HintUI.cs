using UnityEngine;
using Valve.VR;

public class HintUI : MonoBehaviour
{
    [SerializeField] private UIReactiveManager uiReactiveManager;
    public SteamVR_Behaviour_Pose handPose;
    private SteamVR_Action_Boolean xPressAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("XPress");
    [SerializeField] private PlayerHint hint;
    void Update()
    {
        if (xPressAction.GetStateDown(handPose.inputSource))
        {
            uiReactiveManager.UpdateUserUITransform();
            hint.gameObject.SetActive(true);
            hint.Setup();
        }
        if (xPressAction.GetStateUp(handPose.inputSource))
        {
            hint.gameObject.SetActive(false);
        }
    }
}
