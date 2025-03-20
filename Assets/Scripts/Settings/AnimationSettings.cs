public enum Mode
{
    NONE, ANIMATION_RUNTIME, ANIMATION_RECORDING, PROPS_MANAGEMENT
}

namespace Settings
{
    public static class Animation
    {
        public static int FPS = 50;
        public static int TotalTimeInSeconds = 20;
        public static int TotalFrames() { return FPS * TotalTimeInSeconds; }
        public static SceneProperties ScenePropertiesData = null;
        public static Mode AnimationMode = Mode.NONE;
    }
}
