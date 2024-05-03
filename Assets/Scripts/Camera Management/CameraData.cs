public class CameraData
{
    public CameraData(int width, int height)
    {
        Width = width;
        Height = height;
        Closed = false;
    }

    public int Width { get; set; }
    public int Height { get; set; }
    public bool Closed { get; set; }


    public override string ToString() => $"Width: {Width}, Height: {Height}, " +
                                         $"Closed: {Closed}";
}