import json
import os

KEY_FRAME_DATAS = "keyFrameDatas"
INPUT_BODY_TRANSFORMS = "inputTransformsAsFloatArray"
OUTPUT_BODY_TRANSFORMS = "outputTransformsAsFloatArray"


def get_positions_rotations_from_file(file_name):
    result_dict = {}
    with open(os.path.join(os.path.dirname(__file__), file_name)) as f:
        data = json.loads(f.read())[KEY_FRAME_DATAS]
        for frame in data:
            result_dict[frame["time"]] = {
                "positions": frame["positions"],
                "rotations": frame["rotations"],
            }
    return result_dict


reference, ik_approach, ai_full_body, ai_feet_only = (
    get_positions_rotations_from_file(file_name)
    for file_name in ["1.json", "2.json", "3.json", "4.json"]
)
for approach in [ik_approach, ai_full_body, ai_feet_only]:
    difference_positions, difference_rotations = 0, 0
    for time, data in reference.items():
        difference_positions += sum(
            abs(a - b) for a, b in zip(data["positions"], approach[time]["positions"])
        )
        difference_rotations += sum(
            abs(a - b) for a, b in zip(data["rotations"], approach[time]["rotations"])
        )
    print(difference_positions)
    print(difference_rotations)
