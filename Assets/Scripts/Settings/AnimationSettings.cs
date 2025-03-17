public enum AnimationMode
{
    NONE, RUNTIME, RECORDING
}

namespace Settings
{
    public static class Animation
    {
        public static int FPS = 50;
        public static int TotalTimeInSeconds = 20;
        public static int TotalFrames() { return FPS * TotalTimeInSeconds; }
        public static SceneProperties ScenePropertiesData = null;
        public static AnimationMode AnimationMode = AnimationMode.NONE;
    }
}
