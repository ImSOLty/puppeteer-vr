public class CameraSection
{
    public CameraSection(CameraInstance instance, float start, float end)
    {
        CamInstance = instance;
        Start = start;
        End = end;
    }

    public CameraInstance CamInstance { get; set; }
    public float Start { get; set; }
    public float End { get; set; }


    public override string ToString() => $"{CamInstance}, [{Start};{End}]";
}