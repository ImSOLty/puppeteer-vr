using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

public class AIRigSolver : MonoBehaviour
{
    public NNModel modelAsset; // Assign your ONNX model here in the Inspector
    private IWorker worker;
    private Tensor inputTensor;
    private Tensor outputTensor;

    void Start()
    {
        // Load the model
        Model model = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);

        // Initialize input tensor with shape (1, 1, 1, 18) for 18 floats


        // Example: Fill input tensor with data
        float[] inputData = new float[18]; // Populate this array with your actual data
        for (int i = 0; i < inputData.Length; i++)
        {
            inputData[i] = 0;
        }

        inputTensor = new Tensor(1, 1, 1, 18, inputData);

        // Execute the model
        worker.Execute(inputTensor);

        // Fetch output tensor which should have shape (1, 1, 1, 96)
        outputTensor = worker.PeekOutput();

        // Process output data
        float[] outputData = outputTensor.ToReadOnlyArray();
        foreach (float x in outputData){
            Debug.Log(x);
        }

        // Clean up
        inputTensor.Dispose();
        outputTensor.Dispose();
    }

    void OnDestroy()
    {
        worker.Dispose();
    }
}
