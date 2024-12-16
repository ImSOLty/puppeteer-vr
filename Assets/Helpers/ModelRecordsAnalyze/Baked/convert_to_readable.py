import json
import os
from collections import defaultdict
from enum import Enum

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


def convert_files_to_readable(approach_file_names: list[tuple[Approach, str]]):
    result = dict()
    for approach, file_name in approach_file_names:
        with open(os.path.join(os.path.dirname(__file__), file_name), "r") as file:
            data = json.load(file)
        for frame_data in data[KEY_FRAME_DATAS]:
            result[frame_data["time"]] = Frame(
                content_positions=frame_data["positions"],
                content_rotations=frame_data["rotations"],
            )

        with open(
            os.path.join(os.path.dirname(__file__), approach.value + ".json"), "w"
        ) as f:
            f.write(json.dumps(result, cls=ComplexEncoder))


convert_files_to_readable(
    [
        (Approach.REFERENCE, "1.json"),
        (Approach.INVERSE_KINEMATICS, "2.json"),
        (Approach.AI_FULL_BODY, "3.json"),
        (Approach.AI_FEET_WITH_IK, "4.json"),
    ]
)

# def get_positions_rotations_from_file(file_name):
#     result_dict = {}
#     with open(os.path.join(os.path.dirname(__file__), file_name)) as f:
#         data = json.loads(f.read())[KEY_FRAME_DATAS]
#         for frame in data:
#             result_dict[frame["time"]] = {
#                 "positions": frame["positions"],
#                 "rotations": frame["rotations"],
#             }
#     return result_dict


# reference, ik_approach, ai_full_body, ai_feet_only = (
#     get_positions_rotations_from_file(file_name)
#     for file_name in ["1.json", "2.json", "3.json", "4.json"]
# )
# for approach in [ik_approach, ai_full_body, ai_feet_only]:
#     difference_positions, difference_rotations = 0, 0
#     for time, data in reference.items():
#         difference_positions += sum(
#             abs(a - b) for a, b in zip(data["positions"], approach[time]["positions"])
#         )
#         difference_rotations += sum(
#             abs(a - b) for a, b in zip(data["rotations"], approach[time]["rotations"])
#         )
#     print(difference_positions)
#     print(difference_rotations)
