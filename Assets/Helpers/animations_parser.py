import json
import os

directory = os.path.join(os.path.dirname(__file__), "_AnimationsData", "raw")

input_values = []

for name in os.listdir(directory):
    with open(os.path.join(directory, name)) as f:
        print(f"Content of '{name}'")
        data = json.loads(f.read())["keyFrameDatas"]
        for frame in data:
            transform = frame["rigTransform"]
            for body_part, info in transform.items():
                position = info["position"]
                rotation = info["rotation"]
