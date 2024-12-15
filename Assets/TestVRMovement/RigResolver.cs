using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class RigBounds
{
    // Rigs' Transform components bounds min/max vertices
    public Transform BoundsCube, Min, Max;
}

[Serializable]
public class TransformMapping
{
    public Transform RigBone, Reference;
    public Vector3 trackingPositionOffset, trackingRotationOffset;
}

[Serializable]
public class RigTransform
{
    const int FLOATS_FOR_POSITION = 3;
    const int FLOATS_FOR_ROTATION = 3;
    const float DEGREES_360 = 360;

    public Transform Hips, LeftUpLeg, LeftLeg, LeftFoot, RightUpLeg, RightLeg, RightFoot, Spine, Spine1, Spine2,
        LeftShoulder, LeftArm, LeftForeArm, Neck, RightShoulder, RightArm, RightForeArm;
    public TransformMapping LeftHand, Head, RightHand;
    public RigBounds Bounds;

    private float[] GetBonesAsNormalizedArray(Transform[] bones, bool withRotation = true)
    {
        ResetBoundsRotation();

        List<float> vectors = new();
        foreach (Transform transform in bones)
        {
            Vector3 normalizedPosition = NormalizeBonePosition(transform.position);
            vectors.AddRange(new[] { normalizedPosition.x, normalizedPosition.y, normalizedPosition.z });
            if (withRotation)
            {
                Vector3 normalizedRotation = NormalizeBoneRotation(transform.rotation);
                vectors.AddRange(new[] { normalizedRotation.x, normalizedRotation.y, normalizedRotation.z });
            }
        }
        return vectors.ToArray();
    }
    public float[] GetInputReferenceBonesAsNormalizedArray(bool withRotation = true) { return GetBonesAsNormalizedArray(GetAllInputReferenceBones(), withRotation); }
    public float[] GetInputBonesAsNormalizedArray(bool withRotation = true) { return GetBonesAsNormalizedArray(GetAllInputBones(), withRotation); }
    public float[] GetOutputBonesAsNormalizedArray(bool withRotation = true) { return GetBonesAsNormalizedArray(GetAllOutputBones(), withRotation); }
    public float[][] GetInputOutputBonesAsNormalizedArray(bool withRotation = true)
    {
        // Get normalized position and rotations vector from all bones as array of float arrays [[inputs], [outputs]]
        return new[] { GetInputBonesAsNormalizedArray(withRotation), GetOutputBonesAsNormalizedArray(withRotation) };
    }

    private void SetBonesFromNormalizedArray(Transform[] bones, float[] array, bool withRotation = true)
    {
        ResetBoundsRotation();
        int floats_for_rotation = withRotation ? FLOATS_FOR_ROTATION : 0;
        Assert.AreEqual(bones.Length * (FLOATS_FOR_POSITION + floats_for_rotation), array.Length);
        int boneIndex = 0;
        foreach (Transform transform in bones)
        {
            var arrayAsList = array.AsReadOnlyList();
            float[] position = arrayAsList.Skip(boneIndex * (FLOATS_FOR_POSITION + floats_for_rotation)).Take(FLOATS_FOR_POSITION).ToArray();
            transform.position = DenormalizeBonePosition(new Vector3(position[0], position[1], position[2]));
            if (withRotation)
            {
                float[] rotation = arrayAsList.Skip(boneIndex * (FLOATS_FOR_POSITION + floats_for_rotation) + FLOATS_FOR_POSITION).Take(floats_for_rotation).ToArray();
                transform.rotation = DenormalizeBoneRotation(new Vector3(rotation[0], rotation[1], rotation[2]));
            }
            boneIndex++;
        }
    }
    public void SetInputBonesFromNormalizedArray(float[] inputs, bool withRotation = true) { SetBonesFromNormalizedArray(GetAllInputBones(), inputs, withRotation); }
    public void SetOutputBonesFromNormalizedArray(float[] outputs, bool withRotation = true) { SetBonesFromNormalizedArray(GetAllOutputBones(), outputs, withRotation); }
    public void SetInputOutputBonesFromNormalizedArray(float[] inputs, float[] outputs, bool withRotation = true)
    {
        // Set bones' positions and rotations based on arrays of float
        SetInputBonesFromNormalizedArray(inputs, withRotation); SetOutputBonesFromNormalizedArray(outputs, withRotation);
    }
    public void SetInputBonesAsReference()
    {
        foreach (TransformMapping mapping in new[] { Head, LeftHand, RightHand })
        {
            mapping.RigBone.SetPositionAndRotation(
                mapping.Reference.position + mapping.trackingPositionOffset,
                mapping.Reference.rotation * Quaternion.Euler(mapping.trackingRotationOffset)
            );
        }
    }

    private Transform[] GetAllBonesAsArray()
    {
        // Get all bones attached as attributes in a single Transform array
        var inputBones = GetAllInputBones();
        var outputBones = GetAllOutputBones();
        var mergedBones = new Transform[inputBones.Length + outputBones.Length];
        inputBones.CopyTo(mergedBones, 0);
        outputBones.CopyTo(mergedBones, inputBones.Length);
        return mergedBones;
    }

    private Transform[] GetAllInputBones()
    {
        return new[] { Head.RigBone, RightHand.RigBone, LeftHand.RigBone };
    }
    private Transform[] GetAllInputReferenceBones()
    {
        return new[] { Head.Reference, RightHand.Reference, LeftHand.Reference };
    }

    private Transform[] GetAllOutputBones()
    {
        return new[] { Hips, LeftUpLeg, LeftLeg, LeftFoot, RightUpLeg, RightLeg, RightFoot, Spine, Spine1, Spine2,
        LeftShoulder, LeftArm, LeftForeArm, Neck, RightShoulder, RightArm, RightForeArm};
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
