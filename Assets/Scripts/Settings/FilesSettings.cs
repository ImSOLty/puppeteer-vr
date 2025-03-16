using System.IO;
using UnityEngine;

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
    public void Write(string jsonData) { File.WriteAllText(FullPath(), jsonData); }
}

namespace Settings
{
    public static class Files
    {
        public static string streamingAssetsPath = Application.streamingAssetsPath;
        public static string appAssetsFolderName = "Assets";
        public static string AssetsFolderPath = Path.Combine(streamingAssetsPath, appAssetsFolderName);
        public static AppFile AssetsConfiguration = new("AssetsConfiguration.json", AssetsFolderPath);
        public static AppFile CalibrationSettings = new("CalibrationSettings.json", streamingAssetsPath);
    }
}
