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

    public TransformValues(Transform component)
    {
        position = component.position;
        rotation = component.rotation.eulerAngles;
    }
}
[Serializable]
public class RigTransform
{
    public TransformValues LeftUpLeg, LeftLeg, LeftFoot, RightUpLeg, RightLeg, RightFoot, Spine, Spine1, Spine2,
    LeftShoulder, LeftArm, LeftForeArm, LeftHand, Neck, Head, RightShoulder, RightArm, RightForeArm, RightHand;

    public RigTransform(RecordingObjects objects)
    {
        LeftUpLeg = new TransformValues(objects.LeftUpLeg);
        LeftLeg = new TransformValues(objects.LeftLeg);
        LeftFoot = new TransformValues(objects.LeftFoot);
        RightUpLeg = new TransformValues(objects.RightUpLeg);
        RightLeg = new TransformValues(objects.RightLeg);
        RightFoot = new TransformValues(objects.RightFoot);
        Spine = new TransformValues(objects.Spine);
        Spine1 = new TransformValues(objects.Spine1);
        Spine2 = new TransformValues(objects.Spine2);
        LeftShoulder = new TransformValues(objects.LeftShoulder);
        LeftArm = new TransformValues(objects.LeftArm);
        LeftForeArm = new TransformValues(objects.LeftForeArm);
        LeftHand = new TransformValues(objects.LeftHand);
        Neck = new TransformValues(objects.Neck);
        Head = new TransformValues(objects.Head);
        RightShoulder = new TransformValues(objects.RightShoulder);
        RightArm = new TransformValues(objects.RightArm);
        RightForeArm = new TransformValues(objects.RightForeArm);
        RightHand = new TransformValues(objects.RightHand);
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

[Serializable]
public class AnimationClipExtended
{
    public string name;
    public AnimationClip clip;
}

public class AnimationExtractor : MonoBehaviour
{
    public Animator animator;
    public AnimationClipExtended[] clips;
    public RecordingObjects recordingObjects;

    public Transform referenceObjectTransform;
    void Start()
    {
        foreach (AnimationClipExtended clipExtended in clips)//animator.runtimeAnimatorController.animationClips)
        {
            KeyFrameDataList listWithData = new();
            AnimationClip clip = clipExtended.clip;
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

                Debug.Log(binding.path + "/" + binding.propertyName);
                foreach (var key in curve.keys)
                {
                    set.Add(key.time);
                }
            }
            Debug.Log(set.Count);
            foreach (var time in set)
            {
                clip.SampleAnimation(animator.gameObject, time);
                listWithData.AddNewKeyFrame(new KeyFrameData(time, new RigTransform(recordingObjects)));
            }
            File.WriteAllText(Path.Combine(Application.dataPath,
             "Helpers/_AnimationsData/raw",
              clipExtended.name + ".json"), JsonUtility.ToJson(listWithData));
        }
    }
}
