using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Palmmedia.ReportGenerator.Core.Common;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


[Serializable]
public class RecordingObjects
{
    public Transform LeftUpLeg, LeftLeg, LeftFoot, RightUpLeg, RightLeg, RightFoot, Spine, Spine1, Spine2,
    LeftShoulder, LeftArm, LeftForeArm, LeftHand, Neck, Head, RightShoulder, RightArm, RightForeArm, RightHand;
}

[Serializable]
public class TransformValues
{
    public Vector3 position;
    public Vector3 rotation;

    public TransformValues(Transform component, Transform referenceMin, Transform referenceMax)
    {
        position = NormalizePositionVectorByReferenceTransform(component.position, referenceMin, referenceMax);
        rotation = NormalizeRotationVector(component.rotation.eulerAngles);
    }

    public Vector3 NormalizePositionVectorByReferenceTransform(Vector3 vector, Transform referenceMin, Transform referenceMax)
    {
        Vector3 min, max;
        min = referenceMin.position;
        max = referenceMax.position;

        float normalizedX = (vector.x - min.x) / (max.x - min.x);
        float normalizedY = (vector.y - min.y) / (max.y - min.y);
        float normalizedZ = (vector.z - min.z) / (max.z - min.z);


        return new Vector3(normalizedX, normalizedY, normalizedZ);
    }
    public Vector3 NormalizeRotationVector(Vector3 vector)
    {
        float x = (vector.x % 360 + 360) % 360;
        float y = (vector.y % 360 + 360) % 360;
        float z = (vector.z % 360 + 360) % 360;
        return new Vector3(x, y, z) / 360;
    }
}
[Serializable]
public class RigTransform
{

    public TransformValues LeftUpLeg, LeftLeg, LeftFoot, RightUpLeg, RightLeg, RightFoot, Spine, Spine1, Spine2,
    LeftShoulder, LeftArm, LeftForeArm, LeftHand, Neck, Head, RightShoulder, RightArm, RightForeArm, RightHand;

    public RigTransform(RecordingObjects objects, Transform max, Transform min)
    {
        LeftUpLeg = new TransformValues(objects.LeftUpLeg, min, max);
        LeftLeg = new TransformValues(objects.LeftLeg, min, max);
        LeftFoot = new TransformValues(objects.LeftFoot, min, max);
        RightUpLeg = new TransformValues(objects.RightUpLeg, min, max);
        RightLeg = new TransformValues(objects.RightLeg, min, max);
        RightFoot = new TransformValues(objects.RightFoot, min, max);
        Spine = new TransformValues(objects.Spine, min, max);
        Spine1 = new TransformValues(objects.Spine1, min, max);
        Spine2 = new TransformValues(objects.Spine2, min, max);
        LeftShoulder = new TransformValues(objects.LeftShoulder, min, max);
        LeftArm = new TransformValues(objects.LeftArm, min, max);
        LeftForeArm = new TransformValues(objects.LeftForeArm, min, max);
        LeftHand = new TransformValues(objects.LeftHand, min, max);
        Neck = new TransformValues(objects.Neck, min, max);
        Head = new TransformValues(objects.Head, min, max);
        RightShoulder = new TransformValues(objects.RightShoulder, min, max);
        RightArm = new TransformValues(objects.RightArm, min, max);
        RightForeArm = new TransformValues(objects.RightForeArm, min, max);
        RightHand = new TransformValues(objects.RightHand, min, max);
    }
}

[Serializable]
public class KeyFrameData
{
    public float time;
    public RigTransform rigTransform;

    public KeyFrameData(float time, RigTransform rigTransform)
    {
        this.time = time;
        this.rigTransform = rigTransform;
    }
}

[Serializable]
public class KeyFrameDataList
{
    public List<KeyFrameData> keyFrameDatas = new();

    public void AddNewKeyFrame(KeyFrameData data)
    {
        keyFrameDatas.Add(data);
    }
}


public class AnimationExtractor : MonoBehaviour
{
    public Animator animator;
    public RecordingObjects recordingObjects;

    public Transform referenceObjectTransform, referenceMin, referenceMax;
    void Start()
    {
        int i = 0;
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            KeyFrameDataList listWithData = new();
            // define time for unique keyframes
            HashSet<float> set = new HashSet<float>();
            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

                if (
                    binding.path.ToLower().Contains("toe") ||
                binding.path.ToLower().Contains("index") ||
                binding.path.ToLower().Contains("middle") ||
                binding.path.ToLower().Contains("pinky") ||
                binding.path.ToLower().Contains("thumb") ||
                binding.path.ToLower().Contains("ring")
                )
                {
                    continue; // skip fingers
                }

                if (binding.propertyName.ToLower().Contains("scale"))
                {
                    continue; // skip all scale properties
                }

                // Debug.Log(binding.path + "/" + binding.propertyName);
                foreach (var key in curve.keys)
                {
                    set.Add(key.time);
                }
            }
            Debug.Log(set.Count);
            foreach (var time in set)
            {
                clip.SampleAnimation(animator.gameObject, time);
                referenceObjectTransform.rotation = Quaternion.Euler(Vector3.zero);
                listWithData.AddNewKeyFrame(new KeyFrameData(time, new RigTransform(recordingObjects, referenceMin, referenceMax)));
            }
            File.WriteAllText(Path.Combine(Application.dataPath,
             "Helpers/_AnimationsData/raw",
              i + ".json"), JsonUtility.ToJson(listWithData));
            i++;
        }
    }
}
