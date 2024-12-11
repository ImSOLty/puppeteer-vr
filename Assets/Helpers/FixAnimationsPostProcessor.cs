// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;

// class FixAnimationsPostProcessor : AssetPostprocessor
// {

//     ModelImporter avatarImporter;
//     Avatar GetSourceAvatar()
//     {
//         //get avatar to apply to animations being imported
//         avatarImporter = assetImporter as ModelImporter;
//         Avatar avatarObj = (Avatar)AssetDatabase.LoadAssetAtPath("Assets/Models/Y Bot.fbx", typeof(Avatar));
//         return avatarObj;
//     }

//     void OnPostprocessAnimation()
//     {
//         // string[] assetPaths = AssetDatabase.GetAllAssetPaths();
//         // foreach (string assetPath in assetPaths)
//         // {
//         //     System.Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

//         //     if (assetType != null)
//         //     {
//         //         if (assetPath.Contains("Helpers/Animations"))
//         //         {
//         //             if (!assetPath.Contains(".prefab") && assetType.ToString().Contains("GameObject")) // Filter out prefabs and leave only models
//         //             {
//         //                 ModelImporter importer = ModelImporter.GetAtPath(assetPath) as ModelImporter;

//         //                 if (importer.importLights ||
//         //                     importer.importCameras)
//         //                 {
//         //                     importer.anim
//         //                 }
//         //             }
//         //         }
//         //     }
//         // }

//         Avatar sourceAvatar = GetSourceAvatar();
//         var modelImporter = assetImporter as ModelImporter;
//         modelImporter.sourceAvatar = sourceAvatar;
//         modelImporter.animationType = ModelImporterAnimationType.Human;
//         Debug.Log(modelImporter.clipAnimations);
//         // modelImporter.animationType = ModelImporterAnimationType.Human;
//         // modelImporter.motionNodeName = "HumanMaleSkeleton/root/root_motion";
//         // modelImporter.optimizeBones = false;

//     }

 
// }
// public class FixAnimations : MonoBehaviour
// {
//     // Add a menu item named "Do Something" to MyMenu in the menu bar.
//     [MenuItem("HelperMenu/Fix Imported Animations")]
//     static void FixImportedAnimations()
//     {
        
//     }
// }
