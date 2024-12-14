using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;

public class AIRigSolver : MonoBehaviour
{
    private RigResolver rigResolver;
    public NNModel modelAsset;
    private IWorker worker;
    private Tensor inputTensor;
    private Tensor outputTensor;

    void Awake()
    {
        Model model = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
        rigResolver = GetComponent<RigResolver>();
    }
    void LateUpdate()
    {
        // Fill tensor with input data (transforms of input bones)
        float[] inputData = rigResolver.rigTransform.GetInputReferenceBonesAsNormalizedArray(withRotation: false);

        inputTensor = new Tensor(1, 1, 1, inputData.Length, inputData);

        // Execute the model
        worker.Execute(inputTensor);

        // Fetch output tensor
        outputTensor = worker.PeekOutput();

        // Process output data
        float[] outputData = outputTensor.ToReadOnlyArray();
        rigResolver.rigTransform.SetOutputBonesFromNormalizedArray(outputData.ToArray());
        rigResolver.rigTransform.SetInputBonesAsReference(); // Should be set in order to follow XR Rig


        // Clean up
        inputTensor.Dispose();
        outputTensor.Dispose();
    }

    void OnDestroy()
    {
        worker.Dispose();
    }
}
