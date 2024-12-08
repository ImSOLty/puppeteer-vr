import json
import os

directory_parsed = os.path.join(os.path.dirname(__file__), "_AnimationsData", "parsed")
directory_raw = os.path.join(os.path.dirname(__file__), "_AnimationsData", "raw")


input_body_parts = {"LeftHand", "RightHand", "Head"}

result_input = []
result_output = []

for name in os.listdir(directory_raw):
    with open(os.path.join(directory_raw, name)) as f:
        print(f"Parsing '{name}'")
        data = json.loads(f.read())["keyFrameDatas"]
        for frame in data:
            transform = frame["rigTransform"]
            input_values = []
            output_values = []
            for body_part, info in transform.items():
                position = info["position"]
                rotation = info["rotation"]
                value = [
                    position["x"],
                    position["y"],
                    position["z"],
                    rotation["x"],
                    rotation["y"],
                    rotation["z"],
                ]
                if body_part in input_body_parts:
                    input_values.append(value)
                else:
                    output_values.append(value)
            result_input.extend(input_values)
            result_output.extend(output_values)
with open(os.path.join(directory_parsed, "input.json"), "w") as f:
    f.write(json.dumps(result_input))
with open(os.path.join(directory_parsed, "output.json"), "w") as f:
    f.write(json.dumps(result_output))
