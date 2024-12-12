using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


[Serializable]
public class KeyFrameData
{
    public float time;
    public float[] inputTransformsAsFloatArray;
    public float[] outputTransformsAsFloatArray;

    public KeyFrameData(float time, float[] inputs, float[] outputs)
    {
        this.time = time;
        this.inputTransformsAsFloatArray = inputs;
        this.outputTransformsAsFloatArray = outputs;
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
    public RigResolver rigResolver;

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

                foreach (var key in curve.keys)
                {
                    set.Add(key.time);
                }
            }
            Debug.Log(set.Count);
            foreach (var time in set)
            {
                clip.SampleAnimation(animator.gameObject, time);
                listWithData.AddNewKeyFrame(
                    new KeyFrameData(
                        time,
                        rigResolver.rigTransform.GetInputBonesAsNormalizedArray(),
                        rigResolver.rigTransform.GetOutputBonesAsNormalizedArray()
                    )
                );
            }
            File.WriteAllText(Path.Combine(Application.dataPath,
             "Helpers/_AnimationsData/raw",
              i + ".json"), JsonUtility.ToJson(listWithData));
            i++;
        }
    }
}
