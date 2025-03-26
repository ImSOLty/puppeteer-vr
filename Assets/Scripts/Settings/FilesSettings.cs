using System;
using System.IO;
using UnityEngine;

public enum ResultType
{
    IMAGE, VIDEO
}

public class AppFile
{
    public string fileName;
    public string folderPath;

    public AppFile(string fileName, string folderPath)
    {
        this.fileName = fileName;
        this.folderPath = folderPath;
    }


    public bool Exists() { return File.Exists(FullPath()); }
    public string FullPath() { return Path.Combine(folderPath, fileName); }
    public string Read() { return File.ReadAllText(FullPath()); }
    public void Write(string jsonData)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        File.WriteAllText(FullPath(), jsonData);
    }
}

namespace Settings
{
    public static class Files
    {
        public static string streamingAssetsPath = Application.streamingAssetsPath;
        public static string assetsFolderName = "Assets";
        public static string scenesFolderName = "Scenes";
        public static string resultsFolderName = "Results";
        private static string resultsImagesSubFolderName = "Images";
        private static string resultsVideosSubFolderName = "Videos";
        public static string dateFormat = "yyyyMMddTHHmmss";

        public static string GetResultFolder(ResultType resultType)
        {
            string subFolder = resultType == ResultType.IMAGE ? resultsImagesSubFolderName : resultsVideosSubFolderName;
            string folderPath = Path.Combine(streamingAssetsPath, resultsFolderName, subFolder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return folderPath;
        }
        public static string AssetsFolderPath = Path.Combine(streamingAssetsPath, assetsFolderName);
        public static string ScenesFolderPath = Path.Combine(streamingAssetsPath, scenesFolderName);
        public static AppFile AssetsConfiguration = new("AssetsConfiguration.json", AssetsFolderPath);
        public static AppFile BodyCalibrationSettings = new("BodyCalibrationSettings.json", streamingAssetsPath);
        public static AppFile HandCalibrationSettings = new("HandCalibrationSettings.json", streamingAssetsPath);
        public static AppFile ScenesPropertiesData = new("ScenesProperties.json", AssetsFolderPath);
    }
}
