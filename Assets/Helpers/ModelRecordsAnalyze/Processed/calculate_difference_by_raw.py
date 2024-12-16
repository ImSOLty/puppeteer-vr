import json
import os
from enum import Enum
import numpy as np
from scipy.spatial.transform import Rotation as R

KEY_FRAME_DATAS = "keyFrameDatas"


class Approach(Enum):
    REFERENCE = "Reference"
    INVERSE_KINEMATICS = "Inverse Kinematics"
    AI_FULL_BODY = "AI Full Body"
    AI_FEET_WITH_IK = "AI (feet positioning) with IK"


class Bone(Enum):
    # THE ORDER IS NECESSARY
    Hips = "Hips"
    LeftUpLeg = "LeftUpLeg"
    LeftLeg = "LeftLeg"
    LeftFoot = "LeftFoot"
    RightUpLeg = "RightUpLeg"
    RightLeg = "RightLeg"
    RightFoot = "RightFoot"
    Spine = "Spine"
    Spine1 = "Spine1"
    Spine2 = "Spine2"
    LeftShoulder = "LeftShoulder"
    LeftArm = "LeftArm"
    LeftForeArm = "LeftForeArm"
    LeftHand = "LeftHand"
    Neck = "Neck"
    Head = "Head"
    RightShoulder = "RightShoulder"
    RightArm = "RightArm"
    RightForeArm = "RightForeArm"
    RightHand = "RightHand"


class Vector3:
    x: float
    y: float
    z: float

    def __init__(self, x, y, z):
        self.x = x
        self.y = y
        self.z = z

    @classmethod
    def from_list(cls, data: list[float]):
        return cls(*data)

    def reprJSON(self):
        return dict(x=self.x, y=self.y, z=self.z)

    def as_array(self):
        return [self.x, self.y, self.z]


class BoneTransform:
    position: Vector3
    rotation: Vector3

    def __init__(self, position_array: list[float], rotation_array: list[float]):
        self.position = Vector3.from_list(position_array)
        self.rotation = Vector3.from_list(rotation_array)

    def reprJSON(self):
        return dict(position=self.position, rotation=self.rotation)


class Frame:
    transforms: dict[Bone, BoneTransform]

    def __init__(
        self,
        content_positions: list[float],
        content_rotations: list[float],
    ):
        assert len(Bone) == len(content_positions) // 3
        assert len(Bone) == len(content_rotations) // 3
        self.transforms = {
            bone_type.value: BoneTransform(
                position_array=content_positions[i * 3 : (i + 1) * 3],
                rotation_array=content_rotations[i * 3 : (i + 1) * 3],
            )
            for i, bone_type in enumerate(Bone)
        }

    def reprJSON(self):
        return self.transforms


class ComplexEncoder(json.JSONEncoder):
    def default(self, obj):
        if hasattr(obj, "reprJSON"):
            return obj.reprJSON()
        else:
            return json.JSONEncoder.default(self, obj)


def convert_files_to_readable(file_name: str):
    result = dict()
    with open(os.path.join(os.path.dirname(__file__), file_name), "r") as file:
        data = json.load(file)
    for frame_data in data[KEY_FRAME_DATAS]:
        result[frame_data["time"]] = Frame(
            content_positions=frame_data["positions"],
            content_rotations=frame_data["rotations"],
        )
    return result


converted = {
    Approach.REFERENCE: convert_files_to_readable("1.json"),
    Approach.INVERSE_KINEMATICS: convert_files_to_readable("2.json"),
    Approach.AI_FULL_BODY: convert_files_to_readable("3.json"),
    Approach.AI_FEET_WITH_IK: convert_files_to_readable("4.json"),
}


def euler_difference(euler1: list[float], euler2: list[float]):
    # Преобразуем углы Эйлера (в градусах) в кватернионы
    q1 = R.from_euler("xyz", euler1, degrees=True)
    q2 = R.from_euler("xyz", euler2, degrees=True)

    # Вычисляем разницу между кватернионами
    q_diff = q2 * q1.inv()  # Умножаем q2 на обратный q1

    # Получаем угол поворота из кватерниона
    angle = q_diff.magnitude()  # Угол в радианах
    angle_degrees = np.degrees(angle)  # Переводим в градусы

    return angle_degrees


def position_difference(vector1: list[float], vector2: list[float]):
    return sum(abs(a - b) for a, b in zip(vector1, vector2))


def calculate_difference_between_bones(
    bone1: BoneTransform, bone2: BoneTransform
) -> tuple[float, float]:  # difference in position and rotation respectively
    return position_difference(
        bone1.position.as_array(), bone2.position.as_array()
    ), euler_difference(bone1.rotation.as_array(), bone2.rotation.as_array())


def calculate_difference_between_data(data1: dict, data2: dict) -> tuple[float, float]:
    total_position_difference, total_rotation_difference = 0, 0
    for time in data1.keys():
        frame_data1, frame_data2 = data1[time], data2[time]
        for bone in frame_data1.transforms.keys():
            bone_data1 = frame_data1.transforms[bone]
            bone_data2 = frame_data2.transforms[bone]
            position_difference, rotation_difference = (
                calculate_difference_between_bones(bone_data1, bone_data2)
            )
            total_position_difference += position_difference
            total_rotation_difference += rotation_difference
    return [
        total_position_difference / len(data1),
        total_rotation_difference / len(data1),
    ]


print(
    calculate_difference_between_data(
        converted[Approach.REFERENCE], converted[Approach.INVERSE_KINEMATICS]
    )
)
print(
    calculate_difference_between_data(
        converted[Approach.REFERENCE], converted[Approach.AI_FULL_BODY]
    )
)
print(
    calculate_difference_between_data(
        converted[Approach.REFERENCE], converted[Approach.AI_FEET_WITH_IK]
    )
)
