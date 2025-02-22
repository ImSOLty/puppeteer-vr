using UnityEngine;
using UniGLTF;


public class ImportManager : MonoBehaviour
{
    private Shader GLTFUniUnlitShader;
    [SerializeField] private string defaultPath;
    // Start is called before the first frame update
    void Start()
    {
        GLTFUniUnlitShader = Shader.Find(UniUnlitUtils.ShaderName);
        using (var data = new GlbFileParser(defaultPath).Parse())
        using (var context = new VRM.VRMImporterContext(new VRM.VRMData(data)))
        {
            var loaded = context.Load();
            loaded.EnableUpdateWhenOffscreen();

            ConvertGLTFInstanceToURP(loaded);

            loaded.ShowMeshes();
        }
    }

    private void ConvertGLTFInstanceToURP(RuntimeGltfInstance instance)
    {
        foreach (var material in instance.Materials)
        {
            ConvertMaterialToURP(material);
        }
    }

    private void ConvertMaterialToURP(Material material)
    {
        // By default shader is MToon, converting it to GLTF URP
        material.shader = GLTFUniUnlitShader;
        // Setting up material blendmode to cutout
        UniUnlitUtils.SetRenderMode(material, UniUnlitRenderMode.Cutout);
        // Validating properties by setting material blend values and managing keywords
        UniUnlitUtils.ValidateProperties(material);
    }
}
