import numpy as np
import torch
import torch.nn as nn
import torch.optim as optim
from sklearn.model_selection import train_test_split
import torch.onnx

import json
import os

directory_parsed_input = os.path.join(
    os.path.dirname(__file__), "_AnimationsData", "parsed", "input.json"
)
directory_parsed_output = os.path.join(
    os.path.dirname(__file__), "_AnimationsData", "parsed", "output.json"
)

INPUT_BONES = 3
FLOATS_PER_INPUT_BONE = 3
OUTPUT_BONES = 17  # Number of bones in the rig without input ones
FLOATS_PER_OUTPUT_BONE = 6


# Step 1: Data Preparation
# Generate synthetic data for demonstration purposes
def generate_synthetic_data(num_samples, num_bones):
    # Randomly generate input parameters (3 bones - FLOATS_PER_INPUT_BONE values per bone)
    inputs = np.random.rand(num_samples, INPUT_BONES * FLOATS_PER_INPUT_BONE).astype(
        np.float32
    )
    # Randomly generate positions and rotations for each bone (6 values per bone)
    outputs = np.random.rand(num_samples, OUTPUT_BONES * FLOATS_PER_OUTPUT_BONE).astype(
        np.float32
    )
    return inputs, outputs


# # Generate data
# inputs, outputs = generate_synthetic_data(num_samples, num_bones)
with open(directory_parsed_input, "r") as input_file:
    inputs = json.load(input_file)
with open(directory_parsed_output, "r") as output_file:
    outputs = json.load(output_file)

# Split data into training and testing sets
X_train, X_test, y_train, y_test = train_test_split(
    inputs, outputs, test_size=0.1, random_state=42
)


# Step 2: Model Definition
class BonePositionRotationModel(nn.Module):
    def __init__(self, num_bones):
        super(BonePositionRotationModel, self).__init__()
        self.fc1 = nn.Linear(
            INPUT_BONES * FLOATS_PER_INPUT_BONE, 64
        )  # Input layer (3 pos + 3 rot per bone - 3 bones)
        self.fc2 = nn.Linear(64, 128)  # Hidden layer
        self.fc3 = nn.Linear(
            128, OUTPUT_BONES * FLOATS_PER_OUTPUT_BONE
        )  # Output layer (3 pos + 3 rot per bone)

    def forward(self, x):
        x = torch.relu(self.fc1(x))
        x = torch.relu(self.fc2(x))
        x = self.fc3(x)
        return x


# Step 3: Training the Model
model = BonePositionRotationModel(OUTPUT_BONES)
optimizer = optim.Adam(model.parameters(), lr=0.001)
criterion = nn.MSELoss()

# Convert training data to PyTorch tensors
X_train_tensor = torch.tensor(X_train)
y_train_tensor = torch.tensor(y_train)

num_epochs = 1000  # Number of training epochs

for epoch in range(num_epochs):
    model.train()  # Set the model to training mode
    optimizer.zero_grad()  # Clear gradients
    outputs = model(X_train_tensor)  # Forward pass
    loss = criterion(outputs, y_train_tensor)  # Compute loss
    loss.backward()  # Backward pass (compute gradients)
    optimizer.step()  # Update weights

    if (epoch + 1) % 10 == 0:
        print(f"Epoch [{epoch + 1}/{num_epochs}], Loss: {loss.item():.4f}")

# Step 4: Exporting the Model to ONNX Format
dummy_input = torch.randn(
    1, INPUT_BONES * FLOATS_PER_INPUT_BONE
)  # Example input shape for ONNX export
torch.onnx.export(
    model,  # model being run
    dummy_input,  # model input (or a tuple for multiple inputs)
    "bone_model.onnx",  # where to save the model (can be a file or file-like object)
    export_params=True,  # store the trained parameter weights inside the model file
    opset_version=9,  # the ONNX version to export the model to
    do_constant_folding=True,  # whether to execute constant folding for optimization
    input_names=["X"],  # the model's input names
    output_names=["Y"],  # the model's output names
)


print("Model exported to bone_model.onnx")
