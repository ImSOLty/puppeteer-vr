using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TimePositionRotation
{
    public float time;
    public float[] positions;
    public float[] rotations;

    public TimePositionRotation(float time, float[] positions, float[] rotations)
    {
        this.time = time;
        this.positions = positions;
        this.rotations = rotations;
    }
}
public class TransformsOnly : MonoBehaviour
{
    public Transform[] toTrack;
    public TimePositionRotation GetTransformsAsFloat(float time)
    {
        List<float> positionsResult = new();
        List<float> rotationsResult = new();
        foreach (Transform tf in toTrack)
        {
            Vector3 pos = tf.position;
            Vector3 rot = tf.rotation.eulerAngles;
            positionsResult.AddRange(new[] { pos.x, pos.y, pos.z });
            rotationsResult.AddRange(new[] { rot.x, rot.y, rot.z });
        }
        return new TimePositionRotation(time, positionsResult.ToArray(), rotationsResult.ToArray());
    }
}
