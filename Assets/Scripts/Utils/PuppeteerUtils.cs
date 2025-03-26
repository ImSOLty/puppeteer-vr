using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

public enum PuppeteerBone
{
    UNKNOWN = 0,
    Any = 1,
    LeftHand = 2,
    RightHand = 3,
    LeftFoot = 4,
    RightFoot = 5,
    LeftShoulder = 6,
    RightShoulder = 7,
    Waist = 8,
    Spine = 9,
    Chest = 10,
    Head = 11,
    Gamepad = 12,
    Camera = 13,
    Keyboard = 14,
    Treadmill = 15,
    LeftLowerLeg = 16,
    LeftUpperLeg = 17,
    RightLowerLeg = 18,
    RightUpperLeg = 19,
    LeftLowerArm = 20,
    LeftUpperArm = 21,
    RightLowerArm = 22,
    RightUpperArm = 23,
    LeftWrist = 24,
    RightWrist = 25,
    LeftAnkle = 26,
    RightAnkle = 27,
    Hips = 28,
    UpperChest = 29,
    Neck = 30,
    LeftToes = 31,
    RightToes = 32,
    LeftEye = 33,
    RightEye = 34,
    Jaw = 35,
    LeftThumbProximal = 36,
    LeftThumbIntermediate = 37,
    LeftThumbDistal = 38,
    LeftIndexProximal = 39,
    LeftIndexIntermediate = 40,
    LeftIndexDistal = 41,
    LeftMiddleProximal = 42,
    LeftMiddleIntermediate = 43,
    LeftMiddleDistal = 44,
    LeftRingProximal = 45,
    LeftRingIntermediate = 46,
    LeftRingDistal = 47,
    LeftLittleProximal = 48,
    LeftLittleIntermediate = 49,
    LeftLittleDistal = 50,
    RightThumbProximal = 51,
    RightThumbIntermediate = 52,
    RightThumbDistal = 53,
    RightIndexProximal = 54,
    RightIndexIntermediate = 55,
    RightIndexDistal = 56,
    RightMiddleProximal = 57,
    RightMiddleIntermediate = 58,
    RightMiddleDistal = 59,
    RightRingProximal = 60,
    RightRingIntermediate = 61,
    RightRingDistal = 62,
    RightLittleProximal = 63,
    RightLittleIntermediate = 64,
    RightLittleDistal = 65,
    LastBone = 66
}


