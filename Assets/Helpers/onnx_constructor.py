import numpy as np
import torch
import torch.nn as nn
import torch.optim as optim
from sklearn.model_selection import train_test_split
import torch.onnx


# Step 1: Data Preparation
# Generate synthetic data for demonstration purposes
def generate_synthetic_data(num_samples, num_bones):
    # Randomly generate input parameters (3 bones - 6 values per bone)
    inputs = np.random.rand(num_samples, 3 * 6).astype(np.float32)
    # Randomly generate positions and rotations for each bone (6 values per bone)
    outputs = np.random.rand(num_samples, num_bones * 6).astype(np.float32)
    return inputs, outputs


# Parameters
num_samples = 500000  # Total number of samples
num_bones = 62  # Number of bones in the rig

# Generate data
inputs, outputs = generate_synthetic_data(num_samples, num_bones)
# print(len(inputs[0]))
# print(len(outputs[0]))

# Split data into training and testing sets
X_train, X_test, y_train, y_test = train_test_split(
    inputs, outputs, test_size=0.2, random_state=42
)


# Step 2: Model Definition
class BonePositionRotationModel(nn.Module):
    def __init__(self, num_bones):
        super(BonePositionRotationModel, self).__init__()
        self.fc1 = nn.Linear(
            3 * 6, 64
        )  # Input layer (3 pos + 3 rot per bone - 3 bones)
        self.fc2 = nn.Linear(64, 128)  # Hidden layer
        self.fc3 = nn.Linear(
            128, num_bones * 6
        )  # Output layer (3 pos + 3 rot per bone)

    def forward(self, x):
        x = torch.relu(self.fc1(x))
        x = torch.relu(self.fc2(x))
        x = self.fc3(x)
        return x


# Step 3: Training the Model
model = BonePositionRotationModel(num_bones)
optimizer = optim.Adam(model.parameters(), lr=0.001)
criterion = nn.MSELoss()

# Convert training data to PyTorch tensors
X_train_tensor = torch.tensor(X_train)
y_train_tensor = torch.tensor(y_train)

num_epochs = 100  # Number of training epochs

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
dummy_input = torch.randn(1, 3 * 6)  # Example input shape for ONNX export
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
