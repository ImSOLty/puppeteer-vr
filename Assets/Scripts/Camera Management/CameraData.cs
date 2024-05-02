public class CameraData
{
    public CameraData(int width, int height)
    {
        Width = width;
        Height = height;
        Selected = false;
        Closed = false;
    }

    public int Width { get; set; }
    public int Height { get; set; }
    public bool Selected { get; set; }
    public bool Closed { get; set; }


    public override string ToString() => $"Width: {Width}, Height: {Height}, " +
                                         $"Selected: {Selected}, Closed: {Closed}";
}