public class AnimationSettings
{
    public static float FPS = 60.0f;
    public static float Duration = 10.0f;
    public static string Path = "C:\\VKR\\Materials\\Recordings";

    public static string SettingsInfo()
    {
        return $"FPS={FPS}, Duration={Duration}, Path={Path}";
    }
}
