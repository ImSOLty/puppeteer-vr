public enum Mode
{
    NONE, ANIMATION_RUNTIME, ANIMATION_RECORDING, PROPS_MANAGEMENT, PROPS_MANAGEMENT_EDIT
}

namespace Settings
{
    public static class Animation
    {
        public static int FPS = 50;
        public static int TotalTimeInSeconds = 10;
        public static int TotalFrames() { return FPS * TotalTimeInSeconds; }
        public static SceneProperties ScenePropertiesData = null;
        public static Mode AnimationMode = Mode.NONE;
    }
}
