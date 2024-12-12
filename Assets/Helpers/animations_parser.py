import json
import os

directory_parsed = os.path.join(os.path.dirname(__file__), "_AnimationsData", "parsed")
directory_raw = os.path.join(os.path.dirname(__file__), "_AnimationsData", "raw")

KEY_FRAME_DATAS = "keyFrameDatas"
INPUT_BODY_TRANSFORMS = "inputTransformsAsFloatArray"
OUTPUT_BODY_TRANSFORMS = "outputTransformsAsFloatArray"

result_input = []
result_output = []

for name in os.listdir(directory_raw):
    # skip .meta-files
    if name.endswith(".meta"):
        continue

    with open(os.path.join(directory_raw, name)) as f:
        print(f"Parsing '{name}'")
        data = json.loads(f.read())[KEY_FRAME_DATAS]
        for frame in data:
            result_input.append(frame[INPUT_BODY_TRANSFORMS])
            result_output.append(frame[OUTPUT_BODY_TRANSFORMS])

with open(os.path.join(directory_parsed, "input.json"), "w") as f:
    f.write(json.dumps(result_input))
with open(os.path.join(directory_parsed, "output.json"), "w") as f:
    f.write(json.dumps(result_output))
