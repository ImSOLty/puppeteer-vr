public class CameraSectionDivider
{
    private float _position;
    private CameraSection _leftCameraSection, _rightCameraSection;
    private CameraLineDivider _lineDivider;

    public CameraSectionDivider(CameraSection left, CameraSection right, float position)
    {
        _position = position;
        _leftCameraSection = left;
        _rightCameraSection = right;
    }

    public float GetPosition()
    {
        return _position;
    }

    public void SetPosition(float position)
    {
        _position = position;
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