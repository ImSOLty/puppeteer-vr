using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RigBounds
{
    // Rigs' Transform components bounds min/max vertices
    public Transform BoundsCube, Min, Max;
}

[Serializable]
public class RigTransform
{
    const float DEGREES_360 = 360;

    public Transform Hips, LeftUpLeg, LeftLeg, LeftFoot, RightUpLeg, RightLeg, RightFoot, Spine, Spine1, Spine2,
        LeftShoulder, LeftArm, LeftForeArm, LeftHand, Neck, Head, RightShoulder, RightArm, RightForeArm, RightHand;

    public RigBounds Bounds;

    public float[] GetPositionRotationVectorsNormalizedAsArray()
    {
        // Get normalized position and rotations vector from all bones as array of floats

        ResetBoundsRotation();
        List<float> vectorsExtended = new();
        foreach (Transform transform in GetAllBonesAsArray())
        {
            Vector3 normalizedPosition = NormalizeBonePosition(transform.position);
            Vector3 normalizedRotation = NormalizeBoneRotation(transform.rotation);
            vectorsExtended.AddRange(new[] { normalizedPosition.x, normalizedPosition.y, normalizedPosition.z });
            vectorsExtended.AddRange(new[] { normalizedRotation.x, normalizedRotation.y, normalizedRotation.z });
        }
        return vectorsExtended.ToArray();
    }
    private Transform[] GetAllBonesAsArray()
    {
        // Get all bones attached as attributes in a single Transform array
        return new[]{Hips, LeftUpLeg, LeftLeg, LeftFoot, RightUpLeg, RightLeg, RightFoot, Spine, Spine1, Spine2,
        LeftShoulder, LeftArm, LeftForeArm, LeftHand, Neck, Head, RightShoulder, RightArm, RightForeArm, RightHand};
    }

    private Vector3 NormalizeBonePosition(Vector3 position)
    {
        // Returns the position of an argument position inside bounds with coordinates from 0 to 1
        Vector3 min = Bounds.Min.position, max = Bounds.Max.position;
        return new Vector3(
            (position.x - min.x) / (max.x - min.x),
            (position.y - min.y) / (max.x - min.x),
            (position.z - min.z) / (max.x - min.x)
        );
    }

    private Vector3 NormalizeBoneRotation(Quaternion rotation)
    {
        // Returns the position of an argument quaternion from 0 to 1 (in positive eulerAngles divided by 360 degrees)
        Vector3 rotationEuler = rotation.eulerAngles;

        float x = (rotationEuler.x % DEGREES_360 + DEGREES_360) % DEGREES_360;
        float y = (rotationEuler.y % DEGREES_360 + DEGREES_360) % DEGREES_360;
        float z = (rotationEuler.z % DEGREES_360 + DEGREES_360) % DEGREES_360;

        return new Vector3(x, y, z) / DEGREES_360;
    }

    private Vector3 DenormalizeBonePosition(Vector3 normalizedPosition)
    {
        // Returns an actual world position based on normalized position inside bounds with coordinates from 0 to 1
        Vector3 min = Bounds.Min.position, max = Bounds.Max.position;
        return new Vector3(
            normalizedPosition.x * (max.x - min.x) + min.x,
            normalizedPosition.y * (max.y - min.y) + min.y,
            normalizedPosition.z * (max.z - min.z) + min.z
        );
    }

    private Quaternion DenormalizeBoneRotation(Vector3 normalizedRotation)
    {
        // Returns an actual rotation in degrees based on normalized rotation from 0 to 1
        return Quaternion.Euler(normalizedRotation * DEGREES_360);
    }
    private void ResetBoundsRotation()
    {
        // Resets the Bounds rotation, should be called before any time normalization or denormalization is used
        Bounds.BoundsCube.rotation = Quaternion.Euler(Vector3.zero);
    }
}

public class RigResolver : MonoBehaviour
{
    [SerializeField]
    public RigTransform rigTransform; // Rig bones' Transform components
}