namespace Puppeteer
{
    public class BonesMapping
    {
        private static readonly Dictionary<PuppeteerBone, SteamVR_Input_Sources> _steamVR_Input_Sources = new() {
            {PuppeteerBone.Any,SteamVR_Input_Sources.Any},
            {PuppeteerBone.LeftHand,SteamVR_Input_Sources.LeftHand},
            {PuppeteerBone.RightHand,SteamVR_Input_Sources.RightHand},
            {PuppeteerBone.LeftFoot,SteamVR_Input_Sources.LeftFoot},
            {PuppeteerBone.RightFoot,SteamVR_Input_Sources.RightFoot},
            {PuppeteerBone.LeftShoulder,SteamVR_Input_Sources.LeftShoulder},
            {PuppeteerBone.RightShoulder,SteamVR_Input_Sources.RightShoulder},
            {PuppeteerBone.Waist,SteamVR_Input_Sources.Waist},
            {PuppeteerBone.Chest,SteamVR_Input_Sources.Chest},
            {PuppeteerBone.Head,SteamVR_Input_Sources.Head},
            {PuppeteerBone.Gamepad,SteamVR_Input_Sources.Gamepad},
            {PuppeteerBone.Camera,SteamVR_Input_Sources.Camera},
            {PuppeteerBone.Keyboard,SteamVR_Input_Sources.Keyboard},
            {PuppeteerBone.Treadmill,SteamVR_Input_Sources.Treadmill},
            {PuppeteerBone.LeftLowerLeg,SteamVR_Input_Sources.LeftKnee},
            {PuppeteerBone.RightLowerLeg,SteamVR_Input_Sources.RightKnee},
            {PuppeteerBone.LeftLowerArm,SteamVR_Input_Sources.LeftElbow},
            {PuppeteerBone.RightLowerArm,SteamVR_Input_Sources.RightElbow},
            {PuppeteerBone.LeftWrist,SteamVR_Input_Sources.LeftWrist},
            {PuppeteerBone.RightWrist,SteamVR_Input_Sources.RightWrist},
            {PuppeteerBone.LeftAnkle,SteamVR_Input_Sources.LeftAnkle},
            {PuppeteerBone.RightAnkle,SteamVR_Input_Sources.RightAnkle},
        };
        private static readonly Dictionary<PuppeteerBone, HumanBodyBones> _humanBodyBone = new() {
            {PuppeteerBone.LeftHand,HumanBodyBones.LeftHand},
            {PuppeteerBone.RightHand,HumanBodyBones.RightHand},
            {PuppeteerBone.LeftFoot,HumanBodyBones.LeftFoot},
            {PuppeteerBone.RightFoot,HumanBodyBones.RightFoot},
            {PuppeteerBone.LeftShoulder,HumanBodyBones.LeftShoulder},
            {PuppeteerBone.RightShoulder,HumanBodyBones.RightShoulder},
            {PuppeteerBone.Waist,HumanBodyBones.Hips},
            {PuppeteerBone.Chest,HumanBodyBones.Chest},
            {PuppeteerBone.Spine,HumanBodyBones.Spine},
            {PuppeteerBone.Head,HumanBodyBones.Head},
            {PuppeteerBone.LeftLowerLeg,HumanBodyBones.LeftLowerLeg},
            {PuppeteerBone.RightLowerLeg,HumanBodyBones.RightLowerLeg},
            {PuppeteerBone.LeftLowerArm,HumanBodyBones.LeftLowerArm},
            {PuppeteerBone.RightLowerArm,HumanBodyBones.RightLowerArm},
            {PuppeteerBone.LeftUpperLeg,HumanBodyBones.LeftUpperLeg},
            {PuppeteerBone.RightUpperLeg,HumanBodyBones.RightUpperLeg},
            {PuppeteerBone.LeftUpperArm,HumanBodyBones.LeftUpperArm},
            {PuppeteerBone.RightUpperArm,HumanBodyBones.RightUpperArm},
            {PuppeteerBone.UpperChest, HumanBodyBones.UpperChest},
            {PuppeteerBone.Neck, HumanBodyBones.Neck},
            {PuppeteerBone.LeftToes, HumanBodyBones.LeftToes},
            {PuppeteerBone.RightToes, HumanBodyBones.RightToes},
            {PuppeteerBone.LeftEye, HumanBodyBones.LeftEye},
            {PuppeteerBone.RightEye, HumanBodyBones.RightEye},
            {PuppeteerBone.Jaw, HumanBodyBones.Jaw},
            // Fingers
            {PuppeteerBone.LeftThumbProximal,HumanBodyBones.LeftThumbProximal},
            {PuppeteerBone.LeftThumbIntermediate,HumanBodyBones.LeftThumbIntermediate},
            {PuppeteerBone.LeftThumbDistal,HumanBodyBones.LeftThumbDistal},
            {PuppeteerBone.LeftIndexProximal,HumanBodyBones.LeftIndexProximal},
            {PuppeteerBone.LeftIndexIntermediate,HumanBodyBones.LeftIndexIntermediate},
            {PuppeteerBone.LeftIndexDistal,HumanBodyBones.LeftIndexDistal},
            {PuppeteerBone.LeftMiddleProximal,HumanBodyBones.LeftMiddleProximal},
            {PuppeteerBone.LeftMiddleIntermediate,HumanBodyBones.LeftMiddleIntermediate},
            {PuppeteerBone.LeftMiddleDistal,HumanBodyBones.LeftMiddleDistal},
            {PuppeteerBone.LeftRingProximal,HumanBodyBones.LeftRingProximal},
            {PuppeteerBone.LeftRingIntermediate,HumanBodyBones.LeftRingIntermediate},
            {PuppeteerBone.LeftRingDistal,HumanBodyBones.LeftRingDistal},
            {PuppeteerBone.LeftLittleProximal,HumanBodyBones.LeftLittleProximal},
            {PuppeteerBone.LeftLittleIntermediate,HumanBodyBones.LeftLittleIntermediate},
            {PuppeteerBone.LeftLittleDistal,HumanBodyBones.LeftLittleDistal},
            {PuppeteerBone.RightThumbProximal,HumanBodyBones.RightThumbProximal},
            {PuppeteerBone.RightThumbIntermediate,HumanBodyBones.RightThumbIntermediate},
            {PuppeteerBone.RightThumbDistal,HumanBodyBones.RightThumbDistal},
            {PuppeteerBone.RightIndexProximal,HumanBodyBones.RightIndexProximal},
            {PuppeteerBone.RightIndexIntermediate,HumanBodyBones.RightIndexIntermediate},
            {PuppeteerBone.RightIndexDistal,HumanBodyBones.RightIndexDistal},
            {PuppeteerBone.RightMiddleProximal,HumanBodyBones.RightMiddleProximal},
            {PuppeteerBone.RightMiddleIntermediate,HumanBodyBones.RightMiddleIntermediate},
            {PuppeteerBone.RightMiddleDistal,HumanBodyBones.RightMiddleDistal},
            {PuppeteerBone.RightRingProximal,HumanBodyBones.RightRingProximal},
            {PuppeteerBone.RightRingIntermediate,HumanBodyBones.RightRingIntermediate},
            {PuppeteerBone.RightRingDistal,HumanBodyBones.RightRingDistal},
            {PuppeteerBone.RightLittleProximal,HumanBodyBones.RightLittleProximal},
            {PuppeteerBone.RightLittleIntermediate,HumanBodyBones.RightLittleIntermediate},
            {PuppeteerBone.RightLittleDistal,HumanBodyBones.RightLittleDistal},
            {PuppeteerBone.LastBone,HumanBodyBones.LastBone},
        };
        private static readonly Dictionary<SteamVR_Input_Sources, PuppeteerBone> _steamVR_Input_Sources_reverse =
            _steamVR_Input_Sources.ToDictionary(x => x.Value, x => x.Key);
        private static readonly Dictionary<HumanBodyBones, PuppeteerBone> _humanBodyBone_reverse =
            _humanBodyBone.ToDictionary(x => x.Value, x => x.Key);
        public static SteamVR_Input_Sources SteamVR(PuppeteerBone bone)
        {
            return _steamVR_Input_Sources[bone];
        }
        public static PuppeteerBone FromSteamVR(SteamVR_Input_Sources bone)
        {
            return _steamVR_Input_Sources_reverse[bone];
        }
        public static HumanBodyBones HumanBodyBone(PuppeteerBone bone)
        {
            return _humanBodyBone[bone];
        }
        public static PuppeteerBone FromHumanBodyBone(HumanBodyBones bone)
        {
            return _humanBodyBone_reverse[bone];
        }
    };
}