using UnityEngine;
using UniGLTF;


public class ImportManager : MonoBehaviour
{
    private Shader GLTFUniUnlitShader;
    // Start is called before the first frame update
    void Awake()
    {
        GLTFUniUnlitShader = Shader.Find(UniUnlitUtils.ShaderName);
    }

    public RuntimeGltfInstance LoadGLTFByPathName(string pathName)
    {
        using (var data = new GlbFileParser(pathName).Parse())
        using (var context = new ImporterContext(data: data))
        {
            var loaded = context.Load();
            loaded.EnableUpdateWhenOffscreen();

            // Converts to white materials, so comment out
            // ConvertGLTFInstanceToURP(loaded);

            return loaded;
        }
    }

    public RuntimeGltfInstance LoadVRMByPathName(string pathName)
    {
        using (var data = new GlbFileParser(pathName).Parse())
        using (var context = new VRM.VRMImporterContext(new VRM.VRMData(data)))
        {
            var loaded = context.Load();
            loaded.EnableUpdateWhenOffscreen();

            ConvertGLTFInstanceToURP(loaded);
            return loaded;
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
