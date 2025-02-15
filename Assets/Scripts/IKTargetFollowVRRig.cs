using UnityEngine;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform ikTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public bool isUsed = true;

    public void Map()
    {
        if (isUsed)
        {
            ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
            ikTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
        }
    }
}

public class IKTargetFollowVRRig : MonoBehaviour
{
    [Range(0, 1)] public float turnSmoothness = 0.1f;
    public VRMap head;
    public VRMap[] others;
    public Vector3 headBodyPositionOffset;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = head.ikTarget.position + headBodyPositionOffset;
        float yaw = head.vrTarget.eulerAngles.y;
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.Euler(transform.eulerAngles.x, yaw, transform.eulerAngles.z), turnSmoothness);

        head.Map();
        foreach (VRMap map in others)
        {
            map.Map();
        }
    }
}