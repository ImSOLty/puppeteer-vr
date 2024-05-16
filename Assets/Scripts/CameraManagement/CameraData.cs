using UnityEngine;

public class CameraConstants
{
    public const int TextureDepth = 16;
    public const RenderTextureFormat TextureFormat = RenderTextureFormat.ARGB32;

    public const float ScreenResize = 2f;
    public const int DefaultWidth = 1920, DefaultHeight = 1080;
    public const float DefaultFOV = 60.0f;
    public const float DefaultNear = 0.3f, DefaultFar = 100.0f;
}

public class CameraData
{
    public CameraData(string name = "Unnamed")
    {
        Name = name;
        Width = CameraConstants.DefaultWidth;
        Height = CameraConstants.DefaultHeight;
        FOV = CameraConstants.DefaultFOV;
        Near = CameraConstants.DefaultNear;
        Far = CameraConstants.DefaultFar;
        Closed = false;
        CameraColor = Random.ColorHSV();
    }

    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public float FOV { get; set; }
    public float Near { get; set; }
    public float Far { get; set; }
    public bool Closed { get; set; }
    public Color CameraColor { get; set; }


    public override string ToString() => $"Width: {Width}, Height: {Height}, " +
                                         $"Closed: {Closed}";
}