#if ( UNITY_EDITOR )

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FixAnimations : MonoBehaviour
{
    [SerializeField] public Avatar avatar;

    // Add a menu item named "Do Something" to MyMenu in the menu bar.
    [MenuItem("HelperMenu/Fix Imported Animations")]
    static void FixImportedAnimations()
    {
        string[] assetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in assetPaths)
        {
            System.Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

            if (assetType != null)
            {
                if (assetPath.Contains("Helpers/Animations"))
                {
                    if (!assetPath.Contains(".prefab") && assetType.ToString().Contains("GameObject")) // Filter out prefabs and leave only models
                    {
                        ModelImporter importer = ModelImporter.GetAtPath(assetPath) as ModelImporter;

                        if (importer.importLights ||
                            importer.importCameras)
                        {
                            importer.animationType = ModelImporterAnimationType.Human;
                            // importer.sourceAvatar = avatar;
                            // Debug.Log(avatar);
                        }
                    }
                }
            }
        }
    }
}

#endif