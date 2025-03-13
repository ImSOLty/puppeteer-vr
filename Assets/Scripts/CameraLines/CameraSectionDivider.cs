public class CameraSectionDivider
{
    private int _positionFrame;
    private CameraSection _leftCameraSection, _rightCameraSection;
    private CameraLineDivider _lineDivider;

    public CameraSectionDivider(CameraSection left, CameraSection right, int positionFrame)
    {
        _positionFrame = positionFrame;
        _leftCameraSection = left;
        _rightCameraSection = right;
    }

    public int GetPosition()
    {
        return _positionFrame;
    }

    public void SetPosition(int positionFrame)
    {
        _positionFrame = positionFrame;
    }

    public CameraSection GetLeftCameraSection()
    {
        return _leftCameraSection;
    }

    public CameraSection GetRightCameraSection()
    {
        return _rightCameraSection;
    }

    public void SetLeftCameraSection(CameraSection section)
    {

        _leftCameraSection = section;
    }

    public void SetRightCameraSection(CameraSection section)
    {
        _rightCameraSection = section;
    }

    public void SetLineDivider(CameraLineDivider divider)
    {
        _lineDivider = divider;
    }

    public CameraLineDivider GetLineDivider()
    {
        return _lineDivider;
    }

    public override string ToString()
    {
        return $"{_leftCameraSection} {_rightCameraSection}";
    }
}