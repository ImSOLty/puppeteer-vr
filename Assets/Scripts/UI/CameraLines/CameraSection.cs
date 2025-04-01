public class CameraSection
{
    private CameraSectionDivider _rightSectionDivider = null, _leftSectionDivider = null;
    private CameraInstance _cameraInstance;
    private CameraLine _line;

    public CameraSection(CameraInstance instance)
    {
        _cameraInstance = instance;
    }

    public CameraSectionDivider GetRightSectionDivider()
    {
        return _rightSectionDivider;
    }

    public CameraSectionDivider GetLeftSectionDivider()
    {
        return _leftSectionDivider;
    }

    public void SetRightSectionDivider(CameraSectionDivider divider)
    {
        _rightSectionDivider = divider;
    }

    public void SetLeftSectionDivider(CameraSectionDivider divider)
    {
        _leftSectionDivider = divider;
    }

    public CameraInstance GetCameraInstance()
    {
        return _cameraInstance;
    }

    public void SetCameraInstance(CameraInstance instance)
    {
        _cameraInstance = instance;
    }

    public (CameraSection, CameraSectionDivider) SplitSectionAndReturn(int framePosition)
    {
        CameraSection newSection = new CameraSection(_cameraInstance);
        CameraSectionDivider divider = new CameraSectionDivider(this, newSection, framePosition);
        newSection._leftSectionDivider = divider;
        newSection._rightSectionDivider = _rightSectionDivider;
        _rightSectionDivider = divider;
        return (newSection, divider);
    }

    public void SetLine(CameraLine line)
    {
        _line = line;
    }

    public CameraLine GetLine()
    {
        return _line;
    }

    public override string ToString()
    {
        float leftPos = _leftSectionDivider != null ? _leftSectionDivider.GetPosition() : 0;
        float rightPos = _rightSectionDivider != null ? _rightSectionDivider.GetPosition() : 1;
        return $"[{leftPos};{rightPos}]";
    }
}